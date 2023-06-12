/********************************************************************
created:    2014-12-02
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Text;
using System.IO;

namespace Unicorn.IO
{
    public class OctetsReader : BinaryReader, IOctetsReader
    {
        public OctetsReader(Stream stream, OctetsMode mode = OctetsMode.UseFilter)
            : base(stream, Kernel.UTF8)
        {
            _stream = stream;
            _decoder = Kernel.UTF8.GetDecoder();
            _mode = mode;
            _dontCompress = (mode & OctetsMode.Compress) == 0;

            if (_UseFilter())
            {
                SetFilter(_defaultFilter);
            }

            if (null == _stream)
            {
                throw new IOException("Stream is invalid");
            }
        }

        public void SetFilter(Func<char[], int, int, string> filter)
        {
            _charFilter = filter ?? _defaultFilter;
        }

        private string _defaultFilter(char[] buffer, int startIndex, int count)
        {
            if (null == _defaultIntern)
            {
                // OctetsReader不能copy os.intern的数据， 那个array实在是太大了，一份有30K
                // 的new string[]数组.
                _defaultIntern = new StringIntern();
            }

            SetFilter(_defaultIntern.Intern);

            var text = _defaultIntern.Intern(buffer, startIndex, count);
            return text;
        }

        public override string ReadString()
        {
            int textLength = Read7BitEncodedInt();

            if (textLength < 0)
            {
                throw new IOException("Invalid binary file (string len < 0)");
            }

            if (textLength == 0)
            {
                return string.Empty;
            }

            while (textLength > 0)
            {
                var byteCount = (textLength <= _basicLength) ? textLength : _basicLength;
                _FillBuffer(byteCount);

                var charCount = _decoder.GetChars(_bytes, 0, byteCount, _chars, 0);

                if (_builderCount + charCount > _builder.Length)
                {
                    var nextBuilder = new char[_builder.Length << 1];
                    Buffer.BlockCopy(_builder, 0, nextBuilder, 0, _builderCount << 1);
                    _builder = nextBuilder;
                }

                Buffer.BlockCopy(_chars, 0, _builder, _builderCount << 1, charCount << 1);
                _builderCount += charCount;

                textLength -= byteCount;
            }

            var text = _UseFilter() ? _charFilter(_builder, 0, _builderCount) : new string(_builder, 0, _builderCount);
            _builderCount = 0;

            return text;
        }

        private void _FillBuffer(int numBytes)
        {
            int num;
            for (int i = 0; i < numBytes; i += num)
            {
                num = _stream.Read(_bytes, i, numBytes - i);

                if (num == 0)
                {
                    throw new EndOfStreamException();
                }
            }
        }

        private bool _UseFilter()
        {
            return (_mode & OctetsMode.UseFilter) != 0;
        }

        public void ReadVector(out float x, out float y)
        {
            _ReadFloatBuffer(8);

            x = _floatBuffer[0];
            y = _floatBuffer[1];
        }

        public void ReadVector(out float x, out float y, out float z)
        {
            _ReadFloatBuffer(12);

            x = _floatBuffer[0];
            y = _floatBuffer[1];
            z = _floatBuffer[2];
        }

        public void ReadVector(out float x, out float y, out float z, out float w)
        {
            _ReadFloatBuffer(16);

            x = _floatBuffer[0];
            y = _floatBuffer[1];
            z = _floatBuffer[2];
            w = _floatBuffer[3];
        }

        private void _ReadFloatBuffer(int count)
        {
            _stream.Read(_byteBuffer, 0, count);
            Buffer.BlockCopy(_byteBuffer, 0, _floatBuffer, 0, count);
        }

        public override ushort ReadUInt16()
        {
            if (_dontCompress)
            {
                return base.ReadUInt16();
            }
            else
            {
                return (ushort)_UnpackUInt32();
            }
        }

        public override int ReadInt32()
        {
            if (_dontCompress)
            {
                return base.ReadInt32();
            }
            else
            {
                return _UnpackInt32();
            }
        }

        public override uint ReadUInt32()
        {
            if (_dontCompress)
            {
                return base.ReadUInt32();
            }
            else
            {
                return _UnpackUInt32();
            }
        }

        public new int Read7BitEncodedInt()
        {
            return base.Read7BitEncodedInt();
        }

        private int _UnpackInt32()
        {
            uint zigged = _UnpackUInt32();
            int val = (int)zigged;
            var result = (-(val & 0x01)) ^ ((val >> 1) & ~(1 << 31));
            return result;
        }

        private uint _UnpackUInt32()
        {
            uint val = base.ReadByte();
            if ((val & 0x80) == 0)
            {
                return val;
            }

            val &= 0x7F;
            uint chunk = base.ReadByte();
            val |= (chunk & 0x7F) << 7;
            if ((chunk & 0x80) == 0)
            {
                return val;
            }

            chunk = base.ReadByte();
            val |= (chunk & 0x7F) << 14;
            if ((chunk & 0x80) == 0)
            {
                return val;
            }

            chunk = base.ReadByte();
            val |= (chunk & 0x7F) << 21;
            if ((chunk & 0x80) == 0)
            {
                return val;
            }

            chunk = base.ReadByte();
            val |= chunk << 28;
            if ((chunk & 0xF0) == 0)
            {
                return val;
            }

            throw new OverflowException("ReadUInt32 Error!");
        }

        private readonly Decoder _decoder;
        private readonly Stream _stream;
        private readonly OctetsMode _mode;
        private readonly bool _dontCompress;

        private const int _basicLength = 128;
        private readonly byte[] _bytes = new byte[_basicLength];
        private readonly char[] _chars = new char[_basicLength];
        private readonly float[] _floatBuffer = new float[4];
        private readonly byte[] _byteBuffer = new byte[16];

        private char[] _builder = new char[_basicLength];
        private int _builderCount;

        private StringIntern _defaultIntern;
        private Func<char[], int, int, string> _charFilter;
    }
}