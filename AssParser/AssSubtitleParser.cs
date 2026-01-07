using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AssParser;

/// <summary>
/// Provides static methods for parsing Advanced SubStation Alpha (ASS) subtitle files asynchronously into structured
/// subtitle models.
/// </summary>
public static class AssSubtitleParser
{
    /// <summary>
    /// Parse an ASS stream asynchronously.
    /// </summary>
    /// <param name="stream">A stream of your ass file.</param>
    /// <param name="strictnessLevel">Specifies the strictness levels while parsing.</param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <exception cref="AssSubtitleParserException">If there is any invalid part.</exception>
    public static async Task<AssSubtitleModel> ParseAsync(Stream stream, StrictnessLevel strictnessLevel = StrictnessLevel.Strict, CancellationToken token = default)
    {
        var lineCount = 0;
        var ord = new List<string>();
        ScriptInfo? scriptInfo = null;
        Styles? styles = null;
        Events? events = null;
        var extraSections = new Dictionary<string, string>();
        using var assStream = new StreamReader(stream, leaveOpen: true);
        while (await assStream.ReadLineAsync(token) is { } sectionLine)
        {
            lineCount++;
            sectionLine = sectionLine.Trim();
            if (string.IsNullOrEmpty(sectionLine))
                continue;
            if (sectionLine is not ['[', .. var section, ']'])
            {
                if (strictnessLevel.HasFlagFast(StrictnessLevel.AllowInvalidSections))
                    continue;
                throw new AssSubtitleParserException(lineCount, AssSubtitleParserException.ErrorType.InvalidSection, $"{sectionLine} is not a valid section name");
            }
            ord.Add(section);
            switch (section)
            {
                case AssConstants.ScriptInfoSection:
                {
                    scriptInfo = new();
                    while (assStream.Peek() is var peek and not '[' and > -1)
                    {
                        if (peek is '\r' or '\n')
                        {
                            if (peek is '\n')
                                lineCount++;
                            _ = assStream.Read();
                            continue;
                        }

                        var (header, body, line) = await ParseLineAsync(assStream, token);

                        if (line is [';', .. var remains])
                            scriptInfo.Comments.Add(remains.Trim());
                        else
                            switch (header)
                            {
                                case var _ when body is null: goto default;
                                case "!": scriptInfo.Comments.Add(body.Trim()); break;
                                case nameof(ScriptInfo.Title): scriptInfo.Title = body; break;
                                case AssConstants.OriginalScriptLine: scriptInfo.OriginalScript = body; break;
                                case AssConstants.OriginalTranslationLine: scriptInfo.OriginalTranslation = body; break;
                                case AssConstants.OriginalEditingLine: scriptInfo.OriginalEditing = body; break;
                                case AssConstants.SynchPointLine: scriptInfo.SynchPoint = body; break;
                                case AssConstants.ScriptUpdatedByLine: scriptInfo.ScriptUpdatedBy = body; break;
                                case AssConstants.UpdateDetailsLine: scriptInfo.UpdateDetails = body; break;
                                case nameof(ScriptInfo.ScriptType): scriptInfo.ScriptType = body; break;
                                case nameof(ScriptInfo.Collisions): scriptInfo.Collisions = body; break;
                                case nameof(ScriptInfo.PlayResX): scriptInfo.PlayResX = body.GetInt(); break;
                                case nameof(ScriptInfo.PlayResY): scriptInfo.PlayResY = body.GetInt(); break;
                                case nameof(ScriptInfo.PlayDepth): scriptInfo.PlayDepth = body.GetInt(); break;
                                case nameof(ScriptInfo.Timer): scriptInfo.Timer = body.GetFloat(); break;
                                case nameof(ScriptInfo.WrapStyle): scriptInfo.WrapStyle = body.GetEnum<WrapStyle>(); break;
                                case nameof(ScriptInfo.ScaledBorderAndShadow): scriptInfo.ScaledBorderAndShadow = body.GetIsYes(); break;
                                case AssConstants.YCbCrMatrixLine: scriptInfo.YCbCrMatrix = body.GetYCbCrMatrix(); break;
                                default:
                                    if (!strictnessLevel.HasFlagFast(StrictnessLevel.AllowInvalidLines))
                                        throw new AssSubtitleParserException(lineCount, AssSubtitleParserException.ErrorType.InvalidEventLine, $"Invalid script info: {header}");
                                    break;
                            }

                        lineCount++;
                    }

                    break;
                }
                case AssConstants.V4StylesSection or AssConstants.V4PStylesSection:
                {
                    var (header, body, _) = await ParseLineAsync(assStream, token);
                    lineCount++;
                    var formatLine = lineCount;
                    if (header is not AssConstants.FormatLine || body is null)
                        throw new AssSubtitleParserException(formatLine, AssSubtitleParserException.ErrorType.MissingFormatLine, "No format line");
                    var format = body.Split(',');
                    for (var i = 0; i < format.Length; i++)
                        format[i] = format[i].Trim();
                    styles = new() { Format = format };

                    // Read style lines
                    while (assStream.Peek() is var peek and not '[' and > -1)
                    {
                        if (peek is '\r' or '\n')
                        {
                            if (peek is '\n')
                                lineCount++;
                            _ = assStream.Read();
                            continue;
                        }

                        var (header2, body2, _) = await ParseLineAsync(assStream, token);
                        lineCount++;
                        if (header2 is not AssConstants.StyleLine || body2 is null)
                            throw new AssSubtitleParserException(lineCount, AssSubtitleParserException.ErrorType.InvalidStyleLine, "Invalid style line");
                        var data = body2.Split(',');
                        var style = new Style();
                        for (var i = 0; i < format.Length; i++)
                            switch (format[i])
                            {
                                case nameof(Style.Name): style.Name = data[i]; break;
                                case nameof(Style.Fontname): style.Fontname = data[i]; break;
                                case nameof(Style.Fontsize): style.Fontsize = data[i].GetInt(); break;
                                case nameof(Style.PrimaryColour): style.PrimaryColour = data[i].GetColor(); break;
                                case nameof(Style.SecondaryColour): style.SecondaryColour = data[i].GetColor(); break;
                                case nameof(Style.OutlineColour): style.OutlineColour = data[i].GetColor(); break;
                                case nameof(Style.BackColour): style.BackColour = data[i].GetColor(); break;
                                case nameof(Style.Bold): style.Bold = data[i].GetIsN1(); break;
                                case nameof(Style.Italic): style.Italic = data[i].GetIsN1(); break;
                                case nameof(Style.Underline): style.Underline = data[i].GetIsN1(); break;
                                case nameof(Style.StrikeOut): style.StrikeOut = data[i].GetIsN1(); break;
                                case nameof(Style.ScaleX): style.ScaleX = data[i].GetInt(); break;
                                case nameof(Style.ScaleY): style.ScaleY = data[i].GetInt(); break;
                                case nameof(Style.Spacing): style.Spacing = data[i].GetInt(); break;
                                case nameof(Style.Angle): style.Angle = data[i].GetFloat(); break;
                                case nameof(Style.BorderStyle): style.BorderStyle = data[i].GetEnum<BorderStyle>(); break;
                                case nameof(Style.Outline): style.Outline = data[i].GetFloat(); break;
                                case nameof(Style.Shadow): style.Shadow = data[i].GetFloat(); break;
                                case nameof(Style.Alignment): style.Alignment = data[i].GetEnum<Alignment>(); break;
                                case nameof(Style.MarginL): style.MarginL = data[i].GetInt(); break;
                                case nameof(Style.MarginR): style.MarginR = data[i].GetInt(); break;
                                case nameof(Style.MarginV): style.MarginV = data[i].GetInt(); break;
                                case nameof(Style.AlphaLevel): style.AlphaLevel = data[i].GetInt(); break;
                                case nameof(Style.Encoding): style.Encoding = data[i].GetInt(); break;
                                default:
                                    if (!strictnessLevel.HasFlagFast(StrictnessLevel.AllowInvalidLines))
                                        throw new AssSubtitleParserException(formatLine, AssSubtitleParserException.ErrorType.InvalidStyleLine, $"Invalid style: {format[i]}");
                                    break;
                            }

                        styles.Add(style);
                    }

                    break;
                }
                case AssConstants.EventsSection:
                {
                    var (header, body, _) = await ParseLineAsync(assStream, token);
                    lineCount++;
                    var eventLine = lineCount;
                    if (header is not AssConstants.FormatLine || body is null)
                        throw new AssSubtitleParserException(eventLine, AssSubtitleParserException.ErrorType.MissingFormatLine, "No format line");
                    var format = body.Split(',');
                    for (var i = 0; i < format.Length; i++)
                        format[i] = format[i].Trim();
                    events = new() { Format = format };

                    // Read event lines
                    while (assStream.Peek() is var peek and not '[' and > -1)
                    {
                        if (peek is '\r' or '\n')
                        {
                            if (peek is '\n')
                                lineCount++;
                            _ = assStream.Read();
                            continue;
                        }

                        var (header2, body2, _) = await ParseLineAsync(assStream, token);
                        lineCount++;
                        if (body2 is null)
                            throw new AssSubtitleParserException(lineCount, AssSubtitleParserException.ErrorType.InvalidEventLine, "Invalid event line");
                        if (header2 is AssConstants.CommentEventLine or AssConstants.DialogueEventLine)
                        {
                            var e = new Event
                            {
                                Type = header2 is AssConstants.CommentEventLine
                                    ? EventType.Comment
                                    : EventType.Dialogue
                            };
                            var data = body2.Split(',');
                            for (var i = 0; i < format.Length; i++)
                                switch (format[i])
                                {
                                    case nameof(Event.Layer): e.Layer = data[i].GetInt(); break;
                                    case nameof(Event.Start): e.Start = data[i].GetTimeSpan(); break;
                                    case nameof(Event.End): e.End = data[i].GetTimeSpan(); break;
                                    case nameof(Event.Style): e.Style = data[i]; break;
                                    case nameof(Event.Name): e.Name = data[i]; break;
                                    case nameof(Event.MarginL): e.MarginL = data[i].GetInt(); break;
                                    case nameof(Event.MarginR): e.MarginR = data[i].GetInt(); break;
                                    case nameof(Event.MarginV): e.MarginV = data[i].GetInt(); break;
                                    case nameof(Event.Effect): e.Effect = data[i]; break;
                                    case nameof(Event.Text): e.Text = string.Join(',', data[i..]); break;
                                    default:
                                        if (!strictnessLevel.HasFlagFast(StrictnessLevel.AllowInvalidLines))
                                            throw new AssSubtitleParserException(eventLine, AssSubtitleParserException.ErrorType.InvalidEventLine, $"Invalid event: {format[i]}");
                                        break;
                                }

                            events.Add(e);
                        }
                        else
                        {
                            var type = header2 switch
                            {
                                AssConstants.PictureEventLine => OtherEventType.Picture,
                                AssConstants.SoundEventLine => OtherEventType.Sound,
                                AssConstants.MovieEventLine => OtherEventType.Movie,
                                AssConstants.CommandEventLine => OtherEventType.Command,
                                _ => strictnessLevel.HasFlagFast(StrictnessLevel.AllowInvalidLines)
                                    ? (OtherEventType) (-1)
                                    : throw new AssSubtitleParserException(lineCount, AssSubtitleParserException.ErrorType.InvalidEventLine, "Invalid event line")
                            };
                            if (type is not (OtherEventType) (-1))
                                events.OtherEvents.Add((type, body2));
                        }
                    }

                    break;
                }
                default:
                {
                    if (!strictnessLevel.HasFlagFast(StrictnessLevel.AllowUnknownSections) && !AssConstants.KnownExtraSections.Contains(section))
                        throw new AssSubtitleParserException(lineCount, AssSubtitleParserException.ErrorType.UnknownSection, $"Unknown section: [{section}]");
                    var bodyBuffer = new StringBuilder();
                    while (assStream.Peek() is not '\r' and not '\n' and > -1)
                    {
                        _ = bodyBuffer.AppendLine(await assStream.ReadLineAsync(token));
                        lineCount++;
                    }

                    extraSections.Add(section, bodyBuffer.ToString());
                    break;
                }
            }
        }

        if (scriptInfo is null)
            throw new AssSubtitleParserException(0, AssSubtitleParserException.ErrorType.MissingSection, $"No [{AssConstants.ScriptInfoSection}] section found");
        if (styles is null)
            throw new AssSubtitleParserException(0, AssSubtitleParserException.ErrorType.MissingSection, $"No [{AssConstants.V4StylesSection}] / [{AssConstants.V4PStylesSection}] section found");
        if (events is null)
            throw new AssSubtitleParserException(0, AssSubtitleParserException.ErrorType.MissingSection, $"No [{AssConstants.EventsSection}] section found");

        AssSubtitleModel assSubtitleModel = new()
        {
            Ord = ord,
            ScriptInfo = scriptInfo,
            Styles = styles,
            Events = events,
            ExtraSections = extraSections
        };

        return assSubtitleModel;

        static async Task<(string? Header, string? Body, string? Line)> ParseLineAsync(StreamReader streamReader, CancellationToken token)
        {
            if (await streamReader.ReadLineAsync(token) is not { } line)
                return (null, null, null);
            var index = line.IndexOf(':');
            return index is -1
                ? (line.Trim(), null, line)
                : (line[..index].Trim(), line[(index + 1)..].Trim(), line);
        }
    }

