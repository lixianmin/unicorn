
/********************************************************************
created:    2015-01-09
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using Unicorn;

namespace Metadata
{
    [AttributeUsage( AttributeTargets.Class 
		| AttributeTargets.Struct
		| AttributeTargets.Method
		| AttributeTargets.Property
	)]
    public class ExportAttribute: Attribute
    {
		public ExportAttribute (ExportFlags flags)
		{
			_flags = flags;
		}

		public ExportFlags GetExportFlags ()
		{
			return _flags;
		}

		private ExportFlags	_flags;
    }
}
