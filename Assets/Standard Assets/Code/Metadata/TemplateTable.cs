
/********************************************************************
created:    2013-12-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Unicorn.Collections;

namespace Metadata
{
	//[HeapDump(HeapDumpFlags.DontDump)]
    public class TemplateTable: SortedTable<int, Template>
    {
        internal TemplateTable ()
        {

        }

        // public TemplateTable (int capacity): base(capacity)
        // {
        //
        // }

		internal bool IsCompleted;
    }
}