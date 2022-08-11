
/********************************************************************
created:    2015-09-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;

namespace Unicorn.IO
{
    public class BoolPacker
    {
		public void AttachStream (Stream stream)
		{
			if (null == stream)
			{
				var message = "stream should not be null";
				throw new ArgumentNullException(message);
			}

			_stream = stream;
			_Reset();
		}

		public bool Read ()
		{
			if (null == _stream)
			{
				return false;
			}

			if (_index == 0)
			{
				_byteValue = _stream.ReadByte();
			}

			var result = (_byteValue & (1 << _index)) != 0;
			_index = (1 + _index) % 8;

			return result;
		}

		public void Write (bool val)
		{
			if (null == _stream)
			{
				return;
			}

			_byteValue |= ((val ? 1 : 0) << _index);
			_index = (1 + _index) % 8;
			_canFlush = true;

			if (_index == 0)
			{
				Flush();
			}
			else if (_index == 1)
			{
				_bytePosition = (int) _stream.Position;
				_stream.WriteByte((byte) 0);
			}
		}

		public void Flush ()
		{
			// Flush() may be called multi times, so it must be only available when it has data.
			if (null != _stream && _canFlush)
			{
				var lastPosition = _stream.Position;
				_stream.Seek(_bytePosition, SeekOrigin.Begin);
				_stream.WriteByte((byte) _byteValue);
				_stream.Seek(lastPosition, SeekOrigin.Begin);
				
				_Reset();
			}
		}

		private void _Reset ()
		{
			_index = 0;
			_byteValue = 0;
			_bytePosition = 0;
			_canFlush = false;
		}

		private Stream _stream;

		private int _index;
		private int _byteValue;
		private int	_bytePosition;
		private bool _canFlush;
    }
}