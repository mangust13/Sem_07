using Cipher.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cipher.Controllers
{
    public class CryptoController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public CryptoController(IWebHostEnvironment env) => _env = env;

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Process(EncryptViewModel model)
        {
            string targetDir = Path.Combine(_env.ContentRootPath, "Files");
            Directory.CreateDirectory(targetDir);

            try
            {
                // ENCRYPT
                if (model.Mode == "encrypt")
                {
                    if (model.File == null)
                    {
                        ViewBag.Error = "Please select a file.";
                        return View("Index");
                    }

                    string inputPath = Path.Combine(targetDir, model.File.FileName);
                    using (var s = new FileStream(inputPath, FileMode.Create))
                        model.File.CopyTo(s);

                    // RC5
                    var rc5 = new RC5Encryptor(model.Password);
                    var rc5Timer = Stopwatch.StartNew();
                    string rc5Out = Path.Combine(targetDir, $"{Path.GetFileNameWithoutExtension(model.File.FileName)}_RC5.bin");
                    rc5.EncryptFile(inputPath, rc5Out);
                    rc5Timer.Stop();

                    // RSA
                    var rsa = new RSAEncryptor();
                    var rsaTimer = Stopwatch.StartNew();
                    byte[] sample = System.IO.File.ReadAllBytes(inputPath).Take(200).ToArray();
                    string rsaOut = Path.Combine(targetDir, $"{Path.GetFileNameWithoutExtension(model.File.FileName)}_RSA.bin");
                    rsa.EncryptBytes(sample, rsaOut);
                    rsaTimer.Stop();

                    rsa.SaveKeys(targetDir);

                    ViewBag.Message =
                        $"✅ RC5 encrypted: {Path.GetFileName(rc5Out)} ({rc5Timer.ElapsedMilliseconds} ms)<br/>" +
                        $"✅ RSA encrypted: {Path.GetFileName(rsaOut)} ({rsaTimer.ElapsedMilliseconds} ms)<br/>" +
                        $"🔑 Keys saved in {targetDir}";

                    ViewBag.PublicModulus = rsa.PublicModulus;
                    ViewBag.PublicExponent = rsa.PublicExponent;
                    ViewBag.PrivateModulus = rsa.PrivateModulus;
                    ViewBag.PrivateExponent = rsa.PrivateExponent;
                    ViewBag.P = rsa.P;
                    ViewBag.Q = rsa.Q;
                    ViewBag.DP = rsa.DP;
                    ViewBag.DQ = rsa.DQ;
                    ViewBag.InverseQ = rsa.InverseQ;
                }

                // DECRYPT
                else if (model.Mode == "decrypt")
                {
                    if (model.Rc5File == null || model.RsaFile == null)
                    {
                        ViewBag.Error = "Please select both encrypted files.";
                        return View("Index");
                    }

                    string rc5Path = Path.Combine(targetDir, model.Rc5File.FileName);
                    using (var s = new FileStream(rc5Path, FileMode.Create))
                        model.Rc5File.CopyTo(s);

                    string rsaPath = Path.Combine(targetDir, model.RsaFile.FileName);
                    using (var s = new FileStream(rsaPath, FileMode.Create))
                        model.RsaFile.CopyTo(s);

                    // RC5
                    var rc5 = new RC5Encryptor(model.Password);
                    var rc5Timer = Stopwatch.StartNew();
                    string rc5Out = Path.Combine(targetDir, $"{Path.GetFileNameWithoutExtension(model.Rc5File.FileName)}_dec.txt");
                    rc5.DecryptFile(rc5Path, rc5Out);
                    rc5Timer.Stop();

                    // RSA
                    string privateKeyPath = Path.Combine(targetDir, "private_key.xml");
                    if (!System.IO.File.Exists(privateKeyPath))
                    {
                        ViewBag.Error = "❌ private_key.xml not found. Encrypt first!";
                        return View("Index");
                    }

                    var rsa = new RSAEncryptor(privateKeyPath);
                    var rsaTimer = Stopwatch.StartNew();
                    string rsaOut = Path.Combine(targetDir, $"{Path.GetFileNameWithoutExtension(model.RsaFile.FileName)}_dec.txt");
                    rsa.DecryptFile(rsaPath, rsaOut);
                    rsaTimer.Stop();

                    ViewBag.Message =
                        $"✅ RC5 decrypted: {Path.GetFileName(rc5Out)} ({rc5Timer.ElapsedMilliseconds} ms)<br/>" +
                        $"✅ RSA decrypted: {Path.GetFileName(rsaOut)} ({rsaTimer.ElapsedMilliseconds} ms)";
                }
                else
                {
                    ViewBag.Error = "Invalid mode selected.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"⚠️ {ex.Message}";
            }

            return View("Index");
        }
    }
}
