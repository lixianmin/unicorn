
/********************************************************************
created:    2015-10-26
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.IO;

namespace Unicorn.IO
{
    public interface IOctetsReader
    {
        void    ReadVector (out float x, out float y);
        void    ReadVector (out float x, out float y, out float z);
        void    ReadVector (out float x, out float y, out float z, out float w);

		bool	ReadBoolean ();
		byte	ReadByte ();
		double	ReadDouble ();
		short	ReadInt16 ();
		int		ReadInt32 ();
		long	ReadInt64 ();

		sbyte	ReadSByte ();
		float	ReadSingle ();
		string	ReadString ();

		ushort	ReadUInt16 ();
		uint	ReadUInt32 ();
		ulong	ReadUInt64 ();
    }
}