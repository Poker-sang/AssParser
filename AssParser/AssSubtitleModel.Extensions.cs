using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AssParser;

public partial class AssSubtitleModel
{
    /// <summary>
    /// Build the ass file and write it into StreamWriter.
    /// </summary>
    /// <param name="stream">Destination stream.</param>
    /// <returns>A Task.</returns>
    /// <exception cref="Exception">If there is any invalid element in the ass model.</exception>
    public async Task WriteToStreamAsync(Stream stream)
    {
        await using var streamWriter = new StreamWriter(stream, leaveOpen: true);
        foreach (var ord in Ord)
        {
            await (ord switch
            {
                AssConstants.ScriptInfoSection => WriteScriptInfoAsync(streamWriter),
                AssConstants.V4StylesSection or AssConstants.V4PStylesSection => WriteStylesAsync(streamWriter, ord),
                AssConstants.EventsSection => WriteEventsAsync(streamWriter),
                _ when AssConstants.KnownExtraSections.Contains(ord) => WriteExtraSectionAsync(streamWriter, ord),
                _ => throw new ArgumentOutOfRangeException($"Invalid section {ord}")
            });
            await streamWriter.WriteLineAsync();
        }
    }

    private async Task WriteScriptInfoAsync(StreamWriter streamWriter)
    {
        await streamWriter.WriteLineAsync($"[{AssConstants.ScriptInfoSection}]");

        foreach (var comment in ScriptInfo.Comments)
            await streamWriter.WriteLineAsync($"; {comment}");

        await streamWriter.WriteLineAsync($"{nameof(ScriptInfo.Title)}: {ScriptInfo.Title}");
        await streamWriter.WriteLineAsync($"{AssConstants.OriginalScriptLine}: {ScriptInfo.OriginalScript}");

        if (ScriptInfo.OriginalTranslation is { } originalTranslation)
            await streamWriter.WriteLineAsync($"{AssConstants.OriginalTranslationLine}: {originalTranslation}");

        if (ScriptInfo.OriginalEditing is { } originalEditing)
            await streamWriter.WriteLineAsync($"{AssConstants.OriginalEditingLine}: {originalEditing}");

        if (ScriptInfo.OriginalTiming is { } originalTiming)
            await streamWriter.WriteLineAsync($"{AssConstants.OriginalTimingLine}: {originalTiming}");

        if (ScriptInfo.SynchPoint is { } synchPoint)
            await streamWriter.WriteLineAsync($"{AssConstants.SynchPointLine}: {synchPoint}");

        if (ScriptInfo.ScriptUpdatedBy is { } scriptUpdatedBy)
            await streamWriter.WriteLineAsync($"{AssConstants.ScriptUpdatedByLine}: {scriptUpdatedBy}");

        if (ScriptInfo.UpdateDetails is { } updateDetails)
            await streamWriter.WriteLineAsync($"{AssConstants.UpdateDetailsLine}: {updateDetails}");

        await streamWriter.WriteLineAsync($"{nameof(ScriptInfo.ScriptType)}: {ScriptInfo.ScriptType}");
        await streamWriter.WriteLineAsync($"{nameof(ScriptInfo.Collisions)}: {ScriptInfo.Collisions}");
        await streamWriter.WriteLineAsync($"{nameof(ScriptInfo.PlayResX)}: {ScriptInfo.PlayResX}");
        await streamWriter.WriteLineAsync($"{nameof(ScriptInfo.PlayResY)}: {ScriptInfo.PlayResY}");
        await streamWriter.WriteLineAsync($"{nameof(ScriptInfo.PlayDepth)}: {ScriptInfo.PlayDepth}");
        await streamWriter.WriteLineAsync($"{nameof(ScriptInfo.Timer)}: {ScriptInfo.Timer}");
        await streamWriter.WriteLineAsync($"{nameof(ScriptInfo.WrapStyle)}: {ScriptInfo.WrapStyle.GetEnumIntString()}");
        await streamWriter.WriteLineAsync($"{nameof(ScriptInfo.ScaledBorderAndShadow)}: {ScriptInfo.ScaledBorderAndShadow.GetYesNoString()}");
        await streamWriter.WriteLineAsync($"{AssConstants.YCbCrMatrixLine}: {ScriptInfo.YCbCrMatrix}");
    }

