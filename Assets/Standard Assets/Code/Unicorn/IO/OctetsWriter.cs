
/********************************************************************
created:    2015-05-05
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;

namespace Unicorn.IO
{
	public class OctetsWriter : BinaryWriter, IOctetsWriter
    {
		public OctetsWriter (Stream stream): this (stream, OctetsMode.None)
		{

		}

        public OctetsWriter (Stream stream, OctetsMode mode): base(stream, Kernel.UTF8)
        {
//			_mode = mode;
			_dontCompress = (mode & OctetsMode.Compress) == 0;
        }

        public void Write (float x, float y)
		{
			_floatBuffer[0] = x;
			_floatBuffer[1] = y;
			
			_WriteFloatBuffer(8);
		}

        public void Write (float x, float y, float z)
        {
            _floatBuffer[0] = x;
            _floatBuffer[1] = y;
            _floatBuffer[2] = z;

            _WriteFloatBuffer(12);
        }
		
        public void Write (float x, float y, float z, float w)
		{
			_floatBuffer[0] = x;
			_floatBuffer[1] = y;
			_floatBuffer[2] = z;
			_floatBuffer[3] = w;
			
			_WriteFloatBuffer(16);
		}

		public override void Write (string s)
		{
			s = s ?? string.Empty;
			base.Write (s);
		}

		private void _WriteFloatBuffer (int count)
		{
			Buffer.BlockCopy(_floatBuffer, 0, _byteBuffer, 0, count);
			base.Write(_byteBuffer, 0, count);
		}

		public override void Write (ushort val)
		{
			if (_dontCompress)
			{
				base.Write(val);
			}
			else
			{
				_Pack((uint) val);
			}
		}

		public override void Write (int val)
		{
			if (_dontCompress)
			{
				base.Write(val);
			}
			else
			{
				_Pack(val);
			}
		}

		public override void Write (uint val)
		{
			if (_dontCompress)
			{
				base.Write(val);
			}
			else
			{
				_Pack(val);
			}
		}

		private void _Pack (int val)
		{
			uint zigged = (uint) ((val << 1) ^ (val >> 31));
			_Pack(zigged);
		}

		private void _Pack (uint val)
		{
			int count = 0;
			
			do
			{
				_byteBuffer[count++] = (byte) ((val & 0x7F) | 0x80);
			}
			while ((val >>= 7) != 0);
			
			_byteBuffer[count - 1] &= 0x7F;
			base.Write(_byteBuffer, 0, count);
		}

		private readonly float[] _floatBuffer = new float[4];
		private readonly byte[]  _byteBuffer  = new byte[16];

		private readonly bool _dontCompress;
    }
}
