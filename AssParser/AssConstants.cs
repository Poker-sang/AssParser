namespace AssParser;

/// <summary>
/// ASS/SSA constant strings
/// </summary>
public static class AssConstants
{
    /// <summary>
    /// [Aegisub Project Garbage] section
    /// </summary>
    public const string AegisubProjectGarbageSection = "Aegisub Project Garbage";

    /// <summary>
    /// [Aegisub Extradata] section
    /// </summary>
    public const string AegisubExtradataSection = "Aegisub Extradata";

    /// <summary>
    /// [Fonts] section
    /// </summary>
    public const string FontsSection = "Fonts";

    /// <summary>
    /// [Graphics] section
    /// </summary>
    public const string GraphicsSection = "Graphics";

    /// <summary>
    /// [Script Info] section
    /// </summary>
    public const string ScriptInfoSection = "Script Info";

    /// <summary>
    /// [V4 Styles] section
    /// </summary>
    public const string V4StylesSection = "V4 Styles";

    /// <summary>
    /// [V4+ Styles] section
    /// </summary>
    public const string V4PStylesSection = "V4+ Styles";

    /// <inheritdoc cref="Events" />
    public const string EventsSection = nameof(Events);

    /// <summary>
    /// [Format] line in Styles/Events section
    /// </summary>
    public const string FormatLine = "Format";

    /// <inheritdoc cref="Style" />
    public const string StyleLine = nameof(Style);

    /// <inheritdoc cref="EventType.Dialogue" />
    public const string DialogueEventLine = nameof(EventType.Dialogue);

    /// <inheritdoc cref="EventType.Comment" />
    public const string CommentEventLine = nameof(EventType.Comment);

    /// <inheritdoc cref="OtherEventType.Picture" />
    public const string PictureEventLine = nameof(OtherEventType.Picture);

    /// <inheritdoc cref="OtherEventType.Sound" />
    public const string SoundEventLine = nameof(OtherEventType.Sound);

    /// <inheritdoc cref="OtherEventType.Movie" />
    public const string MovieEventLine = nameof(OtherEventType.Movie);

    /// <inheritdoc cref="OtherEventType.Command" />
    public const string CommandEventLine = nameof(OtherEventType.Command);

    /// <inheritdoc cref="ScriptInfo.OriginalScript" />
    public const string OriginalScriptLine = "Original Script";

    /// <inheritdoc cref="ScriptInfo.OriginalTranslation" />
    public const string OriginalTranslationLine = "Original Translation";

    /// <inheritdoc cref="ScriptInfo.OriginalEditing" />
    public const string OriginalEditingLine = "Original Editing";

    /// <inheritdoc cref="ScriptInfo.OriginalTiming" />
    public const string OriginalTimingLine = "Original Timing";

    /// <inheritdoc cref="ScriptInfo.SynchPoint" />
    public const string SynchPointLine = "Synch Point";

    /// <inheritdoc cref="ScriptInfo.ScriptUpdatedBy" />
    public const string ScriptUpdatedByLine = "Script Updated By";

    /// <inheritdoc cref="ScriptInfo.UpdateDetails" />
    public const string UpdateDetailsLine = "Update Details";

    /// <inheritdoc cref="ScriptInfo.YCbCrMatrix" />
    public const string YCbCrMatrixLine = "YCbCr Matrix";
}
