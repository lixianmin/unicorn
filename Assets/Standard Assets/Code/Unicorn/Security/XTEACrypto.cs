
/********************************************************************
created:    2015-09-18
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;

namespace Unicorn.Security
{
    public class XTEACrypto
    {
		private delegate void ProcessorType (uint round, uint[] key, ref uint v0, ref uint v1);

		public XTEACrypto (int round, uint[] key)
		{
			if (round <= 0)
			{
				var message = "\"round\" should be a positive number, round= " + round;
				throw new ArgumentOutOfRangeException(message);
			}

			if (null == key || key.Length != 4)
			{
				var message = "\"key\" should be a 4 length array";
				throw new ArgumentOutOfRangeException(message);
			}

			_round = (uint) round;
			_key = key;
		}

		public void Encrypt (byte[] buffer)
		{
			if (null != buffer && buffer.Length > 0)
			{
				_XTEAEncrypt(buffer, _round, _key, _Encipher);
				_XOREncrypt(buffer, _key);
			}
		}

		public void Decrypt (byte[] buffer)
		{
			if (null != buffer && buffer.Length > 0)
			{
				_XOREncrypt(buffer, _key);
				_XTEAEncrypt(buffer, _round, _key, _Decipher);
			}
		}

		private static void _XTEAEncrypt (byte[] buffer, uint round, uint[] key, ProcessorType processor)
		{
			var bufferLength = buffer.Length;
			var endIndex = bufferLength - 8;

			for (int i= 0; i < endIndex; i += 8)
			{
				var readIndex = i;
				var v0 =  (uint) buffer[readIndex]
						| (uint) buffer[++readIndex] << 8
						| (uint) buffer[++readIndex] << 16
						| (uint) buffer[++readIndex] << 24;
				var v1 =  (uint) buffer[++readIndex]
						| (uint) buffer[++readIndex] << 8
						| (uint) buffer[++readIndex] << 16
						| (uint) buffer[++readIndex] << 24;
				
				processor(round, key, ref v0, ref v1);

				var writeIndex = i;
				buffer [writeIndex]		= (byte) (v0);
				buffer [++writeIndex]	= (byte) (v0 >> 8);
				buffer [++writeIndex]	= (byte) (v0 >> 16);
				buffer [++writeIndex]	= (byte) (v0 >> 24);
				buffer [++writeIndex]	= (byte) (v1);
				buffer [++writeIndex]	= (byte) (v1 >> 8);
				buffer [++writeIndex]	= (byte) (v1 >> 16);
				buffer [++writeIndex]	= (byte) (v1 >> 24);
			}
		}

		private static void _XOREncrypt (byte[] buffer, uint[] key)
		{
			var bufferLength = buffer.Length;
			
			for (int i= 0; i < bufferLength; ++i)
			{
				// because keyLength will constantly be 4, so we can replace %4 with &3
				// buffer[i] = (byte) ( buffer[i] ^ key[i % keyLength]);
				buffer[i] = (byte) ( buffer[i] ^ key[i & 3]);
			}
		}

		private static void _Encipher (uint round, uint[] key, ref uint v0, ref uint v1)
		{
			uint sum = 0;
			uint delta = 0x9E3779B9;
			
			for (uint i= 0; i < round; i++)
			{
				v0 += (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
				sum += delta;
				v1 += (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum>>11) & 3]);
			}
		}

		private static void _Decipher (uint round, uint[] key, ref uint v0, ref uint v1)
		{
			uint delta = 0x9E3779B9;
			uint sum = delta * round;

			for (uint roundIndex=0; roundIndex < round; roundIndex++)
			{
				v1 -= (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + key[(sum>>11) & 3]);
				sum -= delta;
				v0 -= (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + key[sum & 3]);
			}
		}

//		private static uint _ReadUInt32 (byte[] buffer, int index)
//		{
//			var result = (uint)  buffer[index]
//						| (uint) buffer[++index] << 8
//						| (uint) buffer[++index] << 16
//						| (uint) buffer[++index] << 24;
//
//			return result;
//		}
//
//		private static void _WriteUInt32 (byte[] buffer, int index, uint val)
//		{
//			buffer [index]		= (byte) val;
//			buffer [++index]	= (byte) (val >> 8);
//			buffer [++index]	= (byte) (val >> 16);
//			buffer [++index]	= (byte) (val >> 24);
//		}

		private readonly uint   _round;
		private readonly uint[] _key;
    }
}
