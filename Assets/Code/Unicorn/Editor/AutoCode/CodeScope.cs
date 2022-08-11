
/********************************************************************
created:    2014-02-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.IO;

namespace Unicorn.AutoCode
{
    public struct CodeScope: IDisposable
    {
        private CodeScope (CodeWriter writer, string prefix, string postfix)
        {
            _writer = writer;
			_postfix= postfix;

			if (!string.IsNullOrEmpty(prefix))
			{
				_writer.Write(prefix);
			}

            _writer.IncreaseIndent();
        }

		public static CodeScope Create (CodeWriter writer, string prefix, string postfix)
		{
			prefix	= prefix ?? string.Empty;
			postfix	= postfix ?? string.Empty;

			return new CodeScope (writer, prefix, postfix);
        }

		public static CodeScope CreateCSharpScope (CodeWriter writer)
		{
			return new CodeScope (writer, "{\n", "}\n");
		}

		public static CodeScope CreateLuaScope (CodeWriter writer)
		{
            return new CodeScope (writer, string.Empty, "end\n");
		}

        public void Dispose ()
        {
            _writer.DecreaseIndent();

			if (!string.IsNullOrEmpty(_postfix))
			{
				_writer.Write(_postfix);
			}
        }

        private CodeWriter _writer;
        private string _postfix;
    }
}