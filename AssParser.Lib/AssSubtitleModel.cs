using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AssParser.Lib;

public class AssSubtitleModel
{
    public required ScriptInfo ScriptInfo { get; set; }

    public required Styles Styles { get; set; }

    public required Events Events { get; set; }

    public required IList<string> Ord { get; set; }

    public required Dictionary<string, string> UnknownSections { get; set; }

    public async Task<string> GetStringAsync()
    {
        await using var stream = new MemoryStream();
        await AssParser.WriteToStreamAsync(this, stream);
        stream.Position = 0;
        using StreamReader reader = new(stream, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }

    public const string AegisubProjectGarbageSection = "Aegisub Project Garbage";

    public const string AegisubExtradataSection = "Aegisub Extradata";

    public const string FontsSection = "Fonts";

    public const string GraphicsSection = "Graphics";
}

public class ScriptInfo
{
    public const string SectionName = "Script Info";

    public required Dictionary<string, string?> ScriptInfoItems { get; set; }

    public string? Title
    {
        get => ScriptInfoItems.GetValueOrDefault(nameof(Title));
        set => ScriptInfoItems[nameof(Title)] = value;
    }

    public string? ScriptType
    {
        get => ScriptInfoItems.GetValueOrDefault(nameof(ScriptType));
        set => ScriptInfoItems[nameof(ScriptType)] = value;
    }

    public int? WrapStyle
    {
        get => GetIntOrDefault(nameof(WrapStyle));
        set => ScriptInfoItems[nameof(WrapStyle)] = value.ToString();
    }

    public string? ScaledBorderAndShadow
    {
        get => ScriptInfoItems.GetValueOrDefault(nameof(ScaledBorderAndShadow));
        set => ScriptInfoItems[nameof(ScaledBorderAndShadow)] = value;
    }

    public string? YCbCrMatrix
    {
        get => ScriptInfoItems.GetValueOrDefault(nameof(YCbCrMatrix));
        set => ScriptInfoItems[nameof(YCbCrMatrix)] = value;
    }

    public int? PlayResX
    {
        get => GetIntOrDefault(nameof(PlayResX));
        set => ScriptInfoItems[nameof(PlayResX)] = value.ToString();
    }

    public int? PlayResY
    {
        get => GetIntOrDefault(nameof(PlayResY));
        set => ScriptInfoItems[nameof(PlayResY)] = value.ToString();
    }

    private int? GetIntOrDefault(string key)
    {
        if (ScriptInfoItems.TryGetValue(key, out var item))
            return Convert.ToInt32(item);
        return null;
    }
}

public class Styles
{
    public const string SectionName = "V4+ Styles";

    public required IReadOnlyList<string> Format { get; set; }

    public required IList<Style> StylesList { get; set; }
}

public struct Style
{
    public string Name;
    // ReSharper disable once IdentifierTypo
    public string Fontname;
    // ReSharper disable once IdentifierTypo
    public string Fontsize;
    public string PrimaryColour;
    public string SecondaryColour;
    public string OutlineColour;
    public string BackColour;
    public string Bold;
    public string Italic;
    public string Underline;
    public string StrikeOut;
    public string ScaleX;
    public string ScaleY;
    public string Spacing;
    public string Angle;
    public string BorderStyle;
    public string Outline;
    public string Shadow;
    public string Alignment;
    public string MarginL;
    public string MarginR;
    public string MarginV;
    public string Encoding;
    public int LineNumber;
}

public class Events
{
    public const string SectionName = "Events";

    public required IReadOnlyList<string> Format { get; set; }

    public required IList<Event> EventsList { get; set; }
}

public struct Event
{
    public EventType Type;
    public string Layer;
    public string Start;
    public string End;
    public string Style;
    public string Name;
    public string MarginL;
    public string MarginR;
    public string MarginV;
    public string Effect;
    public string Text;
    public int LineNumber;
}

public enum EventType
{
    Comment,
    Dialogue
}

public class FontDetail : IEquatable<FontDetail?>
{
    public string FontName { get; init; } = "";

    public int Bold { get; init; }

    public bool IsItalic { get; init; }

    public string UsedChar { get; set; } = "";

    public override bool Equals(object? obj)
    {
        return Equals(obj as FontDetail);
    }

    public bool Equals(FontDetail? other)
    {
        return other is not null
               && FontName == other.FontName
               && Bold == other.Bold
               && IsItalic == other.IsItalic;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FontName, Bold, IsItalic);
    }

    public static bool operator ==(FontDetail? left, FontDetail? right) => EqualityComparer<FontDetail>.Default.Equals(left, right);

    public static bool operator !=(FontDetail? left, FontDetail? right) => !(left == right);
}
