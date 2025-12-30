using AssParser.Lib;

namespace AssParser.Test.AssParserExtTest;

[TestClass]
public class AssParserExtTest
{
    [TestMethod]
    public async Task UsedFonts_ShouldBe_EquivalentAsync()
    {
        var truth = await File.ReadAllLinesAsync(Path.Combine("AssParserExtTest", "FontsTest.txt"));
        var sortedTruth = new List<string>();
        foreach (var line in truth)
        {
            var parts = line.Split('\t');
            parts[3] = SortString(parts[3]);
            sortedTruth.Add(string.Join('\t', parts));
        }
        var assFile = await Lib.AssParser.ParseFileAsync(Path.Combine("AssParserExtTest", "FontsTest.ass"));
        var fonts = assFile.UsedFonts();
        var res = fonts.Select(font => font.FontName + "\t" + font.Bold + "\t" + font.IsItalic + "\t" + SortString(font.UsedChar));
        CollectionAssert.AreEquivalent(sortedTruth, res.ToList());
    }

    private static string SortString(string s)
    {
        var sa = s.ToArray();
        Array.Sort(sa);
        return new(sa);
    }
}
