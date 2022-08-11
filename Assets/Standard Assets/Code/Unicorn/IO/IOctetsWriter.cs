
/********************************************************************
created:    2015-10-26
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.IO;

namespace Unicorn.IO
{
    public interface IOctetsWriter
    {
        void Write (float x, float y);
        void Write (float x, float y, float z);
        void Write (float x, float y, float z, float w);

		void Write (bool b);
		void Write (byte b);
		void Write (double d);
		void Write (short d);
		void Write (int d);
		void Write (long d);

		void Write (sbyte d);
		void Write (float d);
		void Write (string s);

		void Write (ushort d);
		void Write (uint d);
		void Write (ulong d);
    }
}