namespace AssParser.Test.AssParserTest;

[TestClass]
public class AssParserTest
{
    [TestMethod]
    [DataRow("1.ass")]
    [DataRow("2.ass")]
    public async Task AssParser_ShouldNot_ThrowAsync(string file)
    {
        // Arrange
        var path = Path.Combine(nameof(AssParserTest), file);

        try
        {
            _ = await AssSubtitleParser.ParseFileAsync(path);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
    }

    [TestMethod]
    [DataRow("1.ass")]
    [DataRow("2.ass")]
    public async Task ToString_ShouldBe_SameAsync(string file)
    {
        // Arrange
        var path = Path.Combine(nameof(AssParserTest), file);
        var source = (await File.ReadAllTextAsync(path)).ReplaceLineEndings("\n").Split('\n');
        var assFile = await AssSubtitleParser.ParseFileAsync(path);

        // Act
        var res = (await assFile.GetStringAsync()).ReplaceLineEndings("\n").Split('\n');

        // Assert
        CollectionAssert.IsSubsetOf(source, res);
    }

    [TestMethod]
    [DataRow("format_14.ass")]
    public async Task AssParser_ShouldThrow_InvalidStyleAsync(string file)
    {
        // Arrange
        var path = Path.Combine(nameof(AssParserTest), file);

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<AssSubtitleParserException>(
            async () => await AssSubtitleParser.ParseFileAsync(path));
        
        Assert.AreEqual(14, exception.LineCount);
        Assert.AreEqual(AssSubtitleParserException.ErrorType.InvalidStyleLine, exception.Type);
    }
}
