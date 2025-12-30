using System.Text;
using AssParser.Lib;

namespace AssParser.Test.UUEncodeTest;
    
[TestClass]
public class UuEncodeTest
{
    private byte[] _fontsDataCrlf = null!;
    private byte[] _fontsDataLf = null!;

    [TestInitialize]
    public async Task InitializeAsync()
    {
        var assFile = await Lib.AssParser.ParseFileAsync(Path.Combine("UUEncodeTest", "1.ass"));
        var fontsData = assFile.UnknownSections[AssSubtitleModel.FontsSection];
        fontsData = fontsData[(fontsData.IndexOf('\n') + 1)..].Trim();

        _fontsDataCrlf = Encoding.UTF8.GetBytes(fontsData.ReplaceLineEndings("\r\n"));
        _fontsDataLf = Encoding.UTF8.GetBytes(fontsData.ReplaceLineEndings("\n"));
    }

    [TestMethod]
    public void UUDecode_ShouldBe_Same()
    {
        var ttf = File.ReadAllBytes(Path.Combine("UUEncodeTest", "FreeSans.ttf"));
        var data1 = UUEncode.Decode(_fontsDataCrlf, out _);
        CollectionAssert.AreEqual(ttf, data1.ToArray());
    }

    [TestMethod]
    public void UUEncode_ShouldBe_Same_Crlf()
    {
        var data1 = UUEncode.Decode(_fontsDataCrlf, out var crlf);
        var encoded = UUEncode.Encode(data1, true, crlf);
        CollectionAssert.AreEqual(_fontsDataCrlf, encoded.ToArray());
    }

    [TestMethod]
    public void UUEncode_ShouldBe_Same_Lf()
    {
        var data1 = UUEncode.Decode(_fontsDataLf, out var crlf);
        var encoded = UUEncode.Encode(data1, true, crlf);
        CollectionAssert.AreEqual(_fontsDataLf, encoded.ToArray());
    }

    [TestMethod]
    public void UUEncode_Encode_Coverage_Test()
    {
        CollectionAssert.AreEqual("-1"u8.ToArray(), UUEncode.Encode("1"u8).ToArray());
        CollectionAssert.AreEqual("-4%"u8.ToArray(), UUEncode.Encode("11"u8).ToArray());
        CollectionAssert.AreEqual("-4%R"u8.ToArray(), UUEncode.Encode("111"u8).ToArray());
    }
}
