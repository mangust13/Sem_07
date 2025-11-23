using Cipher.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Cipher;

public class RC5Encryptor
{
    private const int W = 16;
    private const int R = 8;
    private const int KEY_BITS = 8;

    private readonly RC5 _rc5;

    public RC5Encryptor(string password)
    {
        byte[] key = DeriveKeyFromPassword(password, KEY_BITS);
        _rc5 = new RC5(W, R, key);
    }


    public void EncryptFile(string inputPath, string outputPath)
    {
        byte[] data = File.ReadAllBytes(inputPath);

        // Генеруємо IV
        var a = (uint)Math.Pow(9, 3);
        var m = (uint)Math.Pow(2, 22) - 1;
        uint c = 233, x0 = 5;

        var rng = new RandomNumber(a, c, m);
        var randomValues = rng.GenerateNumbers(x0, 2);

        byte[] iv = BitConverter.GetBytes(randomValues[0])
            .Concat(BitConverter.GetBytes(randomValues[1]))
            .Take(8)
            .ToArray();

        byte[] cipher = _rc5.EncryptCBC(data, iv);

        byte[] ivEncrypted = _rc5.EncryptBlock(iv);
        File.WriteAllBytes(outputPath, ivEncrypted.Concat(cipher).ToArray());
    }

    public void DecryptFile(string inputPath, string outputPath)
    {
        byte[] full = File.ReadAllBytes(inputPath);

        byte[] ivEncrypted = full[..8];
        byte[] iv = _rc5.DecryptBlock(ivEncrypted);
        byte[] cipher = full[8..];

        byte[] plain = _rc5.DecryptCBC(cipher, iv);
        File.WriteAllBytes(outputPath, plain);
    }

    // KEY DERIVATION 

    private static byte[] DeriveKeyFromPassword(string password, int keyBits)
    {
        var pwdBytes = Encoding.UTF8.GetBytes(password);
        var h1 = MyMd5.CalculateHash(pwdBytes);

        if (keyBits == 64)
            return h1[..8];

        if (keyBits == 256)
        {
            var h2 = MyMd5.CalculateHash(h1);
            byte[] key = new byte[32];
            Array.Copy(h2, 0, key, 0, 16);
            Array.Copy(h1, 0, key, 16, 16);
            return key;
        }

        return h1;
    }
}
