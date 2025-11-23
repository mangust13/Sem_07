using System;
using System.Text;

namespace Cipher;

public class RC5
{
    private const uint Pw = 0xB7E15163;
    private const uint Qw = 0x9E3779B9;

    private readonly int r, w;
    private readonly uint[] S;

    public RC5(int W, int rounds, byte[] key)
    {
        w = W;
        r = rounds;
        S = KeyExpansion(key);
    }

    // Encrypt
    public byte[] EncryptBlock(byte[] input)
    {
        if (input.Length != 8)
            throw new ArgumentException("RC5 works on 64-bit blocks (8 bytes).");

        uint A = BitConverter.ToUInt32(input, 0);
        uint B = BitConverter.ToUInt32(input, 4);

        A += S[0];
        B += S[1];

        for (int i = 1; i <= r; i++)
        {
            A = RotateLeft(A ^ B, (int)B) + S[2 * i];
            B = RotateLeft(B ^ A, (int)A) + S[2 * i + 1];
        }

        byte[] output = new byte[8];
        Array.Copy(BitConverter.GetBytes(A), 0, output, 0, 4);
        Array.Copy(BitConverter.GetBytes(B), 0, output, 4, 4);
        return output;
    }

    // Decrypt
    public byte[] DecryptBlock(byte[] input)
    {
        if (input.Length != 8)
            throw new ArgumentException("RC5 works on 64-bit blocks (8 bytes).");

        uint A = BitConverter.ToUInt32(input, 0);
        uint B = BitConverter.ToUInt32(input, 4);

        for (int i = r; i >= 1; i--)
        {
            B = RotateRight(B - S[2 * i + 1], (int)A) ^ A;
            A = RotateRight(A - S[2 * i], (int)B) ^ B;
        }

        B -= S[1];
        A -= S[0];

        byte[] output = new byte[8];
        Array.Copy(BitConverter.GetBytes(A), 0, output, 0, 4);
        Array.Copy(BitConverter.GetBytes(B), 0, output, 4, 4);
        return output;
    }

    // KEY EXPANSION
    private uint[] KeyExpansion(byte[] key)
    {
        int u = w / 8;
        int c = Math.Max(1, key.Length / u);
        uint[] L = new uint[c];

        for (int i = key.Length - 1; i >= 0; i--)
        {
            L[i / u] = (L[i / u] << 8) + key[i];
        }
        
        uint[] S = new uint[2 * (r + 1)];
        S[0] = Pw;
        for (int i = 1; i < S.Length; i++)
            S[i] = S[i - 1] + Qw;

        uint A = 0, B = 0;
        int n = 3 * Math.Max(S.Length, L.Length);
        int si = 0, li = 0;

        for (int i = 0; i < n; i++)
        {
            A = S[si] = RotateLeft(S[si] + A + B, 3);
            B = L[li] = RotateLeft(L[li] + A + B, (int)(A + B));
            si = (si + 1) % S.Length;
            li = (li + 1) % L.Length;
        }

        return S;
    }

    // Utilities
    private static uint RotateLeft(uint value, int shift)
    {
        shift &= 31;
        return (value << shift) | (value >> (32 - shift));
    }

    private static uint RotateRight(uint value, int shift)
    {
        shift &= 31;
        return (value >> shift) | (value << (32 - shift));
    }

    // CBC mode

    private static byte[] ApplyPadding(byte[] data)
    {
        int padLen = 8 - (data.Length % 8);
        byte[] padded = new byte[data.Length + padLen];
        Array.Copy(data, padded, data.Length);
        for (int i = data.Length; i < padded.Length; i++)
            padded[i] = (byte)padLen;
        return padded;
    }

    private static byte[] RemovePadding(byte[] data)
    {
        int padLen = data[^1];
        if (padLen <= 0 || padLen > 8)
            throw new InvalidOperationException("Invalid padding");
        byte[] unpadded = new byte[data.Length - padLen];
        Array.Copy(data, unpadded, unpadded.Length);
        return unpadded;
    }

    public byte[] EncryptCBC(byte[] data, byte[] iv)
    {
        byte[] padded = ApplyPadding(data);
        byte[] prev = new byte[8];
        Array.Copy(iv, prev, 8);

        byte[] result = new byte[padded.Length];

        for (int i = 0; i < padded.Length; i += 8)
        {
            byte[] block = new byte[8];
            Array.Copy(padded, i, block, 0, 8);

            for (int j = 0; j < 8; j++)
                block[j] ^= prev[j];

            byte[] cipher = EncryptBlock(block);
            Array.Copy(cipher, 0, result, i, 8);
            prev = cipher;
        }

        return result;
    }

    public byte[] DecryptCBC(byte[] cipher, byte[] iv)
    {
        if (cipher.Length % 8 != 0)
            throw new ArgumentException("Ciphertext length must be multiple of block size.");

        byte[] prev = new byte[8];
        Array.Copy(iv, prev, 8);

        byte[] result = new byte[cipher.Length];

        for (int i = 0; i < cipher.Length; i += 8)
        {
            byte[] block = new byte[8];
            Array.Copy(cipher, i, block, 0, 8);

            byte[] plain = DecryptBlock(block);

            for (int j = 0; j < 8; j++)
                plain[j] ^= prev[j];

            Array.Copy(plain, 0, result, i, 8);
            prev = block;
        }

        return RemovePadding(result);
    }

}
