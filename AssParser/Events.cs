using System.Collections.Generic;

namespace AssParser;

/// <summary>
/// [Events] section
/// </summary>
public class Events : List<Event>
{
    /// <summary>
    /// The format line specifies how SSA will interpret all following Event lines.
    /// </summary>
    public required IReadOnlyList<string> Format { get; set; }

    /// <summary>
    /// <see cref="OtherEventType"/> lines
    /// </summary>
    public List<(OtherEventType Type, string Content)> OtherEvents { get; set; } = [];
}
