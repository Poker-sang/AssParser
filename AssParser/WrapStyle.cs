namespace AssParser;

/// <summary>
/// Defines the default wrapping style.
/// </summary>
public enum WrapStyle
{
    /// <summary>
    /// Smart wrapping, lines are evenly broken.
    /// </summary>
    Smart,

    /// <summary>
    /// End-of-line word wrapping, only \N breaks
    /// </summary>
    EndOfLineWord,

    /// <summary>
    /// No word wrapping, \n \N both breaks
    /// </summary>
    NoWord,

    /// <summary>
    /// Same as <see cref="Smart"/>, but lower line gets wider.
    /// </summary>
    SmartLowerLineWider
}
