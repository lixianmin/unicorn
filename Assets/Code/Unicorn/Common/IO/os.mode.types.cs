
/********************************************************************
created:    2022-08-11
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using UnityEngine;

namespace Unicorn
{
	public static partial class os
	{
		enum ModeTypes
		{
			EditorMode,
			BigMemoryMode,
			Count,
		}

		private static void _InitModeTypes()
		{
			var length = (int)ModeTypes.Count;
			_modeTypes = new bool[length];
			_modeTypeLogKeys = new string[length];

			var modeTypeNames = Enum.GetNames(typeof(ModeTypes));
			for (int i = 0; i < length; ++i)
			{
				_modeTypeLogKeys[i] = modeTypeNames[i] + "@0506";
			}

			_modeTypes[(int)ModeTypes.EditorMode] = isEditor;
			_modeTypes[(int)ModeTypes.BigMemoryMode] = isBigMemory;

			if (isEditor)
			{
				for (int index = 0; index < length; ++index)
				{
					var defaultValue = _modeTypes[index] ? 1 : 0;
					var key = _modeTypeLogKeys[index];
					_modeTypes[index] = 1 == PlayerPrefs.GetInt(key, defaultValue);
				}
			}
		}

		private static void _SetModeType(ModeTypes type, bool nextValue)
		{
			var index = (int)type;

			if (_modeTypes[index] != nextValue)
			{
				_modeTypes[index] = nextValue;

				if (isEditor)
				{
					var key = _modeTypeLogKeys[index];
					PlayerPrefs.SetInt(key, nextValue ? 1 : 0);
				}
			}
		}

		public static bool isEditorMode
		{
			get { return _modeTypes[(int)ModeTypes.EditorMode]; }
			set
			{
				_SetModeType(ModeTypes.EditorMode, value);
			}
		}

		public static bool isBigMemoryMode
		{
			get { return _modeTypes[(int)ModeTypes.BigMemoryMode]; }
			set
			{
				_SetModeType(ModeTypes.BigMemoryMode, value);
			}
		}

		private static bool[] _modeTypes;
		private static string[] _modeTypeLogKeys;
	}
}