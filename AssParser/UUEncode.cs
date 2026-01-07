using System;
using System.Buffers;

namespace AssParser;

public class UUEncode
{
    private static ReadOnlySpan<byte> EncLut => "!\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`"u8;

    internal const byte Cr = (byte) '\r';
    internal const byte Lf = (byte) '\n';

    /// <summary>
    /// Use UUEncode to encode byte[] data. Despite being called uuencoding by ass_specs.doc, the format is actually somewhat different from real uuencoding.
    /// Please refer to https://github.com/Aegisub/Aegisub/blob/6f546951b4f004da16ce19ba638bf3eedefb9f31/libaegisub/ass/uuencode.cpp for more information.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="insertBr">Whether break the line after 80 characters.</param>
    /// <param name="crlf">The linebreak type of source string. True if is CRLF.</param>
    /// <returns>UUEncoded string.</returns>
    public static ReadOnlySpan<byte> Encode(ReadOnlySpan<byte> data, bool insertBr = true, bool crlf = true)
    {
        var written = 0;
        var curr = 0;
        var resLength = (data.Length / 3 * 4) + (data.Length % 3 is 0 ? 0 : (data.Length % 3) + 1);
        if (insertBr)
            resLength += ((resLength / 80) - (resLength % 80 is 0 ? 1 : 0)) * (crlf ? 2 : 1);
        var res = new byte[resLength];
        Span<byte> dst = stackalloc byte[4];
        var length = data.Length;
        for (var pos = 0; pos < length; pos += 3)
        {
            var numBytesRemain = Math.Min(length - pos, 3);

            dst[0] = EncLut[(data[pos] >> 2) & 0x3f];
            switch (numBytesRemain)
            {
                case 1:
                    dst[1] = EncLut[(data[pos + 0] << 4) & 0x3f];
                    break;
                case 2:
                    dst[1] = EncLut[((data[pos + 0] << 4) & 0x3f) | ((data[pos + 1] >> 4) & 0x0f)];
                    dst[2] = EncLut[(data[pos + 1] << 2) & 0x3f];
                    break;
                case 3:
                    dst[1] = EncLut[((data[pos + 0] << 4) & 0x3f) | ((data[pos + 1] >> 4) & 0x0f)];
                    dst[2] = EncLut[((data[pos + 1] << 2) & 0x3f) | ((data[pos + 2] >> 6) & 0x03)];
                    dst[3] = EncLut[(data[pos + 2] << 0) & 0x3f];
                    break;
            }
            for (var i = 0; i < numBytesRemain + 1; i++)
            {
                res[curr] = dst[i];
                curr++;
                written++;
                if (insertBr && written is 80 && numBytesRemain is 3)
                {
                    if (crlf)
                    {
                        res[curr] = Cr;
                        curr++;
                    }
                    res[curr] = Lf;
                    curr++;
                    written = 0;
                }
            }
        }
        return res;
    }

    /// <summary>
    /// Use UUEncode to decode byte[] data. Despite being called uuencoding by ass_specs.doc, the format is actually somewhat different from real uuencoding.
    /// Please refer to https://github.com/Aegisub/Aegisub/blob/6f546951b4f004da16ce19ba638bf3eedefb9f31/libaegisub/ass/uuencode.cpp for more information.
    /// </summary>
    /// <param name="byteData">UUEncoded string.</param>
    /// <param name="crLf">The linebreak type of source string. True if is CRLF.</param>
    /// <returns>UUDecoded byte[].</returns>
    public static ReadOnlySpan<byte> Decode(ReadOnlySpan<byte> byteData, out bool crLf)
    {
        crLf = false;
        var length = byteData.Length;
        Span<byte> src = stackalloc byte[4];
        var writer = new ArrayBufferWriter<byte>(length * 3 / 4);
        for (var pos = 0; pos + 1 < length;)
        {
            var numBytesRemain = Math.Min(length - pos, 4);
            var bytes = 0;
            for (var i = 0; i < numBytesRemain; ++pos)
            {
                var c = byteData[pos];
                if (c is not Lf and not Cr)
                {
                    src[i] = (byte) (c - 33);
                    i++;
                    bytes++;
                }
                else if (c is Cr)
                    crLf = true;
            }

            if (bytes > 1)
            {
                var span = writer.GetSpan(1);
                span[0] = (byte) (((src[0] << 2) & 0xff) | ((src[1] >> 4) & 0x03));
                writer.Advance(1);
            }

            if (bytes > 2)
            {
                var span = writer.GetSpan(1);
                span[0] = (byte) (((src[1] << 4) & 0xff) | ((src[2] >> 2) & 0x0f));
                writer.Advance(1);
            }

            if (bytes > 3)
            {
                var span = writer.GetSpan(1);
                span[0] = (byte) (((src[2] << 6) & 0xff) | ((src[3] >> 0) & 0x3f));
                writer.Advance(1);
            }
        }

        return writer.WrittenSpan;
    }
}
