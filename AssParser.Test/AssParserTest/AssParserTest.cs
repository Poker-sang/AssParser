using AssParser.Lib;

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
        var path = Path.Combine("AssParserTest", file);

        try
        {
            _ = await Lib.AssParser.ParseFileAsync(path);
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
        var path = Path.Combine("AssParserTest", file);
        var source = await File.ReadAllTextAsync(path);
        var assFile = await Lib.AssParser.ParseFileAsync(path);

        // Act
        var res = await assFile.GetStringAsync();

        // Assert
        Assert.AreEqual(source.ReplaceLineEndings("\r\n"), res.ReplaceLineEndings("\r\n"));
    }

    [TestMethod]
    public async Task AssParser_ShouldThrow_InvalidStyleAsync()
    {
        // Arrange
        var path = Path.Combine("AssParserTest", "format_14.ass");

        // Act & Assert
        var exception = await Assert.ThrowsExceptionAsync<AssParserException>(
            async () => await Lib.AssParser.ParseFileAsync(path));
        
        Assert.AreEqual(14, exception.LineCount);
        Assert.AreEqual(AssParserException.AssParserErrorType.InvalidStyleLine, exception.ErrorType);
    }
}
