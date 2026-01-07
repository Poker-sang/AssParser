using System.Collections.Generic;

namespace AssParser;

/// <summary>
/// [Script Info] section
/// </summary>
public class ScriptInfo
{
    /// <summary>
    /// This is a comment used in the script file only. It is not visible when you load the script into SSA.
    /// </summary>
    /// <remarks>
    /// Starts with a semicolon ( ; ) or an exclamation line ( !: ).
    /// </remarks>
    public List<string> Comments { get; set; } = [];

    /// <summary>
    /// This is a description of the script.
    /// </summary>
    public string Title { get; set; } = "";

    /// <summary>
    /// The original author(s) of the script
    /// </summary>
    public string OriginalScript { get; set; } = "";

    /// <summary>
    /// (optional) The original translator of the dialogue
    /// </summary>
    public string? OriginalTranslation { get; set; }

    /// <summary>
    /// (optional) The original script editor(s), typically whoever took the raw translation and turned it into idiomatic english and reworded for readability.
    /// </summary>
    public string? OriginalEditing { get; set; }

    /// <summary>
    /// (optional) Whoever timed the original script
    /// </summary>
    public string? OriginalTiming { get; set; }

    /// <summary>
    /// (optional) Description of where in the video the script should begin playback.
    /// </summary>
    public string? SynchPoint { get; set; }

    /// <summary>
    /// (optional) Names of any other subtitling groups who edited the original script.
    /// </summary>
    public string? ScriptUpdatedBy { get; set; }

    /// <summary>
    /// The details of any updates to the original script made by other subtitling groups.
    /// </summary>
    public string? UpdateDetails { get; set; }

    /// <summary>
    /// This is the SSA script format version e.g. "V4.00". ASS version is “V4.00+”.
    /// </summary>
    public string ScriptType { get; set; } = "";

    /// <summary>
    /// This determines how subtitles are moved, when preventing onscreen collisions.
    /// <list type="bullet">
    ///     <item>
    ///         <term>Normal</term>
    ///         <description>SSA will attempt to position subtitles in the position specified by the "margins". However, subtitles can be shifted vertically to prevent onscreen collisions. With "normal" collision prevention, the subtitles will "stack up" one above the other - but they will always be positioned as close the vertical (bottom) margin as possible - filling in "gaps" in other subtitles if one large enough is available.</description>
    ///     </item>
    ///     <item>
    ///         <term>Reverse</term>
    ///         <description>subtitles will be shifted upwards to make room for subsequent overlapping subtitles. This means the subtitles can nearly always be read top-down - but it also means that the first subtitle can appear halfway up the screen before the subsequent overlapping subtitles appear. It can use a lot of screen area.</description>
    ///     </item>
    /// </list>
    /// </summary>
    public string Collisions { get; set; } = "";

    /// <summary>
    /// This is the width of the screen used by the authors when playing the script.
    /// </summary>
    public int PlayResX { get; set; }

    /// <summary>
    /// This is the height of the screen used by the authors when playing the script.
    /// </summary>
    public int PlayResY { get; set; }

    /// <summary>
    /// This is the colour depth used by the authors when playing the script.
    /// </summary>
    public int PlayDepth { get; set; }

    /// <summary>
    /// This is the Timer Speed for the script, as a percentage. e.g. "100.0000" is exactly 100%. It has four digits following the decimal point.
    /// The timer speed is a time multiplier applied to SSA's clock to stretch or compress the duration of a script. A speed greater than 100% will reduce the overall duration, and means that subtitles will progressively appear sooner and sooner. A speed less than 100% will increase the overall duration of the script means subtitles will progressively appear later and later (like a positive ramp time).<br/>
    /// The stretching or compressing only occurs during script playback - this value does not change the actual timings for each event listed in the script.<br/>
    /// Check the SSA user guide if you want to know why "Timer Speed" is more powerful than "Ramp Time", even though they both achieve the same result.
    /// </summary>
    public float Timer { get; set; }

    /// <summary>
    /// Defines the default wrapping style.
    /// </summary>
    public WrapStyle WrapStyle { get; set; }

    /// <summary>
    /// If true, script resolution is used.<br/>
    /// If false, video resolution is used.<br/>
    /// </summary>
    /// <remarks>
    /// Scaling them relative to the script resolution should always be enabled.
    /// </remarks>
    public bool ScaledBorderAndShadow { get; set; }

    /// <inheritdoc cref="AssParser.YCbCrMatrix" />
    public YCbCrMatrix YCbCrMatrix { get; set; } = YCbCrMatrix.None;
}
