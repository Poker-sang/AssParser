# AssParser &middot; [![Publish to nuget](https://github.com/AmusementClub/AssParser/actions/workflows/dotnet-nuget.yml/badge.svg)](https://github.com/AmusementClub/AssParser/actions/workflows/dotnet-nuget.yml) ![Nuget](https://img.shields.io/nuget/v/AssParser?logo=nuget)  [![Test](https://github.com/AmusementClub/AssParser/actions/workflows/test.yml/badge.svg)](https://github.com/AmusementClub/AssParser/actions/workflows/test.yml)

Parse ASS (SubStation Alpha Subtitles) file faster. No Regex. All managed code.

## Basic Parse

```cs
AssSubtitleModel assfile = await Lib.AssParser.ParseFileAsync(@"path/to/your/assfile");
```

## List used fonted

```cs
AssSubtitleModel assfile = await Lib.AssParser.ParseFileAsync(@"path/to/your/assfile");
IReadOnlyList<FontDetail> fonts = assfile.UsedFonts();
```

Where `FontDetail` is defined as

```cs
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

```

## Get extra section

```cs
AssSubtitleModel assfile = Lib.AssParser.ParseAssFile(Path.Combine("UUEncodeTest", "1.ass")).Result;
string fontsData = assfile.UnknownSections["Fonts"];
```

## Decode & Encode UUEncode

```cs
ReadOnlySpan<byte> data = UUEncode.Decode(fontsData, out var crlf);
ReadOnlySpan<byte> encoded = UUEncode.Eecode(data, crlf)
```
