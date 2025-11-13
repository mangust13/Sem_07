namespace Lab_02.Services;

public static class MyMd5
{
    private const int INIT_A = 0x67452301;
    private const int INIT_B = unchecked((int)0xEFCDAB89u);
    private const int INIT_C = unchecked((int)0x98BADCFEu);
    private const int INIT_D = 0x10325476;

    private static readonly int[] SHIFT_AMTS = {
        7, 12, 17, 22,
        5,  9, 14, 20,
        4, 11, 16, 23,
        6, 10, 15, 21
    };

    private static readonly int[] TABLE_T = BuildTableT();

    private static int[] BuildTableT()
    {
        var t = new int[64];
        for (int i = 0; i < 64; i++)
        {
            double v = Math.Abs(Math.Sin(i + 1.0)) * Math.Pow(2.0, 32.0);
            t[i] = unchecked((int)(long)v);
        }
        return t;
    }

    public static byte[] CalculateHash(byte[] message)
    {
        int messageLenBytes = message.Length;
        int numBlocks = ((messageLenBytes + 8) >> 6) + 1;
        int totalLen = numBlocks << 6;
        var paddingBytes = new byte[totalLen - messageLenBytes];
        paddingBytes[0] = 0x80;

        long messageLenBits = (long)messageLenBytes << 3;
        for (int i = 0; i < 8; i++)
        {
            paddingBytes[paddingBytes.Length - 8 + i] = (byte)messageLenBits;
            messageLenBits = (long)((ulong)messageLenBits >> 8);
        }

        int a = INIT_A;
        int b = INIT_B;
        int c = INIT_C;
        int d = INIT_D;

        var buffer = new int[16];

        for (int block = 0; block < numBlocks; block++)
        {
            int index = block << 6;
            Array.Clear(buffer, 0, buffer.Length);

            for (int j = 0; j < 64; j++, index++)
            {
                int bVal = (index < messageLenBytes)
                    ? message[index]
                    : paddingBytes[index - messageLenBytes];
                buffer[j >> 2] = (int)((bVal & 0xFF) << 24) | ((int)((uint)buffer[j >> 2] >> 8));
            }

            int originalA = a;
            int originalB = b;
            int originalC = c;
            int originalD = d;

            for (int j = 0; j < 64; j++)
            {
                int div16 = j >> 4;
                int f = 0;
                int bufferIndex = j;

                switch (div16)
                {
                    case 0:
                        f = (b & c) | (~b & d);
                        break;
                    case 1:
                        f = (b & d) | (c & ~d);
                        bufferIndex = (bufferIndex * 5 + 1) & 0x0F;
                        break;
                    case 2:
                        f = b ^ c ^ d;
                        bufferIndex = (bufferIndex * 3 + 5) & 0x0F;
                        break;
                    case 3:
                        f = c ^ (b | ~d);
                        bufferIndex = (bufferIndex * 7) & 0x0F;
                        break;
                }

                int s = SHIFT_AMTS[(div16 << 2) | (j & 3)];
                int temp = b + RotateLeft(a + f + buffer[bufferIndex] + TABLE_T[j], s);
                a = d;
                d = c;
                c = b;
                b = temp;
            }

            a += originalA;
            b += originalB;
            c += originalC;
            d += originalD;
        }

        var md5 = new byte[16];
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            int n = (i == 0) ? a : (i == 1) ? b : (i == 2) ? c : d;
            for (int j = 0; j < 4; j++)
            {
                md5[count++] = (byte)n;
                n = (int)((uint)n >> 8);
            }
        }
        return md5;
    }

    private static int RotateLeft(int value, int bits)
    {
        unchecked
        {
            return (int)(((uint)value << bits) | ((uint)value >> (32 - bits)));
        }
    }
}
