namespace AssParser;

/// <summary>
/// 
/// </summary>
public enum OtherEventType
{
    /// <summary>
    /// This is a "picture" event, which means SSA will display the specified .bmp, .jpg, .gif, .ico or .wmf graphic.
    /// </summary>
    Picture,

    /// <summary>
    /// This is a "sound" event, which means SSA will play the specified .wav file.
    /// </summary>
    Sound,

    /// <summary>
    /// This is a "movie" event, which means SSA will play the specified .avi file.
    /// </summary>
    Movie,

    /// <summary>
    /// This is a "command" event, which means SSA will execute the specified program as a background task.
    /// </summary>
    Command
}
