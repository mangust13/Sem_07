using Microsoft.AspNetCore.Mvc;
using Lab_02.Services;

namespace Lab_02.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ComputeTextHash(string Text)
    {
        ViewBag.ActiveTab = "text";

        //if (string.IsNullOrWhiteSpace(Text))
        //{
        //    ViewBag.TextError = "������ �����.";
        //    return View("Index");
        //}

        ViewBag.TextInput = Text;
        ViewBag.TextHash = Md5Utility.ComputeHexFromString(Text);
        return View("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ComputeFileHash(IFormFile File, CancellationToken ct)
    {
        ViewBag.ActiveTab = "file";

        if (File is null || File.Length == 0)
        {
            ViewBag.FileError = "������ ��������� ����.";
            return View("Index");
        }

        await using var stream = File.OpenReadStream();
        ViewBag.FileHash = await Md5Utility.ComputeHexFromStreamAsync(stream, ct);
        return View("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyFileIntegrity(IFormFile File, IFormFile Md5File, CancellationToken ct)
    {
        ViewBag.ActiveTab = "verify";

        if (File is null || File.Length == 0)
        {
            ViewBag.VerifyError = "������ ���� ��� ��������.";
            return View("Index");
        }
        if (Md5File is null || Md5File.Length == 0)
        {
            ViewBag.VerifyError = "������ .md5 ���� � ��������� �����.";
            return View("Index");
        }

        // 1) ������ MD5 ������ �����
        await using var fs = File.OpenReadStream();
        var calc = await Md5Utility.ComputeHexFromStreamAsync(fs, ct);

        // 2) ������ .md5 � ������ 32-hex
        using var reader = new StreamReader(Md5File.OpenReadStream());
        var md5Text = await reader.ReadToEndAsync();
        string referenceHex;
        try
        {
            referenceHex = Md5Utility.ExtractFirstHexFromMd5FileContent(md5Text);
        }
        catch (Exception ex)
        {
            ViewBag.VerifyError = ex.Message;
            return View("Index");
        }

        // 3) ���������
        ViewBag.CalcHash = calc;
        ViewBag.RefHash = referenceHex;
        ViewBag.VerifyMatch = string.Equals(calc, referenceHex, StringComparison.OrdinalIgnoreCase);

        return View("Index");
    }
}