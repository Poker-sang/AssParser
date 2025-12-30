using AssParser.Lib;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace AssParser;

[SimpleJob(RuntimeMoniker.Net10_0, baseline: true)]
[SimpleJob(RuntimeMoniker.NativeAot10_0)]
[MemoryDiagnoser]
[RPlotExporter]
public class ParserBenchmark
{
    private AssSubtitleModel _assFile = null!;
    private FileStream _assStream = null!;
    public const string TestFileName = "[Nekomoe kissaten&VCB-Studio] Cider no You ni Kotoba ga Wakiagaru [Ma10p_1080p][x265_flac].jp&sc.ass";

    [GlobalSetup]
    public async Task SetupAsync()
    {
        _assStream = new FileStream(TestFileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
        _assFile = await ParseFileAsync();
    }

    [Benchmark]
    public async Task<AssSubtitleModel> ParseFileAsync()
    {
        _assStream.Position = 0;
        return await Lib.AssParser.ParseAsync(_assStream);
    }

    [Benchmark]
    public IReadOnlyList<FontDetail> ExtractUsedFonts()
    {
        return _assFile.UsedFonts();
    }
}