    private async Task WriteStylesAsync(StreamWriter streamWriter, string stylesSectionName)
    {
        await streamWriter.WriteLineAsync($"[{stylesSectionName}]");
        await streamWriter.WriteLineAsync($"{AssConstants.FormatLine}: {string.Join(", ", Styles.Format)}");
        foreach (var style in Styles)
        {
            await streamWriter.WriteAsync($"{AssConstants.StyleLine}: ");
            var first = true;
            foreach (var format in Styles.Format)
            {
                if (first)
                    first = false;
                else
                    await streamWriter.WriteAsync(',');
                await streamWriter.WriteAsync(format switch
                {
                    nameof(Style.Name) => style.Name,
                    nameof(Style.Fontname) => style.Fontname,
                    nameof(Style.Fontsize) => style.Fontsize.ToString(),
                    nameof(Style.PrimaryColour) => style.PrimaryColour.GetAbgrString(),
                    nameof(Style.SecondaryColour) => style.SecondaryColour.GetAbgrString(),
                    nameof(Style.OutlineColour) => style.OutlineColour.GetAbgrString(),
                    nameof(Style.BackColour) => style.BackColour.GetAbgrString(),
                    nameof(Style.Bold) => style.Bold.GetN1String(),
                    nameof(Style.Italic) => style.Italic.GetN1String(),
                    nameof(Style.Underline) => style.Underline.GetN1String(),
                    nameof(Style.StrikeOut) => style.StrikeOut.GetN1String(),
                    nameof(Style.ScaleX) => style.ScaleX.ToString(),
                    nameof(Style.ScaleY) => style.ScaleY.ToString(),
                    nameof(Style.Spacing) => style.Spacing.ToString(),
                    nameof(Style.Angle) => style.Angle.ToString(CultureInfo.InvariantCulture),
                    nameof(Style.BorderStyle) => style.BorderStyle.GetEnumIntString(),
                    nameof(Style.Outline) => style.Outline.ToString(CultureInfo.InvariantCulture),
                    nameof(Style.Shadow) => style.Shadow.ToString(CultureInfo.InvariantCulture),
                    nameof(Style.Alignment) => style.Alignment.GetEnumIntString(),
                    nameof(Style.MarginL) => style.MarginL.ToString(),
                    nameof(Style.MarginR) => style.MarginR.ToString(),
                    nameof(Style.MarginV) => style.MarginV.ToString(),
                    nameof(Style.AlphaLevel) => style.AlphaLevel.ToString(),
                    nameof(Style.Encoding) => style.Encoding.ToString(),
                    _ => throw new ArgumentOutOfRangeException($"Invalid style {format} in [{stylesSectionName}]")
                });
            }
            await streamWriter.WriteLineAsync();
        }
    }

    private async Task WriteEventsAsync(StreamWriter streamWriter)
    {
        await streamWriter.WriteLineAsync($"[{AssConstants.EventsSection}]");
        await streamWriter.WriteLineAsync($"{AssConstants.FormatLine}: {string.Join(", ", Events.Format)}");
        foreach (var item in Events)
        {
            await streamWriter.WriteAsync($"{item.Type}: ");
            var first = true;
            foreach (var format in Events.Format)
            {
                if (first)
                    first = false;
                else
                    await streamWriter.WriteAsync(',');
                await streamWriter.WriteAsync(format switch
                {
                    nameof(Event.Layer) => item.Layer.ToString(),
                    nameof(Event.Start) => item.Start.GetTimeSpanString(),
                    nameof(Event.End) => item.End.GetTimeSpanString(),
                    nameof(Event.Style) => item.Style,
                    nameof(Event.Name) => item.Name,
                    nameof(Event.MarginL) => item.MarginL.ToString(),
                    nameof(Event.MarginR) => item.MarginR.ToString(),
                    nameof(Event.MarginV) => item.MarginV.ToString(),
                    nameof(Event.Effect) => item.Effect,
                    nameof(Event.Text) => item.Text,
                    _ => throw new ArgumentOutOfRangeException($"Invalid style {format}")
                });
            }
            await streamWriter.WriteLineAsync();
        }
        foreach (var item in Events.OtherEvents)
            await streamWriter.WriteLineAsync($"{item.Type}: {item.Content}");
    }

