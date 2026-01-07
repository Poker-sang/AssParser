using System;

namespace AssParser;

/// <summary>
/// Specifies the strictness levels while parsing or writing.
/// </summary>
[Flags]
public enum StrictnessLevel
{
    Strict,
    AllowUnknownSections = 0b1,
    AllowInvalidSections = 0b10,
    AllowInvalidLines = 0b100,
    None = AllowUnknownSections | AllowInvalidSections | AllowInvalidLines
}
