using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace DSSLab.Controllers;

public class SignatureController : Controller
{
    private readonly IWebHostEnvironment _env;
    public SignatureController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Sign(string inputText, IFormFile inputFile)
    {
        byte[] data;

        if (inputFile != null && inputFile.Length > 0)
        {
            using var ms = new MemoryStream();
            await inputFile.CopyToAsync(ms);
            data = ms.ToArray();
        }
        else
        {
            data = Encoding.UTF8.GetBytes(inputText ?? "");
        }


        using var dsa = DSA.Create(2048);
        var privateKey = dsa.ExportPkcs8PrivateKey();
        var publicKey = dsa.ExportSubjectPublicKeyInfo();

        byte[] hash = SHA256.HashData(data);
        byte[] signature = dsa.CreateSignature(hash);

        ViewBag.SignatureHex = BitConverter.ToString(signature).Replace("-", "");
        ViewBag.PublicKey = Convert.ToBase64String(publicKey);
        ViewBag.PrivateKey = Convert.ToBase64String(privateKey);

        var projectRoot = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!.FullName;
        var filesPath = Path.Combine(projectRoot, "Files");
        Directory.CreateDirectory(filesPath);

        var baseName = inputFile != null ? Path.GetFileNameWithoutExtension(inputFile.FileName) : "input";

        var signaturePath = Path.Combine(filesPath, $"{baseName}_SIGNATURE.sig");
        var publicPath = Path.Combine(filesPath, $"{baseName}_PUBLIC.key");
        var privatePath = Path.Combine(filesPath, $"{baseName}_PRIVATE.key");

        await System.IO.File.WriteAllBytesAsync(signaturePath, signature);
        await System.IO.File.WriteAllBytesAsync(publicPath, publicKey);
        await System.IO.File.WriteAllBytesAsync(privatePath, privateKey);

        ViewBag.SavePath = filesPath;
        if (inputFile == null && !string.IsNullOrWhiteSpace(inputText))
        {
            var textPath = Path.Combine(filesPath, "input_TEXT.txt");
            await System.IO.File.WriteAllTextAsync(textPath, inputText);
        }


        return View("Index");
    }

    [HttpGet]
    public IActionResult Verify()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Verify(IFormFile fileToVerify, IFormFile signatureFile, IFormFile publicKeyFile)
    {
        if (fileToVerify == null || signatureFile == null || publicKeyFile == null)
        {
            ViewBag.Result = "⚠️ Please select all three files.";
            return View();
        }

        try
        {
            byte[] data, signature, publicKey;

            using (var ms = new MemoryStream())
            {
                await fileToVerify.CopyToAsync(ms);
                data = ms.ToArray();
            }
            using (var ms = new MemoryStream())
            {
                await signatureFile.CopyToAsync(ms);
                signature = ms.ToArray();
            }
            using (var ms = new MemoryStream())
            {
                await publicKeyFile.CopyToAsync(ms);
                publicKey = ms.ToArray();
            }

            using var dsa = DSA.Create();
            dsa.ImportSubjectPublicKeyInfo(publicKey, out _);

            byte[] hash = SHA256.HashData(data);
            bool verified = dsa.VerifySignature(hash, signature);

            ViewBag.Result = verified
                ? "✅ Signature is valid!"
                : "❌ Signature does NOT match!";
        }
        catch (CryptographicException)
        {
            ViewBag.Result = "❌ Invalid key file. Please make sure you selected the PUBLIC key (.key).";
        }
        catch (Exception ex)
        {
            ViewBag.Result = $"❌ Unexpected error: {ex.Message}";
        }

        return View();
    }

}
