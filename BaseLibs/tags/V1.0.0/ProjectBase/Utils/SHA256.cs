using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace ProjectBase.Utils
{

	public class SHA256
	{
        public static string CreateSalt(int size)
        {
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

        public static string GenerateHash(string input, string salt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input + salt);
            SHA256Managed sHA256ManagedString = new SHA256Managed();
            byte[] hash = sHA256ManagedString.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static bool AreEqual(string plainTextInput, string hashedInput, string salt)
        {
            string newHashedPin = GenerateHash(plainTextInput, salt);
            return newHashedPin.Equals(hashedInput);
        }

		#region "旧算法，为了与E2biz站点兼容"
		private const int BITS_TO_A_BYTE = 8;
		private const int BYTES_TO_A_WORD = 4;

		private const int BITS_TO_A_WORD = 32;
		private static long[] m_lOnBits = new long[31];
		private static long[] m_l2Power = new long[31];

		private static long[] K = new long[64];
		private static void Init()
		{
			m_lOnBits[0] = 1;
			m_lOnBits[1] = 3;
			m_lOnBits[2] = 7;
			m_lOnBits[3] = 15;
			m_lOnBits[4] = 31;
			m_lOnBits[5] = 63;
			m_lOnBits[6] = 127;
			m_lOnBits[7] = 255;
			m_lOnBits[8] = 511;
			m_lOnBits[9] = 1023;
			m_lOnBits[10] = 2047;
			m_lOnBits[11] = 4095;
			m_lOnBits[12] = 8191;
			m_lOnBits[13] = 16383;
			m_lOnBits[14] = 32767;
			m_lOnBits[15] = 65535;
			m_lOnBits[16] = 131071;
			m_lOnBits[17] = 262143;
			m_lOnBits[18] = 524287;
			m_lOnBits[19] = 1048575;
			m_lOnBits[20] = 2097151;
			m_lOnBits[21] = 4194303;
			m_lOnBits[22] = 8388607;
			m_lOnBits[23] = 16777215;
			m_lOnBits[24] = 33554431;
			m_lOnBits[25] = 67108863;
			m_lOnBits[26] = 134217727;
			m_lOnBits[27] = 268435455;
			m_lOnBits[28] = 536870911;
			m_lOnBits[29] = 1073741823;
			m_lOnBits[30] = 2147483647;

			m_l2Power[0] = 1;
			m_l2Power[1] = 2;
			m_l2Power[2] = 4;
			m_l2Power[3] = 8;
			m_l2Power[4] = 16;
			m_l2Power[5] = 32;
			m_l2Power[6] = 64;
			m_l2Power[7] = 128;
			m_l2Power[8] = 256;
			m_l2Power[9] = 512;
			m_l2Power[10] = 1024;
			m_l2Power[11] = 2048;
			m_l2Power[12] = 4096;
			m_l2Power[13] = 8192;
			m_l2Power[14] = 16384;
			m_l2Power[15] = 32768;
			m_l2Power[16] = 65536;
			m_l2Power[17] = 131072;
			m_l2Power[18] = 262144;
			m_l2Power[19] = 524288;
			m_l2Power[20] = 1048576;
			m_l2Power[21] = 2097152;
			m_l2Power[22] = 4194304;
			m_l2Power[23] = 8388608;
			m_l2Power[24] = 16777216;
			m_l2Power[25] = 33554432;
			m_l2Power[26] = 67108864;
			m_l2Power[27] = 134217728;
			m_l2Power[28] = 268435456;
			m_l2Power[29] = 536870912;
			m_l2Power[30] = 1073741824;

			K[0] = 0x428a2f98;
			K[1] = 0x71374491;
			K[2] = 0xb5c0fbcf;
			K[3] = 0xe9b5dba5;
			K[4] = 0x3956c25b;
			K[5] = 0x59f111f1;
			K[6] = 0x923f82a4;
			K[7] = 0xab1c5ed5;
			K[8] = 0xd807aa98;
			K[9] = 0x12835b01;
			K[10] = 0x243185be;
			K[11] = 0x550c7dc3;
			K[12] = 0x72be5d74;
			K[13] = 0x80deb1fe;
			K[14] = 0x9bdc06a7;
			K[15] = 0xc19bf174;
			K[16] = 0xe49b69c1;
			K[17] = 0xefbe4786;
			K[18] = 0xfc19dc6;
			K[19] = 0x240ca1cc;
			K[20] = 0x2de92c6f;
			K[21] = 0x4a7484aa;
			K[22] = 0x5cb0a9dc;
			K[23] = 0x76f988da;
			K[24] = 0x983e5152;
			K[25] = 0xa831c66d;
			K[26] = 0xb00327c8;
			K[27] = 0xbf597fc7;
			K[28] = 0xc6e00bf3;
			K[29] = 0xd5a79147;
			K[30] = 0x6ca6351;
			K[31] = 0x14292967;
			K[32] = 0x27b70a85;
			K[33] = 0x2e1b2138;
			K[34] = 0x4d2c6dfc;
			K[35] = 0x53380d13;
			K[36] = 0x650a7354;
			K[37] = 0x766a0abb;
			K[38] = 0x81c2c92e;
			K[39] = 0x92722c85;
			K[40] = 0xa2bfe8a1;
			K[41] = 0xa81a664b;
			K[42] = 0xc24b8b70;
			K[43] = 0xc76c51a3;
			K[44] = 0xd192e819;
			K[45] = 0xd6990624;
			K[46] = 0xf40e3585;
			K[47] = 0x106aa070;
			K[48] = 0x19a4c116;
			K[49] = 0x1e376c08;
			K[50] = 0x2748774c;
			K[51] = 0x34b0bcb5;
			K[52] = 0x391c0cb3;
			K[53] = 0x4ed8aa4a;
			K[54] = 0x5b9cca4f;
			K[55] = 0x682e6ff3;
			K[56] = 0x748f82ee;
			K[57] = 0x78a5636f;
			K[58] = 0x84c87814;
			K[59] = 0x8cc70208;
			K[60] = 0x90befffa;
			K[61] = 0xa4506ceb;
			K[62] = 0xbef9a3f7;
			K[63] = 0xc67178f2;
		}

		/// <summary>
		/// E2biz站点进行单点登录验证时使用，一般情况下应使用GenerateHash算法。此方法只是为了保持与E2biz站点的兼容
		/// </summary>
		/// <param name="psSecurity"></param>
		/// <returns></returns>
		public static string ComputeHash(string psSecurity)
		{
			Init();

			long[] HASH = new long[8];
			long[] M;
			long[] W = new long[64];
			long a;
			long b;
			long c;
			long d;
			long e;
			long f;
			long g;
			long h;
			long i;
			long j;
			long T1;
			long T2;

			HASH[0] = 0x6a09e667;
			HASH[1] = 0xbb67ae85;
			HASH[2] = 0x3c6ef372;
			HASH[3] = 0xa54ff53a;
			HASH[4] = 0x510e527f;
			HASH[5] = 0x9b05688c;
			HASH[6] = 0x1f83d9ab;
			HASH[7] = 0x5be0cd19;

			M = ConvertToWordArray(psSecurity);

			for (i = 0; i < M.Length; i += 16)
			{
				a = HASH[0];
				b = HASH[1];
				c = HASH[2];
				d = HASH[3];
				e = HASH[4];
				f = HASH[5];
				g = HASH[6];
				h = HASH[7];

				for (j = 0; j <= 63; j++)
				{
					if (j < 16)
					{
						W[j] = M[j + i];
					}
					else
					{
						W[j] = AddUnsigned(AddUnsigned(AddUnsigned(Gamma1(W[j - 2]), W[j - 7]), Gamma0(W[j - 15])), W[j - 16]);
					}

					T1 = AddUnsigned(AddUnsigned(AddUnsigned(AddUnsigned(h, Sigma1(e)), Ch(e, f, g)), K[j]), W[j]);
					T2 = AddUnsigned(Sigma0(a), Maj(a, b, c));

					h = g;
					g = f;
					f = e;
					e = AddUnsigned(d, T1);
					d = c;
					c = b;
					b = a;
					a = AddUnsigned(T1, T2);
				}

				HASH[0] = AddUnsigned(a, HASH[0]);
				HASH[1] = AddUnsigned(b, HASH[1]);
				HASH[2] = AddUnsigned(c, HASH[2]);
				HASH[3] = AddUnsigned(d, HASH[3]);
				HASH[4] = AddUnsigned(e, HASH[4]);
				HASH[5] = AddUnsigned(f, HASH[5]);
				HASH[6] = AddUnsigned(g, HASH[6]);
				HASH[7] = AddUnsigned(h, HASH[7]);
			}

			string sReturn = Strings.LCase(Strings.Right("00000000" + Conversion.Hex(HASH[0]), 8) + Strings.Right("00000000" + Conversion.Hex(HASH[1]), 8) + Strings.Right("00000000" + Conversion.Hex(HASH[2]), 8) + Strings.Right("00000000" + Conversion.Hex(HASH[3]), 8) + Strings.Right("00000000" + Conversion.Hex(HASH[4]), 8) + Strings.Right("00000000" + Conversion.Hex(HASH[5]), 8) + Strings.Right("00000000" + Conversion.Hex(HASH[6]), 8) + Strings.Right("00000000" + Conversion.Hex(HASH[7]), 8));
			return sReturn;
		}

		private static long[] ConvertToWordArray(string sMessage)
		{
			int lMessageLength;
			int lNumberOfWords;

			int lBytePosition;
			int lByteCount;
			long lWordCount;
			long lByte;

			const int MODULUS_BITS = 512;
			const int CONGRUENT_BITS = 448;

			lMessageLength = sMessage.Length;

			lNumberOfWords = (((lMessageLength + ((MODULUS_BITS - CONGRUENT_BITS) / BITS_TO_A_BYTE)) / (MODULUS_BITS / BITS_TO_A_BYTE)) + 1) * (MODULUS_BITS / BITS_TO_A_WORD);

			long[] lWordArray = new long[lNumberOfWords];

			lBytePosition = 0;
			lByteCount = 0;
			while (!(lByteCount >= lMessageLength))
			{
				lWordCount = lByteCount / BYTES_TO_A_WORD;

				lBytePosition = (3 - (lByteCount % BYTES_TO_A_WORD)) * BITS_TO_A_BYTE;

				lByte = Strings.Asc(sMessage.Substring(lByteCount, 1));

				lWordArray[lWordCount] = lWordArray[lWordCount] | LShift(lByte, lBytePosition);
				lByteCount = lByteCount + 1;
			}
			lWordCount = lByteCount / BYTES_TO_A_WORD;
			lBytePosition = (3 - (lByteCount % BYTES_TO_A_WORD)) * BITS_TO_A_BYTE;
			lWordArray[lWordCount] = lWordArray[lWordCount] | LShift(0x80, lBytePosition);
			lWordArray[lNumberOfWords - 1] = LShift(lMessageLength, 3);
			lWordArray[lNumberOfWords - 2] = RShift(lMessageLength, 29);

			return lWordArray;
		}

		private static long Ch(long x, long y, long z)
		{
			return ((x & y) ^ ((~x) & z));
		}
		private static long Maj(long x, long y, long z)
		{
			return ((x & y) ^ (x & z) ^ (y & z));
		}
		private static long S(long x, long n)
		{
			return (RShift(x, (n & m_lOnBits[4])) | LShift(x, (32 - (n & m_lOnBits[4]))));
		}
		private static long R(long x, long n)
		{
			return RShift(x, n & m_lOnBits[4]);
		}
		private static long Sigma0(long x)
		{
			return (S(x, 2) ^ S(x, 13) ^ S(x, 22));
		}
		private static long Sigma1(long x)
		{
			return (S(x, 6) ^ S(x, 11) ^ S(x, 25));
		}
		private static long Gamma0(long x)
		{
			return (S(x, 7) ^ S(x, 18) ^ R(x, 3));
		}
		private static long Gamma1(long x)
		{
			return (S(x, 17) ^ S(x, 19) ^ R(x, 10));
		}
		private static long LShift(long lValue, long iShiftBits)
		{

			if (iShiftBits == 0)
			{
				return lValue;
			}
			else if (iShiftBits == 31)
			{
				if (Convert.ToBoolean(lValue & 1))
				{
					return 0x80000000;
				}
				else
				{
					return 0;
				}

			}
			else if (iShiftBits < 0 | iShiftBits > 31)
			{
				Information.Err().Raise(6);
			}

			if (Convert.ToBoolean(lValue & m_l2Power[31 - iShiftBits]))
			{
				return ((lValue & m_lOnBits[31 - (iShiftBits + 1)]) * m_l2Power[iShiftBits]) | 0x80000000;
			}
			else
			{
				return ((lValue & m_lOnBits[31 - iShiftBits]) * m_l2Power[iShiftBits]);
			}

		}
		private static long RShift(long lValue, long iShiftBits)
		{

			if (iShiftBits == 0)
			{
				return lValue;

			}
			else if (iShiftBits == 31)
			{
				if ((lValue & 0x80000000) == 1)
				{
					return 1;
				}
				else
				{
					return 0;
				}

			}
			else if (iShiftBits < 0 | iShiftBits > 31)
			{
				Information.Err().Raise(6);
			}

			long v = (lValue & 0x7ffffffe) / m_l2Power[iShiftBits];

			if (Convert.ToBoolean(lValue & 0x80000000))
			{
				v = (v | (0x40000000 / m_l2Power[iShiftBits - 1]));
			}
			return v;
		}
		private static long AddUnsigned(long lX, long lY)
		{
			long lX4;
			long lY4;
			long lX8;
			long lY8;
			long lResult;

			lX8 = lX & 0x80000000;
			lY8 = lY & 0x80000000;
			lX4 = lX & 0x40000000;
			lY4 = lY & 0x40000000;

			lResult = (lX & 0x3fffffff) + (lY & 0x3fffffff);

			if (Convert.ToBoolean(lX4 & lY4))
			{
				lResult = lResult ^ 0x80000000 ^ lX8 ^ lY8;
			}
			else if (Convert.ToBoolean(lX4 | lY4))
			{
				if (Convert.ToBoolean(lResult & 0x40000000))
				{
					lResult = lResult ^ 0xc0000000 ^ lX8 ^ lY8;
				}
				else
				{
					lResult = lResult ^ 0x40000000 ^ lX8 ^ lY8;
				}
			}
			else
			{
				lResult = lResult ^ lX8 ^ lY8;
			}

			return lResult;
		}
		#endregion
	}


}
