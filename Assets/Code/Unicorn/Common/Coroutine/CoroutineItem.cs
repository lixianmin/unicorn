
/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Collections;

namespace Unicorn
{
	public class CoroutineItem: IIsYieldable
	{
		internal CoroutineItem ()
		{

		}

		public void Kill ()
		{
			isKilled = true;
		}

		public override string ToString ()
		{
			return string.Format ("[CoroutineItem] routine={0}, isDone={1}, isKilled={2}, isRecyclable={3}"
			                      , routine
			                      , isDone.ToString()
			                      , isKilled.ToString()
			                      , isRecyclable.ToString());
		}

		bool IIsYieldable.isYieldable
		{
			get
			{
				return isDone || isKilled;
			}
		}

		internal IEnumerator	routine			{ get; set; }
		internal bool 			isRecyclable	{ get; set; }

		public bool		       	isDone			{ get; internal set; }	// done normally.
		public bool				isKilled 		{ get; internal set; }	// killed manually.
	}
}