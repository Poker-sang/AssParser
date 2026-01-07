namespace AssParser.Test.AssParserExtTest;

[TestClass]
public class AssParserExtTest
{
    [DataRow("FontsTest.ass", "FontsTest.txt")]
    [TestMethod]
    public async Task UsedFonts_ShouldBe_EquivalentAsync(string ass, string txt)
    {
        var truth = await File.ReadAllLinesAsync(Path.Combine(nameof(AssParserExtTest), txt));
        var sortedTruth = truth.Select(line => line.Split('\t'))
            .Select(parts => new FontDetail
            {
                FontName = parts[0],
                Bold = int.Parse(parts[1]),
                IsItalic = bool.Parse(parts[2]),
                UsedChar = SortString(parts[3])
            })
            .ToArray();

        var assFile = await AssSubtitleParser.ParseFileAsync(Path.Combine(nameof(AssParserExtTest), ass));
        var fonts = assFile.UsedFonts();

        foreach (var fontDetail in fonts)
            fontDetail.UsedChar = SortString(fontDetail.UsedChar);

        CollectionAssert.AreEquivalent(sortedTruth, fonts, EqualityComparer<FontDetail>.Create(
            (x, y) => x is not null && y is not null && x == y && x.UsedChar == y.UsedChar,
            obj => HashCode.Combine(obj.GetHashCode(), obj.UsedChar)));
    }

    private static string SortString(string s)
    {
        var sa = s.ToArray();
        Array.Sort(sa);
        return new(sa);
    }
}