    /// <summary>
    /// Parse an ASS file asynchronously.
    /// </summary>
    /// <param name="assFile">Path to your ass file.</param>
    /// <param name="strictnessLevel">Specifies the strictness levels while parsing.</param>
    /// <param name="token"></param>
    /// <returns>Parsed AssSubtitleModel object.</returns>
    /// <exception cref="AssSubtitleParserException">If there is any invalid part.</exception>
    public static async Task<AssSubtitleModel> ParseFileAsync(string assFile, StrictnessLevel strictnessLevel = StrictnessLevel.Strict, CancellationToken token = default)
    {
        await using var assStream = new FileStream(assFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        return await ParseAsync(assStream, strictnessLevel, token);
    }

    extension(string str)
    {
        private Color GetColor()
        {
            if (str is not ['&', 'H', .. var remains])
                throw new FormatException($"{str} is not a valid ABGR color string.");
            var abgr = uint.Parse(remains, NumberStyles.HexNumber);
            var span = MemoryMarshal.CreateSpan(ref Unsafe.As<uint, byte>(ref abgr), 4);
            return Color.FromArgb(byte.MaxValue - span[3], span[0], span[1], span[2]);
        }

        private bool GetIsN1() => str is "-1";

        private int GetInt() => int.Parse(str);

        private float GetFloat() => float.Parse(str);

        private T GetEnum<T>() where T : struct, Enum => (T) Enum.ToObject(typeof(T), int.Parse(str));

        private TimeSpan GetTimeSpan() => TimeSpan.ParseExact(str, @"h\:mm\:ss\.ff", null);

        private bool GetIsYes() => str.ToLower() is "yes";

        private YCbCrMatrix GetYCbCrMatrix() => YCbCrMatrix.Parse(str);
    }

    extension(StrictnessLevel e)
    {
        private bool HasFlagFast(StrictnessLevel flag) => (e & flag) == flag;
    }

    extension(Color color)
    {
        internal string GetAbgrString() => $"&H{byte.MaxValue - color.A:X2}{color.B:X2}{color.G:X2}{color.R:X2}";
    }

    extension(bool b)
    {
        internal string GetN1String() => (b ? -1 : 0).ToString();

        internal string GetYesNoString() => b ? "yes" : "no";
    }

    extension<T>(T e) where T : struct, Enum
    {
        internal string GetEnumIntString() => Convert.ToInt32(e).ToString();
    }

    extension(TimeSpan timeSpan)
    {
        internal string GetTimeSpanString() => timeSpan.ToString(@"h\:mm\:ss\.ff");
    }
}
