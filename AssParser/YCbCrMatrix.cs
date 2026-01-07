using System;

namespace AssParser;

/// <summary>
/// Colors in ASS are specified as BGR values, but videos are normally stored in YCbCr, and there are several possible conversions between the two.
/// </summary>
/// <remarks>
/// You can ignore this entirely unless you are creating subtitles which need to exactly match a color in the video (such as if you are masking a portion of the screen with a vector drawing).
/// </remarks>
public readonly struct YCbCrMatrix()
{
    /// <summary>
    /// Create an instance of <see cref="YCbCrMatrix" />
    /// </summary>
    /// <param name="range"></param>
    /// <param name="colorSpace"></param>
    public YCbCrMatrix(YCbCrRange range, YCbCrColorSpace colorSpace) : this()
    {
        Range = range;
        ColorSpace = colorSpace;
        IsNone = false;
    }

    /// <summary>
    /// Represents value "None"
    /// </summary>
    public static readonly YCbCrMatrix None = new();

    public bool IsNone { get; } = true;

    /// <inheritdoc cref="YCbCrRange" />
    public YCbCrRange Range { get; }

    /// <inheritdoc cref="YCbCrColorSpace" />
    public YCbCrColorSpace ColorSpace { get; }

    /// <summary>
    /// Parses a string representation of a YCbCr matrix and returns the corresponding <see cref="YCbCrMatrix"/> instance.
    /// </summary>
    /// <exception cref="FormatException">Thrown if <paramref name="str"/> is not in a valid format or contains an unrecognized range or color space value.</exception>
    public static YCbCrMatrix Parse(string str)
    {
        if (str is nameof(None))
            return None;
        if (str.Split('.') is not [var rangeStr, var colorSpaceStr])
            throw new FormatException($"{str} is not a valid {nameof(YCbCrMatrix)} string.");
        var range = rangeStr switch
        {
            nameof(YCbCrRange.PC) => YCbCrRange.PC,
            nameof(YCbCrRange.TV) => YCbCrRange.TV,
            _ => throw new FormatException($"{rangeStr} is not a valid {nameof(YCbCrRange)} string.")
        };

        var colorSpace = colorSpaceStr switch
        {
            nameof(YCbCrColorSpace.BT601) or "601" => YCbCrColorSpace.BT601,
            nameof(YCbCrColorSpace.BT709) or "709" => YCbCrColorSpace.BT709,
            nameof(YCbCrColorSpace.SMPTE240M) or "240M" => YCbCrColorSpace.SMPTE240M,
            nameof(YCbCrColorSpace.FCC) => YCbCrColorSpace.FCC,
            _ => throw new FormatException($"{colorSpaceStr} is not a valid {nameof(YCbCrColorSpace)} string.")
        };

        return new(range, colorSpace);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return IsNone
            ? nameof(None)
            : $"{Range}.{ColorSpace switch {
                YCbCrColorSpace.BT601 => "601",
                YCbCrColorSpace.BT709 => "709",
                YCbCrColorSpace.SMPTE240M => "240M",
                _ => ColorSpace.ToString(),
            }}";
    }

    // ReSharper disable InconsistentNaming
    public enum YCbCrRange
    {
        PC,
        TV
    }

    public enum YCbCrColorSpace
    {
        BT601,
        BT709,
        SMPTE240M,
        FCC
    }
    // ReSharper restore InconsistentNaming
}
