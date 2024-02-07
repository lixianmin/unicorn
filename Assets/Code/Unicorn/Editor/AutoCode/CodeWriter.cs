/********************************************************************
created:    2014-02-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;
using System.Text;
using System.Globalization;

namespace Unicorn.AutoCode
{
    public class CodeWriter : IDisposable
    {
        public CodeWriter(string path)
        {
            _writer = new StreamWriter(path);
            _writer.NewLine = os.linesep;

            _InitTabs();

            var lineComment = _GetLineComment(path);
            _WriteFileHead(lineComment);
        }

        ~CodeWriter()
        {
            _DoDispose(false);
        }

        public void Dispose()
        {
            _DoDispose(true);
        }

        protected virtual void _DoDispose(bool isDisposing)
        {
            _writer?.Dispose();
            _writer = null;
        }

        private void _InitTabs()
        {
            var sb = new StringBuilder();

            var count = _indentTexts.Length;
            for (int i = 0; i < count; ++i)
            {
                _indentTexts[i] = sb.ToString();
                sb.Append("    ");
            }
        }

        public void IncreaseIndent()
        {
            _indent = _indent >= _kMaxIndentCount ? _kMaxIndentCount : (_indent + 1);
        }

        public void DecreaseIndent()
        {
            _indent = _indent <= 0 ? 0 : (_indent - 1);
        }

        public void Write(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (message.Trim().Length != 0)
                {
                    _writer.Write(_indentTexts[_indent]);
                }

                _writer.Write(message);
            }
        }

        public void Write(string format, params object[] args)
        {
            if (null == format)
            {
                return;
            }

            var message = string.Format(null, format, args);
            if (message.Trim().Length != 0)
            {
                _writer.Write(_indentTexts[_indent]);
            }

            _writer.Write(message);
        }

        public void WriteLine()
        {
            _writer.WriteLine();
        }

        public void WriteLine(string message)
        {
            Write(message);
            _writer.WriteLine();
        }

        public void WriteLine(string format, params object[] args)
        {
            if (null != format)
            {
                Write(format, args);
                _writer.WriteLine();
            }
        }

        private void _WriteFileHead(string lineComment)
        {
            _writer.WriteLine();

            _writer.Write(lineComment);
            _writer.WriteLine(
                "Warning: all code of this file are generated automatically, so do not modify it manually ~");

            _writer.Write(lineComment);
//			_writer.WriteLine("Any questions are welcome, mailto:lixianmin@gmail.com  O(\u2229_\u2229)O");
            _writer.WriteLine("Any questions are welcome, mailto:lixianmin@gmail.com");

            _writer.WriteLine();
        }

        private string _GetLineComment(string path)
        {
            if (path.EndsWith(".cs", CompareOptions.OrdinalIgnoreCase))
            {
                return "// ";
            }

            if (path.EndsWith(".lua", CompareOptions.OrdinalIgnoreCase))
            {
                return "-- ";
            }

            return string.Empty;
        }

        public StreamWriter BaseWriter => _writer;

        public int Indent => _indent;

        private const int _kMaxIndentCount = 20;
        private string[] _indentTexts = new string[_kMaxIndentCount];

        private StreamWriter _writer;
        private int _indent;
    }
}