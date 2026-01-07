using System;

namespace AssParser;

/// <summary>
/// This is an Event definition, which is either a <see cref="EventType.Dialogue"/> or a <see cref="EventType.Comment"/>.
/// </summary>
public struct Event
{
    /// <inheritdoc cref="EventType" />
    public EventType Type;

    /// <summary>
    /// Subtitles having different layer number will be ignored during the collusion detection.
    /// </summary>
    public int Layer;

    /// <summary>
    /// Start Time of the Event. This is the time elapsed during script playback at which the text will appear onscreen. Note that there is a single digit for the hours!
    /// </summary>
    public TimeSpan Start;

    /// <summary>
    /// End Time of the Event. This is the time elapsed during script playback at which the text will disappear offscreen. Note that there is a single digit for the hours!
    /// </summary>
    public TimeSpan End;

    /// <summary>
    /// Style name. If it is "Default", then your own *Default style will be substituted.
    /// </summary>
    public string Style;

    /// <summary>
    /// Character name. This is the name of the character who speaks the dialogue. It is for information only, to make the script is easier to follow when editing/timing.
    /// </summary>
    public string Name;

    /// <summary>
    /// 4-figure Left Margin override. The values are in pixels. All zeroes mean the default margins defined by the style are used.
    /// </summary>
    public int MarginL;

    /// <summary>
    /// 4-figure Right Margin override. The values are in pixels. All zeroes mean the default margins defined by the style are used.
    /// </summary>
    public int MarginR;

    /// <summary>
    /// 4-figure Bottom Margin override. The values are in pixels. All zeroes mean the default margins defined by the style are used.
    /// </summary>
    public int MarginV;

    /// <summary>
    /// Transition Effect. This is either empty, or contains information for one of the three transition effects implemented in SSA v4.x
    /// The effect names are case-sensitive and must appear exactly as shown.The effect names do not have quote marks around them.
    /// <list type="bullet">
    ///     <item>
    ///         <term>Karaoke</term>
    ///         <description>
    ///             the text will be successively highlighted one word at a time.<br/>
    ///             <b>**Karaoke as an effect type is obsolete.**</b>
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>Scroll up;y1;y2;delay[;fadeawayheight]</term>
    ///         <description>
    ///             the text/picture will scroll up the screen. The parameters after the words "Scroll up" are separated by semicolons.<br/>
    ///             The y1 and y2 values define a vertical region on the screen in which the text will scroll. The values are in pixels, and it doesn't matter which value (top or bottom) comes first.<br/>
    ///             If the values are zeroes then the text will scroll up the full height of the screen.<br/>
    ///             The delay value can be a number from 1 to 100, and it slows down the speed of the scrolling - zero means no delay and the scrolling will be as fast as possible.
    ///         </description>
    ///     </item>
    ///     <item>
    ///         <term>Banner;delay</term>
    ///         <description>
    ///             text will be forced into a single line, regardless of length, and scrolled from right to left across the screen.<br/>
    ///             The delay value can be a number from 1 to 100, and it slows down the speed of the scrolling - zero means no delay and the scrolling will be as fast as possible.
    ///         </description>
    ///     </item>
    /// </list>
    /// </summary>
    public string Effect;

    /// <summary>
    /// Subtitle Text. This is the actual text which will be displayed as a subtitle onscreen. Everything after the 9th comma is treated as the subtitle text, so it can include commas.
    /// The text can include \n codes which is a line break, and can include Style Override control codes, which appear between braces { }.
    /// </summary>
    public string Text;
}
