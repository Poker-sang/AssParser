using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AssParser;

/// <summary>
/// Represents an Advanced SubStation Alpha (ASS) or SubStation Alpha (SSA) subtitle file.
/// </summary>
public partial class AssSubtitleModel
{
    /// <inheritdoc cref="AssParser.ScriptInfo" />
    public required ScriptInfo ScriptInfo { get; set; }

    /// <inheritdoc cref="AssParser.Styles" />
    public required Styles Styles { get; set; }

    /// <inheritdoc cref="AssParser.Events" />
    public required Events Events { get; set; }

    /// <summary>
    /// All sections' name
    /// </summary>
    public required List<string> Ord { get; set; }

    /// <summary>
    /// [Fonts] / [Graphics] / [Aegisub Extradata] / [Aegisub Project Garbage] and any other sections
    /// </summary>
    public required Dictionary<string, string> ExtraSections { get; set; }

    /// <summary>
    /// Asynchronously retrieves the content as a string.
    /// </summary>
    public async Task<string> GetStringAsync()
    {
        await using var stream = new MemoryStream();
        await WriteToStreamAsync(stream);
        stream.Position = 0;
        using StreamReader reader = new(stream, leaveOpen: true);
        return await reader.ReadToEndAsync();
    }
}
