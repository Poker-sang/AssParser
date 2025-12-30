using AssParser.Lib;

namespace AssParser;

public static class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var assFile = await Lib.AssParser.ParseFileAsync(ParserBenchmark.TestFileName);
            var fonts = assFile.UsedFonts();
            foreach (var font in fonts)
            {
                Console.WriteLine(font.FontName + "\t" + font.UsedChar);
            }
            var txt = await assFile.GetStringAsync();
        }
        catch (AggregateException ae)
        {
            foreach (var ex in ae.InnerExceptions)
            {
                // Handle the custom exception.
                if (ex is AssParserException assException)
                {
                    Console.WriteLine(assException?.ToString());
                }
                // Rethrow any other exception.
                else
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
#if !DEBUG
        var summary = BenchmarkDotNet.Running.BenchmarkRunner.Run<ParserBenchmark>();
#endif
        Console.ReadLine();
    }
}
