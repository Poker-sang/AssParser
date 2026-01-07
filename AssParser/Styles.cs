using System.Collections.Generic;

namespace AssParser;

/// <summary>
/// [V4 Styles] / [V4+ Styles] section
/// </summary>
public class Styles : List<Style>
{
    /// <summary>
    /// The format line specifies how SSA will interpret all following Style lines.
    /// </summary>
    public required IReadOnlyList<string> Format { get; set; }
}
