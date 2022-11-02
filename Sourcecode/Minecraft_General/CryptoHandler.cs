using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EvilChecker.Minecraft_General
{
    class CryptoHandler
    {
		public static RSACryptoServiceProvider DecodeRSAPublicKey(byte[] x509key)
		{
			byte[] second = new byte[]
			{
				42,
				134,
				72,
				134,
				247,
				13,
				1,
				1,
				1
			};
			BinaryReader binaryReader = new BinaryReader(new MemoryStream(x509key));
			if (binaryReader.ReadByte() != 48)
			{
				return null;
			}
			CryptoHandler.ReadASNLength(binaryReader);
			if (binaryReader.ReadByte() == 48)
			{
				int num = CryptoHandler.ReadASNLength(binaryReader);
				if (binaryReader.ReadByte() == 6)
				{
					byte[] array = new byte[CryptoHandler.ReadASNLength(binaryReader)];
					binaryReader.Read(array, 0, array.Length);
					if (!array.SequenceEqual(second))
					{
						return null;
					}
					int count = num - 2 - array.Length;
					binaryReader.ReadBytes(count);
				}
				if (binaryReader.ReadByte() == 3)
				{
					CryptoHandler.ReadASNLength(binaryReader);
					binaryReader.ReadByte();
					if (binaryReader.ReadByte() == 48)
					{
						CryptoHandler.ReadASNLength(binaryReader);
						if (binaryReader.ReadByte() == 2)
						{
							byte[] array2 = new byte[CryptoHandler.ReadASNLength(binaryReader)];
							binaryReader.Read(array2, 0, array2.Length);
							if (array2[0] == 0)
							{
								byte[] array3 = new byte[array2.Length - 1];
								Array.Copy(array2, 1, array3, 0, array2.Length - 1);
								array2 = array3;
							}
							if (binaryReader.ReadByte() == 2)
							{
								byte[] array4 = new byte[CryptoHandler.ReadASNLength(binaryReader)];
								binaryReader.Read(array4, 0, array4.Length);
								RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider();
								rsacryptoServiceProvider.ImportParameters(new RSAParameters
								{
									Modulus = array2,
									Exponent = array4
								});
								return rsacryptoServiceProvider;
							}
						}
					}
				}
				return null;
			}
			return null;
		}
		private static int ReadASNLength(BinaryReader reader)
		{
			int num = (int)reader.ReadByte();
			if ((num & 128) == 128)
			{
				int num2 = num & 15;
				byte[] array = new byte[4];
				reader.Read(array, 4 - num2, num2);
				Array.Reverse(array);
				num = BitConverter.ToInt32(array, 0);
			}
			return num;
		}
		public static byte[] GenerateAESPrivateKey()
		{
			AesManaged aesManaged = new AesManaged();
			aesManaged.KeySize = 128;
			aesManaged.GenerateKey();
			return aesManaged.Key;
		}
		public static string getServerHash(string serverID, byte[] PublicKey, byte[] SecretKey)
		{
			byte[] array = CryptoHandler.digest(new byte[][]
			{
				Encoding.GetEncoding("iso-8859-1").GetBytes(serverID),
				SecretKey,
				PublicKey
			});
			bool flag = (array[0] & 128) == 128;
			if (flag)
			{
				array = CryptoHandler.TwosComplementLittleEndian(array);
			}
			string text = CryptoHandler.GetHexString(array).TrimStart(new char[]
			{
				'0'
			});
			if (flag)
			{
				text = "-" + text;
			}
			return text;
		}
		private static byte[] digest(byte[][] tohash)
		{
			SHA1CryptoServiceProvider sha1CryptoServiceProvider = new SHA1CryptoServiceProvider();
			for (int i = 0; i < tohash.Length; i++)
			{
				sha1CryptoServiceProvider.TransformBlock(tohash[i], 0, tohash[i].Length, tohash[i], 0);
			}
			sha1CryptoServiceProvider.TransformFinalBlock(new byte[0], 0, 0);
			return sha1CryptoServiceProvider.Hash;
		}
		private static string GetHexString(byte[] p)
		{
			string text = string.Empty;
			for (int i = 0; i < p.Length; i++)
			{
				text += p[i].ToString("x2");
			}
			return text;
		}
		private static byte[] TwosComplementLittleEndian(byte[] p)
		{
			bool flag = true;
			for (int i = p.Length - 1; i >= 0; i--)
			{
				p[i] = (byte)~p[i];
				if (flag)
				{
					flag = (p[i] == byte.MaxValue);
					int num = i;
					p[num] += 1;
				}
			}
			return p;
		}
		public static IAesStream getAesStream(Stream underlyingStream, byte[] AesKey)
		{
			return new RegularAesStream(underlyingStream, AesKey);
		}
		public interface IAesStream
		{
			// Token: 0x060007B6 RID: 1974
			int Read(byte[] buffer, int offset, int count);

			// Token: 0x060007B7 RID: 1975
			void Write(byte[] buffer, int offset, int count);
		}
	}
}
