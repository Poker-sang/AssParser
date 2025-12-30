using System;

namespace AssParser.Lib;

public class AssParserException(
    int lineCount,
    AssParserException.AssParserErrorType errorType,
    string? message = null,
    Exception? inner = null)
    : Exception($"[{lineCount}] {errorType}: {message}", inner)
{
    public readonly int LineCount = lineCount;
    public readonly AssParserErrorType ErrorType = errorType;

    /// <summary>
    /// Print readable exception in English.
    /// </summary>
    /// <returns>Exception message and line content.</returns>
    public override string ToString() => Message;

    public enum AssParserErrorType
    {
        UnknownError,
        InvalidSection,
        MissingSection,
        MissingFormatLine,
        InvalidStyleLine,
        InvalidEventLine
    }
}
