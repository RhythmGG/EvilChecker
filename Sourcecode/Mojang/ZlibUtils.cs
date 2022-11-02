using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilChecker.Properties
{
    class ZlibUtils
    {
		public static byte[] Compress(byte[] to_compress)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (ZlibStream zlibStream = new ZlibStream(memoryStream, CompressionMode.Compress))
				{
					zlibStream.Write(to_compress, 0, to_compress.Length);
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

	}
}
