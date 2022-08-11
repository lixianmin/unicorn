using System;
using System.IO;

namespace Unicorn.IO
{
	public class OctetsStream : Stream
	{
		private int _initialIndex;

		private int _dirtyBytes;

		private int _position;

		private int _capacity;

		private byte[] _buffer;

		private int _length;

		public virtual int Capacity
		{
			get
			{
				return _capacity - _initialIndex;
			}
			set
			{
				if (value != _capacity)
				{
					if (value < 0 || value < _length)
					{
						throw new ArgumentOutOfRangeException("value", "New capacity cannot be negative or less than the current capacity " + value + " " + _capacity);
					}
					if (_buffer == null || value != _buffer.Length)
					{
						byte[] array = null;
						if (value != 0)
						{
							array = new byte[value];
							if (_buffer != null)
							{
								Buffer.BlockCopy(_buffer, 0, array, 0, _length);
							}
						}
						_dirtyBytes = 0;
						_buffer = array;
						_capacity = value;
					}
				}
			}
		}

		public override long Position
		{
			get
			{
				return _position - _initialIndex;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", "Position cannot be negative");
				}
				if (value > 2147483647)
				{
					throw new ArgumentOutOfRangeException("value", "Position must be non-negative and less than 2^31 - 1 - origin");
				}
				_position = _initialIndex + (int)value;
			}
		}

		public override long Length
		{
			get
			{
				return _length - _initialIndex;
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public OctetsStream()
			: this(0)
		{
		}

		public OctetsStream(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			_capacity = capacity;
			_buffer = new byte[capacity];
		}

		public OctetsStream(byte[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException("index or count is less than 0.");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException("index+count", "The size of the buffer is less than index + count.");
			}
			_buffer = buffer;
			_capacity = count + index;
			_length = _capacity;
			_position = index;
			_initialIndex = index;
		}

		public override long Seek(long offset, SeekOrigin loc)
		{
			if (offset > 2147483647)
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
				num = _initialIndex;
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
			if (num < _initialIndex)
			{
				throw new IOException("Attempted to seek before start of OctetsStream.");
			}
			_position = num;
			return _position;
		}

		public override void SetLength(long value)
		{
			if (value > _capacity)
			{
				throw new NotSupportedException("Expanding this OctetsStream is not supported");
			}
			if (value < 0 || value + _initialIndex > 2147483647)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num = (int)value + _initialIndex;
			if (num > _length)
			{
				_Expand(num);
			}
			else if (num < _length)
			{
				_dirtyBytes += _length - num;
			}
			_length = num;
			if (_position > _length)
			{
				_position = _length;
			}
		}

		private void _Expand(int newSize)
		{
			if (newSize > _capacity)
			{
				int num = newSize;
				if (num < 32)
				{
					num = 32;
				}
				else if (num < _capacity << 1)
				{
					num = _capacity << 1;
				}
				Capacity = num;
			}
			else if (_dirtyBytes > 0)
			{
				Array.Clear(_buffer, _length, _dirtyBytes);
				_dirtyBytes = 0;
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
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException("offset+count", "The size of the buffer is less than offset + count.");
			}
			if (_position > _length - count)
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

		public void WriteTo(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			stream.Write(_buffer, _initialIndex, _length - _initialIndex);
		}

		public byte[] GetBuffer()
		{
			return _buffer;
		}

		public byte[] ToArray()
		{
			int num = _length - _initialIndex;
			byte[] array = new byte[num];
			if (_buffer != null)
			{
				Buffer.BlockCopy(_buffer, _initialIndex, array, 0, num);
			}
			return array;
		}

		public override void Flush()
		{
		}
	}
}
