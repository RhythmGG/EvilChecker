using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static EvilChecker.Minecraft_General.CryptoHandler;

namespace EvilChecker.Minecraft_General
{
	public class RegularAesStream : Stream, IAesStream
	{
		// Token: 0x060007C9 RID: 1993 RVA: 0x0002B164 File Offset: 0x00029364
		public RegularAesStream(Stream stream, byte[] key)
		{
			this.BaseStream = stream;
			this.enc = new CryptoStream(stream, this.GenerateAES(key).CreateEncryptor(), CryptoStreamMode.Write);
			this.dec = new CryptoStream(stream, this.GenerateAES(key).CreateDecryptor(), CryptoStreamMode.Read);
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060007CA RID: 1994 RVA: 0x0002B1B0 File Offset: 0x000293B0
		// (set) Token: 0x060007CB RID: 1995 RVA: 0x0002B1B8 File Offset: 0x000293B8
		public Stream BaseStream { get; set; }

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060007CC RID: 1996 RVA: 0x00008359 File Offset: 0x00006559
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060007CD RID: 1997 RVA: 0x00008206 File Offset: 0x00006406
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060007CE RID: 1998 RVA: 0x00008359 File Offset: 0x00006559
		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x0002B1C1 File Offset: 0x000293C1
		public override void Flush()
		{
			this.BaseStream.Flush();
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060007D0 RID: 2000 RVA: 0x00008209 File Offset: 0x00006409
		public override long Length
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060007D1 RID: 2001 RVA: 0x00008209 File Offset: 0x00006409
		// (set) Token: 0x060007D2 RID: 2002 RVA: 0x00008209 File Offset: 0x00006409
		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x0002B1CE File Offset: 0x000293CE
		public override int ReadByte()
		{
			return this.dec.ReadByte();
		}

		// Token: 0x060007D4 RID: 2004 RVA: 0x0002B1DB File Offset: 0x000293DB
		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.dec.Read(buffer, offset, count);
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x00008209 File Offset: 0x00006409
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x00008209 File Offset: 0x00006409
		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x0002B1EB File Offset: 0x000293EB
		public override void WriteByte(byte b)
		{
			this.enc.WriteByte(b);
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x0002B1F9 File Offset: 0x000293F9
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.enc.Write(buffer, offset, count);
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x0002B209 File Offset: 0x00029409
		private RijndaelManaged GenerateAES(byte[] key)
		{
			return new RijndaelManaged
			{
				Mode = CipherMode.CFB,
				Padding = PaddingMode.None,
				KeySize = 128,
				FeedbackSize = 8,
				Key = key,
				IV = key
			};
		}

		// Token: 0x0400058D RID: 1421
		private CryptoStream enc;

		// Token: 0x0400058E RID: 1422
		private CryptoStream dec;
	}
}
