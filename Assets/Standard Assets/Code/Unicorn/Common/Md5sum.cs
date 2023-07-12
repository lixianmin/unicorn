
/********************************************************************
created:    2014-03-04
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System.Security.Cryptography;

namespace Unicorn
{
    public class Md5sum
    {
        public string GetHexDigest32(byte[] inputBuffer)
        {
            if (null == inputBuffer)
            {
                return string.Empty;
            }

            byte[] bytes = _md5.ComputeHash(inputBuffer);

            if (null == _outputBuffer32)
            {
                _outputBuffer32 = new char[32];
            }

            int bufferLength = bytes.Length << 1;
            int index = 0;
            for (int i = 0; i < bufferLength; i += 2)
            {
                byte one = bytes[index++];

                int a = one >> 4;
                int b = one - (a << 4);
                _outputBuffer32[i] = a < 10 ? (char)(a + 48) : (char)(a - 10 + 97);
                _outputBuffer32[i + 1] = b < 10 ? (char)(b + 48) : (char)(b - 10 + 97);
            }

            var digest = new string(_outputBuffer32);
            return digest;
        }

        public string GetHexDigest16(byte[] inputBuffer, int startIndex, int length)
        {
            // unable to reproduce the following commented bug
            // (If inputBuffer.Length is 0, then _md5.ComputeHash will deadlock.)
            //
            if (null == inputBuffer)
            {
                return string.Empty;
            }

            byte[] bytes = _md5.ComputeHash(inputBuffer);

            if (null == _outputBuffer16)
            {
                _outputBuffer16 = new char[16];
            }

            int bufferLength = bytes.Length;
            int index = 0;
            for (int i = 0; i < bufferLength; i += 2)
            {
                byte one = bytes[4 + index];
                ++index;

                int a = one >> 4;
                int b = one - (a << 4);
                _outputBuffer16[i] = a < 10 ? (char)(a + 48) : (char)(a - 10 + 97);
                _outputBuffer16[i + 1] = b < 10 ? (char)(b + 48) : (char)(b - 10 + 97);
            }

            var digest = new string(_outputBuffer16, startIndex, length);
            return digest;
        }

        public string GetHexDigest16(byte[] inputBuffer)
        {
            return GetHexDigest16(inputBuffer, 0, 16);
        }

        /// <summary>
        /// 资源上传到CDN服务器时，用于防止由于文件同名而不被CDN更新，配合文件名使用，
		/// 以filename+digest的形式命名新文件，因此只需要非常短的签名就可以了
        /// </summary>
        public string GetAssetDigest(byte[] inputBuffer)
        {
            return GetHexDigest16(inputBuffer, 0, AssetDigestLength);
        }

        public static readonly Md5sum It = new Md5sum();
        public const int AssetDigestLength = 4;

        private char[] _outputBuffer16;
        private char[] _outputBuffer32;

        private readonly MD5 _md5 = new MD5CryptoServiceProvider();
    }
}
