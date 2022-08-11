
/********************************************************************
created:    2014-02-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.IO;

namespace Unicorn.AutoCode
{
    public struct MacroScope: IDisposable
    {
		private MacroScope (StreamWriter writer, string prefix)
        {
            _writer = writer;

			if (!string.IsNullOrEmpty(prefix))
			{
				_writer.Write(prefix);
			}
		}

		public static MacroScope CreateEditorScope (StreamWriter writer)
		{
			return new MacroScope (writer, "#if UNITY_EDITOR\n");
		}

        public void Dispose ()
        {
			_writer.Write("#endif\n");
        }

		private StreamWriter _writer;
    }
}