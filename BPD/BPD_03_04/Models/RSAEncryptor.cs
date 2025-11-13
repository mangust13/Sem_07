using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace Cipher.Models
{
    public class RSAEncryptor
    {
        private readonly RSACryptoServiceProvider _rsa;

        public RSAEncryptor(string? privateKeyPath = null)
        {
            _rsa = new RSACryptoServiceProvider(2048);

            if (!string.IsNullOrEmpty(privateKeyPath) && File.Exists(privateKeyPath))
            {
                string xml = File.ReadAllText(privateKeyPath);
                _rsa.FromXmlString(xml);
            }
        }

        // --- Публічні та приватні компоненти ---
        public string PublicModulus { get; private set; } = "";
        public string PublicExponent { get; private set; } = "";
        public string PrivateModulus { get; private set; } = "";
        public string PrivateExponent { get; private set; } = "";
        public string P { get; private set; } = "";
        public string Q { get; private set; } = "";
        public string DP { get; private set; } = "";
        public string DQ { get; private set; } = "";
        public string InverseQ { get; private set; } = "";

        public void SaveKeys(string dir)
        {
            File.WriteAllText(Path.Combine(dir, "public_key.xml"), _rsa.ToXmlString(false));
            File.WriteAllText(Path.Combine(dir, "private_key.xml"), _rsa.ToXmlString(true));
        }

        public void EncryptFile(string inputPath, string outputPath)
        {
            byte[] data = File.ReadAllBytes(inputPath);
            byte[] encrypted = _rsa.Encrypt(data, false);
            File.WriteAllBytes(outputPath, encrypted);
            ParseKeyComponents();
        }
        public void DecryptFile(string inputPath, string outputPath)
        {
            byte[] encrypted = File.ReadAllBytes(inputPath);
            byte[] decrypted = _rsa.Decrypt(encrypted, false);
            File.WriteAllBytes(outputPath, decrypted);
            ParseKeyComponents();
        }

        public void EncryptBytes(byte[] data, string outputPath)
        {
            byte[] enc = _rsa.Encrypt(data, false);
            File.WriteAllBytes(outputPath, enc);
            ParseKeyComponents();
        }

        public byte[] DecryptBytes(byte[] encrypted)
        {
            return _rsa.Decrypt(encrypted, false);
        }

        private void ParseKeyComponents()
        {
            try
            {
                // public
                var pubXml = XDocument.Parse(_rsa.ToXmlString(false));
                PublicModulus = pubXml.Root?.Element("Modulus")?.Value ?? "";
                PublicExponent = pubXml.Root?.Element("Exponent")?.Value ?? "";

                // private
                var privXml = XDocument.Parse(_rsa.ToXmlString(true));
                PrivateModulus = privXml.Root?.Element("Modulus")?.Value ?? "";
                PrivateExponent = privXml.Root?.Element("D")?.Value ?? "";
                P = privXml.Root?.Element("P")?.Value ?? "";
                Q = privXml.Root?.Element("Q")?.Value ?? "";
                DP = privXml.Root?.Element("DP")?.Value ?? "";
                DQ = privXml.Root?.Element("DQ")?.Value ?? "";
                InverseQ = privXml.Root?.Element("InverseQ")?.Value ?? "";
            }
            catch
            {
                PublicModulus = PublicExponent = PrivateModulus = PrivateExponent =
                P = Q = DP = DQ = InverseQ = "";
            }
        }
    }
}
