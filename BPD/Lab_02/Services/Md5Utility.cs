using System.Text;
using System.Text.RegularExpressions;

namespace Lab_02.Services;

public static class Md5Utility
{
    public static string ComputeHexFromString(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text ?? string.Empty);
        var md5 = MyMd5.CalculateHash(bytes);
        return ToHex(md5);
    }

    public static async Task<string> ComputeHexFromStreamAsync(Stream stream, CancellationToken ct = default)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(stream, ct);
        var md5 = MyMd5.CalculateHash(ms.ToArray());
        return ToHex(md5);
    }

    public static string ExtractFirstHexFromMd5FileContent(string md5Text)
    {
        var m = Regex.Match(md5Text ?? "", "(?i)\\b[a-f0-9]{32}\\b");
        if (!m.Success)
            throw new InvalidDataException("У .md5-файлі не знайдено 32-символьний MD5 (hex).");
        return m.Value.ToUpperInvariant();
    }

    private static string ToHex(byte[] bytes) =>
        BitConverter.ToString(bytes).Replace("-", string.Empty).ToUpperInvariant();
}
