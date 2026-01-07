using System.Drawing;

namespace AssParser;

/// <summary>
/// This is a Style definition, used to format text displayed by the script.
/// </summary>
public struct Style
{
    /// <summary>
    /// The name of the Style. Case-sensitive. Cannot include commas.
    /// </summary>
    public string Name;

    /// <summary>
    /// The font name as used by Windows. Case-sensitive.
    /// </summary>
    public string Fontname;

    /// <summary>
    /// 
    /// </summary>
    public int Fontsize;

    /// <summary>
    /// 
    /// </summary>
    public Color PrimaryColour;

    /// <summary>
    /// This colour may be used instead of the <see cref="PrimaryColour"/> when a subtitle is automatically shifted to prevent an onscreen collision, to distinguish the different subtitles.
    /// </summary>
    public Color SecondaryColour;

    /// <summary>
    /// (AKA. TertiaryColour, deprecated name) This colour may be used instead of the  <see cref="PrimaryColour"/> or <see cref="SecondaryColour"/> when a subtitle is automatically shifted to prevent an onscreen collision, to distinguish the different subtitles.
    /// </summary>
    public Color OutlineColour;

    /// <summary>
    /// This is the colour of the subtitle outline or shadow, if these are used.
    /// </summary>
    public Color BackColour;

    /// <summary>
    /// This defines whether text is bold (true) or not (false).
    /// </summary>
    public bool Bold;

    /// <summary>
    /// This defines whether text is italic (true) or not (false).
    /// </summary>
    public bool Italic;

    /// <remarks>
    /// ASS only.
    /// </remarks>
    public bool Underline;

    /// <remarks>
    /// ASS only.
    /// </remarks>
    public bool StrikeOut;

    /// <remarks>
    /// ASS only. Modifies the width of the font. [percent]
    /// </remarks>
    public int ScaleX;

    /// <remarks>
    /// ASS only. Modifies the height of the font. [percent]
    /// </remarks>
    public int ScaleY;

    /// <summary>
    /// ASS only. Extra space between characters. [pixels]
    /// </summary>
    public int Spacing;

    /// <summary>
    /// ASS only. The origin of the rotation is defined by the alignment. Can be a floating point number. [degrees]
    /// </summary>
    public float Angle;

    /// <inheritdoc cref="AssParser.BorderStyle" />
    public BorderStyle BorderStyle;

    /// <summary>
    /// If <see cref="BorderStyle"/> is <see cref="BorderStyle.OutlineDropShadow"/>,  then this specifies the width of the outline around the text, in pixels. Values may be 0, 1, 2, 3 or 4.
    /// </summary>
    public float Outline;

    /// <summary>
    /// If <see cref="BorderStyle"/> is <see cref="BorderStyle.OutlineDropShadow"/>, then this specifies the depth of the drop shadow behind the text, in pixels. Values may be 0, 1, 2, 3 or 4. Drop shadow is always used in addition to an outline.
    /// </summary>
    public float Shadow;

    /// <summary>
    /// This sets how text is "justified" within the Left/Right onscreen margins, and also the vertical placing.
    /// </summary>
    public Alignment Alignment;

    /// <summary>
    /// This defines the Left Margin in pixels. It is the distance from the left-hand edge of the screen.
    /// </summary>
    public int MarginL;

    /// <summary>
    /// This defines the Right Margin in pixels. It is the distance from the right-hand edge of the screen.
    /// </summary>
    public int MarginR;

    /// <summary>
    /// This defines the vertical Left Margin in pixels.
    /// <list type="bullet">
    ///     <item>
    ///         <term>subtitle</term>
    ///         <description>it is the distance from the bottom of the screen.</description>
    ///     </item>
    ///     <item>
    ///         <term>toptitle</term>
    ///         <description>it is the distance from the top of the screen.</description>
    ///     </item>
    ///     <item>
    ///         <term>midtitle</term>
    ///         <description>the value is ignored - the text will be vertically centred</description>
    ///     </item>
    /// </list>
    /// </summary>
    public int MarginV;

    /// <remarks>
    /// SSA only. This defines the transparency of the text. SSA does not use this yet.
    /// </remarks>
    public int AlphaLevel;

    /// <remarks>
    /// This specifies the font character set or encoding and on multilingual Windows installations it provides access to characters used in multiple than one language. It is usually 0 (zero) for English (Western, ANSI) Windows.
    /// </remarks>
    public int Encoding;
}
