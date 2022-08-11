
/********************************************************************
created:    2015-06-21
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Globalization;
using Unicorn;

namespace Metadata
{
    public class MetaType
    {
		public MetaType (Type rawType)
		{
			_rawType = rawType;

			if (typeof(Template).IsAssignableFrom(rawType))
			{
				IsTemplate = true;
			} 
			else if (typeof(Config).IsAssignableFrom(rawType))
			{
				IsConfig = true;
			}
		}

		public CodeAssembly GetCodeAssembly ()
		{
			if (_codeAssembly == CodeAssembly.None)
			{
				var fullname = _rawType.Assembly.FullName;
				if (fullname.StartsWithEx("Assembly-CSharp-firstpass", CompareOptions.Ordinal))
				{
					_codeAssembly = CodeAssembly.StandardAssembly;
				}
				else if (_rawType.Assembly == TypeTools.GetEditorAssembly())
				{
					_codeAssembly = CodeAssembly.EditorAssembly;
				}
				else 
				{
					_codeAssembly = CodeAssembly.ClientAssembly;
				}
			}

			return _codeAssembly;
		}

		public string GetAutoCodeDirectory ()
		{
			var codeAssembly = GetCodeAssembly();

			if (codeAssembly == CodeAssembly.StandardAssembly)
			{
                return EditorMetaTools.StandardAutoCodeDirectory;
			}
			else if (codeAssembly == CodeAssembly.ClientAssembly)
			{
                return EditorMetaTools.ClientAutoCodeDirectory;
			}
			
			return string.Empty;
		}

		public ExportFlags GetExportFlags ()
		{
			if (_exportFlags == ExportFlags.None)
			{
				_exportFlags = EditorMetaTools.GetExportFlags(_rawType);
			}

			return _exportFlags;
		}

		public Type RawType		{ get { return _rawType; } }
		public bool IsTemplate	{ get; private set; }
		public bool IsConfig	{ get; private set; }

		public string Name			{ get { return _rawType.Name; } }
		public string Namespace 	{ get { return _rawType.Namespace; } }
		public string FullName		{ get { return _rawType.FullName; } }
		public bool IsAbstract		{ get { return _rawType.IsAbstract; } }
		public bool IsNested		{ get { return _rawType.IsNested; } }
		public bool IsSerializable	{ get { return _rawType.IsSerializable; } }

		private Type _rawType;
		private CodeAssembly _codeAssembly;
		private ExportFlags _exportFlags;
    }
}