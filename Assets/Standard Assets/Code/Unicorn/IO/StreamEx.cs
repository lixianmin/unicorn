using System;
using System.IO;

namespace Unicorn.IO
{
	public static class StreamEx
	{
		private static readonly float[] _floatBuffer = new float[4];

		private static readonly byte[] _byteBuffer = new byte[16];

		public static bool ReadBoolean(this Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream = null.");
			}
			return stream.ReadByte() != 0;
		}

		public static void Write(this Stream stream, bool v)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream = null.");
			}
			_byteBuffer[0] = (byte)(v ? 1 : 0);
			stream.Write(_byteBuffer, 0, 1);
		}

		public static float ReadSingle(this Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream = null.");
			}
			_ReadFloatBuffer(stream, 4);
			return _floatBuffer[0];
		}

		public static void Write(this Stream stream, float num)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream = null.");
			}
			_floatBuffer[0] = num;
			_WriteFloatBuffer(stream, 4);
		}

		public static int ReadInt32(this Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream = null.");
			}
			stream.Read(_byteBuffer, 0, 4);
			return _byteBuffer[0] | (_byteBuffer[1] << 8) | (_byteBuffer[2] << 16) | (_byteBuffer[3] << 24);
		}

		public static void Write(this Stream stream, int num)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream = null.");
			}
			_byteBuffer[0] = (byte)num;
			_byteBuffer[1] = (byte)(num >> 8);
			_byteBuffer[2] = (byte)(num >> 16);
			_byteBuffer[3] = (byte)(num >> 24);
			stream.Write(_byteBuffer, 0, 4);
		}

		public static long ReadInt64(this Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream = null.");
			}
			stream.Read(_byteBuffer, 0, 8);
			uint num = (uint)(_byteBuffer[0] | (_byteBuffer[1] << 8) | (_byteBuffer[2] << 16) | (_byteBuffer[3] << 24));
			uint num2 = (uint)(_byteBuffer[4] | (_byteBuffer[5] << 8) | (_byteBuffer[6] << 16) | (_byteBuffer[7] << 24));
			return (long)(((ulong)num2 << 32) | num);
		}

		public static void Write(this Stream stream, long num)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream = null.");
			}
			int num2 = 0;
			for (int i = 0; i < 8; i++)
			{
				_byteBuffer[i] = (byte)(num >> num2);
				num2 += 8;
			}
			stream.Write(_byteBuffer, 0, 8);
		}

		private static void _ReadFloatBuffer(Stream stream, int count)
		{
			stream.Read(_byteBuffer, 0, count);
			Buffer.BlockCopy(_byteBuffer, 0, _floatBuffer, 0, count);
		}

		private static void _WriteFloatBuffer(Stream stream, int count)
		{
			Buffer.BlockCopy(_floatBuffer, 0, _byteBuffer, 0, count);
			stream.Write(_byteBuffer, 0, count);
		}
	}
}
