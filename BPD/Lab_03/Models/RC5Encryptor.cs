using Lab_03.Models;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Lab_03;

public class RC5Encryptor
{
    private const int W = 16;      // word size
    private const int R = 8;      // rounds
    private const int KEY_BITS = 32;

    private readonly RC5 _rc5;

    public RC5Encryptor(string password)
    {
        byte[] key = DeriveKeyFromPassword(password, KEY_BITS);
        _rc5 = new RC5(R, key);
    }

    // ========================= MAIN METHODS =========================

    public void EncryptFile(string inputPath, string outputPath)
    {
        byte[] data = File.ReadAllBytes(inputPath);

        // Генеруємо IV через власний генератор
        var rng = new RandomNumber(1103515245, 12345, 0x7FFFFFFF);
        uint seed = (uint)DateTime.Now.Ticks;
        var randomValues = rng.GenerateNumbers(10, 2);

        byte[] iv = BitConverter.GetBytes(randomValues[0])
            .Concat(BitConverter.GetBytes(randomValues[1]))
            .Take(8)
            .ToArray();

        byte[] cipher = _rc5.EncryptCBC(data, iv);

        // перший блок файлу — зашифрований IV
        byte[] ivEncrypted = _rc5.EncryptBlock(iv);
        File.WriteAllBytes(outputPath, ivEncrypted.Concat(cipher).ToArray());
    }

    public void DecryptFile(string inputPath, string outputPath)
    {
        byte[] full = File.ReadAllBytes(inputPath);

        // Перші 8 байтів — IV у зашифрованому вигляді
        byte[] ivEncrypted = full[..8];
        byte[] iv = _rc5.DecryptBlock(ivEncrypted);
        byte[] cipher = full[8..];

        byte[] plain = _rc5.DecryptCBC(cipher, iv);
        File.WriteAllBytes(outputPath, plain);
    }

    // ========================= KEY DERIVATION =========================

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

        // За замовчуванням 128-бітовий ключ
        return h1;
    }
}
