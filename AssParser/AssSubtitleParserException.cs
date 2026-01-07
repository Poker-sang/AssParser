using System;

namespace AssParser;

/// <summary>
/// Represents errors that occur during parsing of ASS subtitle files.
/// </summary>
/// <param name="lineCount">The line number in the subtitle file where the parsing error occurred.</param>
/// <param name="errorType">The specific type of parsing error encountered.</param>
/// <param name="message">An optional message that describes the error in more detail.</param>
/// <param name="innerException">The exception that is the cause of the current exception, or null if no inner exception is specified.</param>
public class AssSubtitleParserException(
    int lineCount,
    AssSubtitleParserException.ErrorType errorType,
    string? message = null,
    Exception? innerException = null)
    : Exception($"[{lineCount}] {errorType}: {message}", innerException)
{
    /// <summary>
    /// The line number in the subtitle file where the parsing error occurred.
    /// </summary>
    public readonly int LineCount = lineCount;

    /// <inheritdoc cref="AssParser.AssSubtitleParserException.ErrorType" />
    public readonly ErrorType Type = errorType;

    /// <inheritdoc />
    public override string ToString() => Message;

    /// <summary>
    /// Types of parsing errors.
    /// </summary>
    public enum ErrorType
    {
#pragma warning disable CS1591
        UnknownError,
        UnknownSection,
        InvalidSection,
        MissingSection,
        MissingFormatLine,
        InvalidStyleLine,
        InvalidEventLine
#pragma warning restore CS1591
    }
}
