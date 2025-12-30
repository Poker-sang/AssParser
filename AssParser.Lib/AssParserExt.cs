using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssParser.Lib;

public static class AssParserExt
{
    /// <summary>
    /// Export all used fonts. All used chars are listed in FontDetail.UsedChar, including \h.
    /// Italic, Bold and @(vertical alignment) is considered as different font.
    /// </summary>
    /// <param name="assSubtitle"></param>
    /// <returns>List of distinct used fonts.</returns>
    /// <exception cref="Exception">If there is any invalid part.</exception>
    public static IReadOnlyList<FontDetail> UsedFonts(this AssSubtitleModel assSubtitle)
    {
        ConcurrentDictionary<FontDetail, ConcurrentDictionary<char, byte>> result = new();
        Dictionary<string, Style> styles = [];
        foreach (var style in assSubtitle.Styles.StylesList)
        {
            _ = styles.TryAdd(style.Name, style);
        }
        _ = Parallel.ForEach(assSubtitle.Events.EventsList, item =>
        {
            var spLeft = item.Text.Split('{').ToList();
            var currentStyle = styles[item.Style];
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
                var word = text.Replace("\\N", "").Replace("\\n", "").Replace("\\h", "\u00A0");
                if (word.Length > 0)
                {
                    var bold = currentStyle.Bold is not "0" ? 1 : 0;
                    var isItalic = currentStyle.Italic is not "0";
                    var detail = new FontDetail
                    {
                        FontName = currentStyle.Fontname,
                        UsedChar = word,
                        Bold = bold,
                        IsItalic = isItalic
                    };
                    var charDir = result.GetOrAdd(detail, new ConcurrentDictionary<char, byte>());
                    foreach (var c in word)
                    {
                        _ = charDir.TryAdd(c, 0);
                    }
                }
            }
            foreach (var s in spLeft)
            {
                var spRight = s.Split('}');
                if (spRight.Length > 0)
                {
                    var tags = spRight[0].Split("\\");
                    foreach (var t in tags)
                    {
                        if (t.Length == 0)
                        {
                            continue;
                        }
                        switch (t[0])
                        {
                            case 'f':
                                if (t.Length > 2 && t[1] is 'n')
                                    currentStyle.Fontname = t[2..];
                                break;
                            case 'b':
                                if (t.Length is 2 or 4)
                                    if (int.TryParse(t[1..], out var weight))
                                        currentStyle.Bold = weight.ToString();
                                break;
                            case 'i':
                                if (t.Length is 2)
                                    currentStyle.Italic = t[1..];
                                break;
                            case 'r':
                                currentStyle = t.Length switch
                                {
                                    1 => styles[item.Style],
                                    > 1 when !styles.ContainsKey(t[1..]) => throw new Exception($"Style {t} not found"),
                                    > 1 => styles[t[1..]],
                                    _ => currentStyle
                                };
                                break;
                            default:
                                break;
                        }
                    }
                    if (spRight.Length > 1)
                    {
                        var word = spRight[1].Replace("\\N", "").Replace("\\n", "").Replace("\\h", "\u00A0");
                        if (word.Length > 0)
                        {
                            var bold = Convert.ToInt32(currentStyle.Bold);
                            bold = bold == -1 ? 1 : bold;
                            var isItalic = currentStyle.Italic != "0";
                            var detail = new FontDetail()
                            {
                                FontName = currentStyle.Fontname,
                                UsedChar = word,
                                Bold = bold,
                                IsItalic = isItalic
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
            var sb = new StringBuilder();
            foreach (var c in s.Value)
                _ = sb.Append(c.Key);
            s.Key.UsedChar = sb.ToString();
            fonts[i] = s.Key;
            i++;
        }
        return fonts;
    }
}
