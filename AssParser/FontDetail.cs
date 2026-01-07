using System;
using System.Collections.Generic;

namespace AssParser;

/// <summary>
/// Font detail used in [Fonts] section
/// </summary>
public class FontDetail : IEquatable<FontDetail?>
{
    public string FontName { get; init; } = "";

    public int Bold { get; init; }

    public bool IsItalic { get; init; }

    public IReadOnlyList<char> UsedChar { get; set; } = [];

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as FontDetail);

    /// <inheritdoc/>
    public bool Equals(FontDetail? other)
    {
        return other is not null
               && FontName == other.FontName
               && Bold == other.Bold
               && IsItalic == other.IsItalic;
    }

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(FontName, Bold, IsItalic);

    /// <inheritdoc cref="Equals(FontDetail?)"/>
    public static bool operator ==(FontDetail? left, FontDetail? right) => EqualityComparer<FontDetail>.Default.Equals(left, right);

    /// <summary>
    /// 
    /// </summary>
    public static bool operator !=(FontDetail? left, FontDetail? right) => !(left == right);
}