    private async Task WriteExtraSectionAsync(StreamWriter streamWriter, string sectionName)
    {
        var unknownSection = ExtraSections[sectionName];
        await streamWriter.WriteLineAsync($"[{sectionName}]");
        await streamWriter.WriteLineAsync(unknownSection);
    }

    /// <summary>
    /// Export all used fonts. All used chars are listed in <see cref="FontDetail.UsedChar"/>, including \h.
    /// Italic, Bold and @(vertical alignment) is considered as different font.
    /// </summary>
    /// <returns>List of distinct used fonts.</returns>
    /// <exception cref="Exception">If there is any invalid part.</exception>
    public IReadOnlyList<FontDetail> UsedFonts()
    {
        ConcurrentDictionary<FontDetail, ConcurrentDictionary<char, byte>> result = new();
        Dictionary<string, Style> styles = [];
        foreach (var style in Styles)
            _ = styles.TryAdd(style.Name, style);
        _ = Parallel.ForEach(Events, item =>
        {
            var spLeft = item.Text.Split('{').ToList();
            var currentStyle = styles[item.Style];
            var currentBold = currentStyle.Bold ? 1 : 0;
            if (!item.Text.StartsWith('{'))
            {
                string text;
                if (spLeft.Count is 0)
                    text = item.Text;
                else
                {
                    text = spLeft[0];
                    spLeft.RemoveAt(0);
                }
                var word = text.Replace("\\N", "").Replace("\\n", "").Replace("\\h", "\u00A0").ToCharArray();
                if (word.Length > 0)
                {
                    var detail = new FontDetail
                    {
                        FontName = currentStyle.Fontname,
                        Bold = currentBold,
                        IsItalic = currentStyle.Italic,
                        UsedChar = word
                    };
                    var charDir = result.GetOrAdd(detail, new ConcurrentDictionary<char, byte>());
                    foreach (var c in word)
                        _ = charDir.TryAdd(c, 0);
                }
            }
            foreach (var s in spLeft)
            {
                var spRight = s.Split('}');
                if (spRight.Length > 0)
                {
                    var tags = spRight[0].Split('\\');
                    foreach (var t in tags)
                    {
                        if (t is not [var first, .. var remains])
                            continue;
                        switch (first)
                        {
                            case 'f':
                                if (remains is ['n', .. { Length: > 0 } r])
                                    currentStyle.Fontname = r;
                                break;
                            case 'b':
                                if (remains.Length is 1 or 3)
                                    if (int.TryParse(remains, out var weight))
                                        currentBold = weight;
                                break;
                            case 'i':
                                if (remains.Length is 1)
                                    if (int.TryParse(remains, out var italic))
                                        currentStyle.Italic = italic is not 0;
                                break;
                            case 'r':
                                currentStyle = remains.Length is 0 
                                    ? styles[item.Style]
                                    : styles[remains];
                                currentBold = currentStyle.Bold ? 1 : 0;
                                break;
                            default:
                                break;
                        }
                    }
                    if (spRight.Length > 1)
                    {
                        var word = spRight[1].Replace("\\N", "").Replace("\\n", "").Replace("\\h", "\u00A0").ToCharArray();
                        if (word.Length > 0)
                        {
                            var detail = new FontDetail
                            {
                                FontName = currentStyle.Fontname,
                                Bold = currentBold,
                                IsItalic = currentStyle.Italic,
                                UsedChar = word
                            };
                            var charDir = result.GetOrAdd(detail, new ConcurrentDictionary<char, byte>());
                            foreach (var c in word)
                                _ = charDir.TryAdd(c, 0);
                        }
                    }
                }
            }
        });
        var fonts = new FontDetail[result.Count];
        var i = 0;
        foreach (var s in result)
        {
            var chars = s.Value.Keys.Order().ToArray();
            s.Key.UsedChar = chars;
            fonts[i] = s.Key;
            i++;
        }
        return fonts;
    }
}
