using System;
using System.IO;

namespace Unicorn.IO
{
    public class OctetsStream : Stream
    {
        public virtual int Capacity
        {
            get => _buffer.Length;
            set
            {
                var lastCapacity = _buffer.Length;
                if (value != lastCapacity)
                {
                    if (value < 0 || value < _length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value),
                            "New capacity cant be negative or less than the data length" + value + " " + _length);
                    }

                    var array = Array.Empty<byte>();
                    if (value != 0)
                    {
                        array = new byte[value];
                        Buffer.BlockCopy(_buffer, 0, array, 0, _length);
                    }

                    _buffer = array;
                }
            }
        }

        public override long Position
        {
            get => _position - 0;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Position cannot be negative");
                }

                if (value > int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException("value",
                        "Position must be non-negative and less than 2^31 - 1 - origin");
                }

                _position = (int)value;
            }
        }

        public override long Length => _length;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public OctetsStream()
            : this(0)
        {
        }

        public OctetsStream(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            _buffer = new byte[capacity];
        }

        public override long Seek(long offset, SeekOrigin loc)
        {
            if (offset > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("Offset out of range. " + offset);
            }

            int num;
            switch (loc)
            {
                case SeekOrigin.Begin:
                    if (offset < 0)
                    {
                        throw new IOException("Attempted to seek before start of OctetsStream.");
                    }

                    num = 0;
                    break;
                case SeekOrigin.Current:
                    num = _position;
                    break;
                case SeekOrigin.End:
                    num = _length;
                    break;
                default:
                    throw new ArgumentException("loc", "Invalid SeekOrigin");
            }

            num += (int)offset;
            if (num < 0)
            {
                throw new IOException("Attempted to seek before start of OctetsStream.");
            }

            _position = num;
            return _position;
        }

        public override void SetLength(long value)
        {
            if (value > _buffer.Length)
            {
                throw new NotSupportedException("Expanding this OctetsStream is not supported");
            }

            if (value < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            int num = (int)value;
            if (num > _length)
            {
                _Expand(num);
            }

            _length = num;
            if (_position > _length)
            {
                _position = _length;
            }
        }

        private void _Expand(int newSize)
        {
            var capacity = _buffer.Length;
            if (newSize > capacity)
            {
                int num = newSize;
                if (num < 32)
                {
                    num = 32;
                }
                else if (num < capacity << 1)
                {
                    num = capacity << 1;
                }

                Capacity = num;
            }
        }

        public override int ReadByte()
        {
            if (_position >= _length)
            {
                return -1;
            }

            return _buffer[_position++];
        }

        public override void WriteByte(byte value)
        {
            if (_position >= _length)
            {
                _Expand(_position + 1);
                _length = _position + 1;
            }

            _buffer[_position++] = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException("offset or count less than zero.");
            }

            if (buffer.Length - offset < count)
            {
                throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
            }

            if (_position >= _length || count == 0)
            {
                return 0;
            }

            if (_position > _length - count)
            {
                count = _length - _position;
            }

            Buffer.BlockCopy(_buffer, _position, buffer, offset, count);
            _position += count;
            return count;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (buffer.Length < offset + count)
            {
                throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
            }

            if (count == 0)
            {
                return;
            }

            var newSize = _position + count;
            if (newSize > _length)
            {
                _Expand(_position + count);
            }

            Buffer.BlockCopy(buffer, offset, _buffer, _position, count);
            _position += count;
            if (_position >= _length)
            {
                _length = _position;
            }
        }

        public void Tidy()
        {
            var count = _length - _position;
            if (count > 0)
            {
                Buffer.BlockCopy(_buffer, _position, _buffer, 0, count);
            }

            _position = 0;
            _length = count;
        }

        public void WriteTo(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            stream.Write(_buffer, 0, _length);
        }

        // public byte[] GetBuffer()
        // {
        //     return _buffer;
        // }

        public byte[] ToArray()
        {
            var num = _length;
            var array = new byte[num];
            Buffer.BlockCopy(_buffer, 0, array, 0, num);

            return array;
        }

        public override void Flush()
        {
        }

        public void Reset()
        {
            _position = 0;
            _length = 0;
        }

        private byte[] _buffer;
        private int _position;
        private int _length;
    }
}