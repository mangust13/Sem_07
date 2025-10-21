using Lab_03.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab_03.Controllers
{
    public class CryptoController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public CryptoController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Process(EncryptViewModel model)
        {
            if (model.File == null || string.IsNullOrEmpty(model.Password))
            {
                ViewBag.Error = "Please enter a password and select a file.";
                return View("Index");
            }

            string targetDir = Path.Combine(_env.ContentRootPath, "Files");
            Directory.CreateDirectory(targetDir);

            string inputPath = Path.Combine(targetDir, model.File.FileName);
            using (var stream = new FileStream(inputPath, FileMode.Create))
                model.File.CopyTo(stream);

            string outputFileName;

            if (model.Mode == "encrypt")
            {
                outputFileName = model.File.FileName + ".bin";
            }
            else
            {
                string originalName = model.File.FileName;
                if (originalName.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
                    originalName = originalName[..^4];
                outputFileName = originalName;
            }

            string outputPath = Path.Combine(targetDir, outputFileName);

            // Якщо файл уже існує — додаємо суфікс _copy або timestamp
            if (System.IO.File.Exists(outputPath))
            {
                string name = Path.GetFileNameWithoutExtension(outputFileName);
                string ext = Path.GetExtension(outputFileName);
                outputFileName = $"{name}_copy{ext}";
                outputPath = Path.Combine(targetDir, outputFileName);
            }

            try
            {
                var encryptor = new RC5Encryptor(model.Password);

                if (model.Mode == "encrypt")
                {
                    encryptor.EncryptFile(inputPath, outputPath);
                    ViewBag.Message = $"✅ File encrypted successfully and saved to: {outputPath}";
                }
                else
                {
                    encryptor.DecryptFile(inputPath, outputPath);
                    ViewBag.Message = $"✅ File decrypted successfully and saved to: {outputPath}";
                }
            }
            catch (InvalidOperationException)
            {
                ViewBag.Error = "❌ Unable to decrypt — the file is corrupted or already encrypted.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"⚠️ Unexpected error: {ex.Message}";
            }

            return View("Index");
        }
    }
}
