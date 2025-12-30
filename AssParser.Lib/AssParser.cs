using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AssParser.Lib;

public static class AssParser
{
    /// <summary>
    /// Parse a single ass file asynchronously.
    /// </summary>
    /// <param name="stream">A StreamReader of your ass file.</param>
    /// <returns></returns>
    /// <exception cref="AssParserException">If there is any invalid part.</exception>
    public static async Task<AssSubtitleModel> ParseAsync(Stream stream)
    {
        var lineCount = 0;
        var ord = new List<string>();
        ScriptInfo? scriptedInfo = null;
        Styles? styles = null;
        Events? events = null;
        var unknownSections = new Dictionary<string, string>();
        using var assStream = new StreamReader(stream, leaveOpen: true);
        while (await assStream.ReadLineAsync() is { } line)
        {
            lineCount++;
            if (string.IsNullOrEmpty(line))
                continue;
            if (line is not ['[', .. var tag, ']'])
                throw new AssParserException(lineCount, AssParserException.AssParserErrorType.InvalidSection, $"{line} is not a valid section name");
            ord.Add(tag);
            switch (tag)
            {
                case ScriptInfo.SectionName:
                {
                    var commentCount = 0;
                    var scriptInfoItems = new Dictionary<string, string?>();
                    while (assStream.Peek() is var peek and not '[' and > -1)
                    {
                        switch (peek)
                        {
                            case '\n':
                            {
                                lineCount++;
                                goto case '\r';
                            }
                            case '\r':
                            {
                                _ = assStream.Read();
                                continue;
                            }
                            case ';':
                                scriptInfoItems.Add($";{commentCount++}", await assStream.ReadLineAsync());
                                lineCount++;
                                continue;
                        }

                        var (header, body) = await ParseLineAsync(assStream);
                        lineCount++;
                        scriptInfoItems.Add(header, body);
                    }

                    scriptedInfo = new()
                    {
                        ScriptInfoItems = scriptInfoItems
                    };

                    break;
                }
                case Styles.SectionName:
                {
                    var (header, body) = await ParseLineAsync(assStream);
                    lineCount++;
                    var formatLine = lineCount;
                    if (header is not "Format")
                        throw new AssParserException(formatLine, AssParserException.AssParserErrorType.MissingFormatLine, "No format line");
                    var format = body.Split(',');
                    for (var i = 0; i < format.Length; i++)
                        format[i] = format[i].Trim();
                    var stylesList = new List<Style>();

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

                        var (header2, body2) = await ParseLineAsync(assStream);
                        lineCount++;
                        if (header2 is not "Style")
                            throw new AssParserException(lineCount, AssParserException.AssParserErrorType.InvalidStyleLine, "Wrong Style Line");
                        var data = body2.Split(',');
                        var style = new Style
                        {
                            LineNumber = lineCount
                        };
                        for (var i = 0; i < format.Length; i++)
                        {
                            switch (format[i])
                            {
                                case nameof(Style.Name): style.Name = data[i]; break;
                                case nameof(Style.Fontname): style.Fontname = data[i]; break;
                                case nameof(Style.Fontsize): style.Fontsize = data[i]; break;
                                case nameof(Style.PrimaryColour): style.PrimaryColour = data[i]; break;
                                case nameof(Style.SecondaryColour): style.SecondaryColour = data[i]; break;
                                case nameof(Style.OutlineColour): style.OutlineColour = data[i]; break;
                                case nameof(Style.BackColour): style.BackColour = data[i]; break;
                                case nameof(Style.Bold): style.Bold = data[i]; break;
                                case nameof(Style.Italic): style.Italic = data[i]; break;
                                case nameof(Style.Underline): style.Underline = data[i]; break;
                                case nameof(Style.StrikeOut): style.StrikeOut = data[i]; break;
                                case nameof(Style.ScaleX): style.ScaleX = data[i]; break;
                                case nameof(Style.ScaleY): style.ScaleY = data[i]; break;
                                case nameof(Style.Spacing): style.Spacing = data[i]; break;
                                case nameof(Style.Angle): style.Angle = data[i]; break;
                                case nameof(Style.BorderStyle): style.BorderStyle = data[i]; break;
                                case nameof(Style.Outline): style.Outline = data[i]; break;
                                case nameof(Style.Shadow): style.Shadow = data[i]; break;
                                case nameof(Style.Alignment): style.Alignment = data[i]; break;
                                case nameof(Style.MarginL): style.MarginL = data[i]; break;
                                case nameof(Style.MarginR): style.MarginR = data[i]; break;
                                case nameof(Style.MarginV): style.MarginV = data[i]; break;
                                case nameof(Style.Encoding): style.Encoding = data[i]; break;
                                default: throw new AssParserException(formatLine, AssParserException.AssParserErrorType.InvalidStyleLine, "Invalid style");
                            }
                        }

                        stylesList.Add(style);
                    }

                    styles = new()
                    {
                        Format = format,
                        StylesList = stylesList
                    };

                    break;
                }
                case Events.SectionName:
                {
                    var (header, body) = await ParseLineAsync(assStream);
                    lineCount++;
                    var eventLine = lineCount;
                    if (header is not "Format")
                        throw new AssParserException(eventLine, AssParserException.AssParserErrorType.MissingFormatLine, "No format line");
                    var format = body.Split(',');
                    for (var i = 0; i < format.Length; i++)
                        format[i] = format[i].Trim();
                    var eventsList = new List<Event>();

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

                        var (header2, body2) = await ParseLineAsync(assStream);
                        lineCount++;
                        var e = new Event
                        {
                            LineNumber = lineCount,
                            Type = header2 switch
                            {
                                nameof(EventType.Comment) => EventType.Comment,
                                nameof(EventType.Dialogue) => EventType.Dialogue,
                                _ => throw new AssParserException(lineCount, AssParserException.AssParserErrorType.InvalidEventLine, "Invalid event")
                            }
                        };
                        var data = body2.Split(',');
                        for (var i = 0; i < format.Length; i++)
                        {
                            switch (format[i])
                            {
                                case nameof(Event.Layer): e.Layer = data[i]; break;
                                case nameof(Event.Start): e.Start = data[i]; break;
                                case nameof(Event.End): e.End = data[i]; break;
                                case nameof(Event.Style): e.Style = data[i]; break;
                                case nameof(Event.Name): e.Name = data[i]; break;
                                case nameof(Event.MarginL): e.MarginL = data[i]; break;
                                case nameof(Event.MarginR): e.MarginR = data[i]; break;
                                case nameof(Event.MarginV): e.MarginV = data[i]; break;
                                case nameof(Event.Effect): e.Effect = data[i]; break;
                                case nameof(Event.Text): e.Text = string.Join(',', data[i..]); break;
                                default: throw new AssParserException(eventLine, AssParserException.AssParserErrorType.InvalidEventLine, "Invalid event");
                            }
                        }

                        eventsList.Add(e);
                    }

                    events = new()
                    {
                        Format = format,
                        EventsList = eventsList
                    };

                    break;
                }
                default:
                {
                    var bodyBuffer = new StringBuilder();
                    while (assStream.Peek() is not '\r' and not '\n' and > -1)
                    {
                        _ = bodyBuffer.AppendLine(await assStream.ReadLineAsync());
                        lineCount++;
                    }

                    unknownSections.Add(tag, bodyBuffer.ToString());
                    break;
                }
            }
        }

        if (ord.Count is 0)
            throw new AssParserException(0, AssParserException.AssParserErrorType.MissingSection, "No sections found");
        if (scriptedInfo is null)
            throw new AssParserException(0, AssParserException.AssParserErrorType.MissingSection, $"No [{ScriptInfo.SectionName}] section found");
        if (styles is null)
            throw new AssParserException(0, AssParserException.AssParserErrorType.MissingSection, $"No [{Styles.SectionName}] section found");
        if (events is null)
            throw new AssParserException(0, AssParserException.AssParserErrorType.MissingSection, $"No [{Events.SectionName}] section found");

        AssSubtitleModel assSubtitleModel = new()
        {
            Ord = ord,
            ScriptInfo = scriptedInfo,
            Styles = styles,
            Events = events,
            UnknownSections = unknownSections
        };

        return assSubtitleModel;

        static async Task<(string Header, string Body)> ParseLineAsync(StreamReader streamReader)
        {
            if (await streamReader.ReadLineAsync() is not { } line)
                return ("", "");
            var index = line.IndexOf(':');
            return index is -1
                ? (line.Trim(), "")
                : (line[..index].Trim(), line[(index + 1)..].Trim());
        }
    }

    /// <summary>
    /// Parse a single ass file asynchronously.
    /// </summary>
    /// <param name="assFile">Path to your ass file.</param>
    /// <returns>Parsed AssSubtitleModel object.</returns>
    /// <exception cref="AssParserException">If there is any invalid part.</exception>
    public static async Task<AssSubtitleModel> ParseFileAsync(string assFile)
    {
        await using var assStream = new FileStream(assFile, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        return await ParseAsync(assStream);
    }

    /// <summary>
    /// Build the ass file and write it into StreamWriter.
    /// </summary>
    /// <param name="assSubtitleModel">Ass model.</param>
    /// <param name="stream">Destination stream.</param>
    /// <returns>A Task.</returns>
    /// <exception cref="Exception">If there is any invalid element in the ass model.</exception>
    public static async Task WriteToStreamAsync(AssSubtitleModel assSubtitleModel, Stream stream)
    {
        await using var streamWriter = new StreamWriter(stream, leaveOpen: true);
        await streamWriter.WriteLineAsync($"[{ScriptInfo.SectionName}]");
        foreach (var item in assSubtitleModel.ScriptInfo.ScriptInfoItems)
            if (item.Key.StartsWith(';'))
                await streamWriter.WriteLineAsync(item.Value);
            else
                await streamWriter.WriteLineAsync($"{item.Key}: {item.Value}");
        await streamWriter.WriteLineAsync();
        if (assSubtitleModel.UnknownSections.TryGetValue(AssSubtitleModel.AegisubProjectGarbageSection, out var aegisubProjectGarbage))
        {
            await streamWriter.WriteLineAsync($"[{AssSubtitleModel.AegisubProjectGarbageSection}]");
            await streamWriter.WriteLineAsync(aegisubProjectGarbage);
        }
        await streamWriter.WriteLineAsync($"[{Styles.SectionName}]");
        await streamWriter.WriteLineAsync($"Format: {string.Join(", ", assSubtitleModel.Styles.Format)}");
        foreach (var style in assSubtitleModel.Styles.StylesList)
        {
            await streamWriter.WriteAsync("Style: ");
            for (var i = 0; i < assSubtitleModel.Styles.Format.Count; i++)
            {
                await streamWriter.WriteAsync(assSubtitleModel.Styles.Format[i] switch
                {
                    nameof(Style.Name) => style.Name,
                    nameof(Style.Fontname) => style.Fontname,
                    nameof(Style.Fontsize) => style.Fontsize,
                    nameof(Style.PrimaryColour) => style.PrimaryColour,
                    nameof(Style.SecondaryColour) => style.SecondaryColour,
                    nameof(Style.OutlineColour) => style.OutlineColour,
                    nameof(Style.BackColour) => style.BackColour,
                    nameof(Style.Bold) => style.Bold,
                    nameof(Style.Italic) => style.Italic,
                    nameof(Style.Underline) => style.Underline,
                    nameof(Style.StrikeOut) => style.StrikeOut,
                    nameof(Style.ScaleX) => style.ScaleX,
                    nameof(Style.ScaleY) => style.ScaleY,
                    nameof(Style.Spacing) => style.Spacing,
                    nameof(Style.Angle) => style.Angle,
                    nameof(Style.BorderStyle) => style.BorderStyle,
                    nameof(Style.Outline) => style.Outline,
                    nameof(Style.Shadow) => style.Shadow,
                    nameof(Style.Alignment) => style.Alignment,
                    nameof(Style.MarginL) => style.MarginL,
                    nameof(Style.MarginR) => style.MarginR,
                    nameof(Style.MarginV) => style.MarginV,
                    nameof(Style.Encoding) => style.Encoding,
                    _ => throw new ArgumentOutOfRangeException($"Invalid style {assSubtitleModel.Styles.Format[i]} in [{Styles.SectionName}]")
                });
                if (i != assSubtitleModel.Styles.Format.Count - 1)
                    await streamWriter.WriteAsync(',');
            }
            await streamWriter.WriteAsync(Environment.NewLine);
        }
        await streamWriter.WriteLineAsync();
        if (assSubtitleModel.UnknownSections.TryGetValue(AssSubtitleModel.FontsSection, out var fonts))
        {
            await streamWriter.WriteLineAsync($"[{AssSubtitleModel.FontsSection}]");
            await streamWriter.WriteLineAsync(fonts);
        }
        if (assSubtitleModel.UnknownSections.TryGetValue(AssSubtitleModel.GraphicsSection, out var graphics))
        {
            await streamWriter.WriteLineAsync($"[{AssSubtitleModel.GraphicsSection}]");
            await streamWriter.WriteLineAsync(graphics);
        }
        await streamWriter.WriteLineAsync($"[{Events.SectionName}]");
        await streamWriter.WriteAsync($"Format: {string.Join(", ", assSubtitleModel.Events.Format)}");
        foreach (var item in assSubtitleModel.Events.EventsList)
        {
            await streamWriter.WriteAsync(Environment.NewLine);
            await streamWriter.WriteAsync($"{item.Type}: ");
            for (var i = 0; i < assSubtitleModel.Events.Format.Count; i++)
            {
                await (assSubtitleModel.Events.Format[i] switch
                {
                    nameof(Event.Layer) => streamWriter.WriteAsync(item.Layer),
                    nameof(Event.Start) => streamWriter.WriteAsync(item.Start),
                    nameof(Event.End) => streamWriter.WriteAsync(item.End),
                    nameof(Event.Style) => streamWriter.WriteAsync(item.Style),
                    nameof(Event.Name) => streamWriter.WriteAsync(item.Name),
                    nameof(Event.MarginL) => streamWriter.WriteAsync(item.MarginL),
                    nameof(Event.MarginR) => streamWriter.WriteAsync(item.MarginR),
                    nameof(Event.MarginV) => streamWriter.WriteAsync(item.MarginV),
                    nameof(Event.Effect) => streamWriter.WriteAsync(item.Effect),
                    nameof(Event.Text) => streamWriter.WriteAsync(item.Text),
                    _ => throw new ArgumentOutOfRangeException($"Invalid style {assSubtitleModel.Events.Format[i]}")
                });
                if (i != assSubtitleModel.Events.Format.Count - 1)
                    await streamWriter.WriteAsync(',');
            }
        }
        if (assSubtitleModel.UnknownSections.TryGetValue(AssSubtitleModel.AegisubExtradataSection, out var aegisubExtradata))
        {
            await streamWriter.WriteLineAsync();
            await streamWriter.WriteLineAsync();
            await streamWriter.WriteLineAsync($"[{AssSubtitleModel.AegisubExtradataSection}]");
            await streamWriter.WriteAsync(aegisubExtradata);
        }
    }
}
