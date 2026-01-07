namespace AssParser;

/// <summary>
/// 
/// </summary>
public enum EventType
{
    /// <summary>
    /// This is a Dialogue event, i.e. Some text to display.
    /// </summary>
    Dialogue,

    /// <summary>
    /// This is a "comment" event.
    /// This contains the same information as a Dialogue, Picture, Sound, Movie, or Command event, but it is ignored during script playback.
    /// </summary>
    Comment
}
