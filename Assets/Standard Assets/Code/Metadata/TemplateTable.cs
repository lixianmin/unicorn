
/********************************************************************
created:    2013-12-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using Unicorn;
using Unicorn.Collections;

namespace Metadata
{
	//[HeapDump(HeapDumpFlags.DontDump)]
    public class TemplateTable: SortedTable<int, Template>
    {
        public TemplateTable ()
        {

        }

        public TemplateTable (int capacity): base(capacity)
        {

        }

		internal bool IsCompleted;
    }
}