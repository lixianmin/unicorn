
/********************************************************************
created:    2014-09-03
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;

namespace Unicorn
{
    public static class ExtendedStream
    {
        public static bool ReadBooleanEx (this Stream stream)
        {
            if (null == stream)
            {
                throw new ArgumentNullException("stream = null.");
            }

            return stream.ReadByte() != 0;
        }

        public static void WriteEx (this Stream stream, bool v)
        {
            if (null == stream)
            {
                throw new ArgumentNullException("stream = null.");
            }

            _byteBuffer[0] = (byte)((!v) ? 0 : 1);
            stream.Write(_byteBuffer, 0, 1);
        }

        public static float ReadSingleEx (this Stream stream)
        {
            if (null == stream)
            {
                throw new ArgumentNullException("stream = null.");
            }

            _ReadFloatBuffer(stream, 4);
            var result = _floatBuffer[0];
            return result;
        }

        public static void WriteEx (this Stream stream, float num)
        {
            if (null == stream)
            {
                throw new ArgumentNullException("stream = null.");
            }

            _floatBuffer[0] = num;
            _WriteFloatBuffer(stream, 4);
        }

		public static int ReadInt32Ex (this Stream stream)
		{
			if (null == stream)
			{
                throw new ArgumentNullException("stream = null.");
			}

			stream.Read(_byteBuffer, 0, 4);

			var num = (int) _byteBuffer [0]
					| (int) _byteBuffer [1] << 8 
					| (int) _byteBuffer [2] << 16
					| (int) _byteBuffer [3] << 24;

			return num;
		}

        public static void WriteEx (this Stream stream, int num)
        {
            if (null == stream)
            {
                throw new ArgumentNullException("stream = null.");
            }

            _byteBuffer [0] = (byte) num;
            _byteBuffer [1] = (byte) (num >> 8);
            _byteBuffer [2] = (byte) (num >> 16);
            _byteBuffer [3] = (byte) (num >> 24);

            stream.Write(_byteBuffer, 0, 4);
        }

        public static long ReadInt64Ex (this Stream stream)
        {
            if (null == stream)
            {
                throw new ArgumentNullException("stream = null.");
            }

            stream.Read(_byteBuffer, 0, 8);

            uint num = (uint) _byteBuffer [0]
                     | (uint) _byteBuffer [1] << 8 
                     | (uint) _byteBuffer [2] << 16
                     | (uint) _byteBuffer [3] << 24;

            uint num2= (uint) _byteBuffer [4]
                     | (uint) _byteBuffer [5] << 8 
                     | (uint) _byteBuffer [6] << 16
                     | (uint) _byteBuffer [7] << 24;

            var result = (long)((ulong)num2 << 32 | (ulong)num);
            return result;
        }

        public static void WriteEx (this Stream stream, long num)
        {
            if (null == stream)
            {
                throw new ArgumentNullException("stream = null.");
            }

            int shift = 0;
            for (int i= 0; i< 8; ++i)
            {
                _byteBuffer[i] = (byte)(num >> shift);
                shift += 8;
            }

            stream.Write(_byteBuffer, 0, 8);
        }

		public static byte[] ReadBytesEx (this Stream stream)
		{
			if (null == stream)
			{
                throw new ArgumentNullException("stream = null.");
			}

			var count = stream.ReadInt32Ex();
			if (count < 0)
			{
				var message = string.Format("count={0}", count);
				throw new Exception(message);
			}

			var buffer = new byte[count];
			stream.Read(buffer, 0, count);

			return buffer;
		}

		public static void WriteEx (this Stream stream, byte[] bytes)
		{
			if (null == stream)
			{
                throw new ArgumentNullException("stream = null.");
			}
			
            bytes = bytes ?? EmptyArray<byte>.Instance;
			var count = bytes.Length;
			stream.WriteEx(count);
			stream.Write(bytes, 0, count);
		}

        private static void _ReadFloatBuffer (Stream stream, int count)
        {
            stream.Read(_byteBuffer, 0, count);
            Buffer.BlockCopy(_byteBuffer, 0, _floatBuffer, 0, count);
        }

        private static void _WriteFloatBuffer (Stream stream, int count)
        {
            Buffer.BlockCopy(_floatBuffer, 0, _byteBuffer, 0, count);
            stream.Write(_byteBuffer, 0, count);
        }

        private static readonly float[] _floatBuffer = new float[4];
        private static readonly byte[]  _byteBuffer = new byte[16];
    }
}