
/********************************************************************
created:    2015-3-9
author:     fangxun

Copyright (C) - All Rights Reserved 
 *********************************************************************/

using System;
using System.Collections.Generic;

namespace Unicorn
{
	public class CustomArgument
	{
		public CustomArgument()
		{
			string customArgBlock = string.Empty;
			string[] args = Environment.GetCommandLineArgs();
			for (int i = 1; i < args.Length; i++)
			{
				if (args[i - 1] == "-CustomArgs")
				{
					customArgBlock = args[i];
					break;
				}
			}

			if (!string.IsNullOrEmpty(customArgBlock))
			{
				string[] args2 = customArgBlock.Split(';');
				for (int i = 0; i < args2.Length; i++)
				{
					string[] items = args2[i].Split('=');
					if (items.Length == 2)
					{
						_argDictionary.Add(items[0], items[1]);
					}
					else
					{
						Console.Error.WriteLine(string.Format("-CustomArgs[{0}]: {1} format is wrong!", i.ToString(), args2[i]));
					}
				}
			}

			if (_argDictionary.Count > 0)
			{
				_isValid = true;
			}
		}

		public bool GetCustomArgumentBool(string key)
		{
			if (_isValid)
			{
				if (_argDictionary.ContainsKey(key))
				{
					return bool.Parse(_argDictionary[key]);
				}
			}
			return false;
		}

		public int GetCustomArgumentInt(string key)
		{
			if (_isValid)
			{
				if (_argDictionary.ContainsKey(key))
				{
					return int.Parse(_argDictionary[key]);
				}
			}
			return 0;
		}

		public float GetCustomArgumentFloat(string key)
		{
			if (_isValid)
			{
				if (_argDictionary.ContainsKey(key))
				{
					return float.Parse(_argDictionary[key]);
				}
			}
			return 0.0f;
		}

		public string GetCustomArgumentString(string key)
		{
			if (_isValid)
			{
				if (_argDictionary.ContainsKey(key))
				{
					return _argDictionary[key];
				}
			}
			return string.Empty;
		}

		public string[] GetCustomArgumentStringArray(string key)
		{
			if (_isValid)
			{
				if (_argDictionary.ContainsKey(key))
				{
					string[] items = _argDictionary[key].Split('|');
					return items;
				}
			}
			return new string[] { };
		}

		public override string ToString()
		{
			string description = string.Empty;
			var e = _argDictionary.GetEnumerator();
			int index = 0;
			while (e.MoveNext())
			{
				string kk = e.Current.Key;
				string vv = e.Current.Value;
				string item = string.Format("[Item({0}):<{1}, {2}>] ", index, kk, vv);
				index++;
				description += item;
			}

			return string.Format("CustomArgument: [IsValid={0}], [ArgDictionary.Count={1}], [ArgDictionary.Items={2}]",
								  IsValid.ToString(),
								  _argDictionary.Count.ToString(),
								  description);
		}

		public bool IsValid
		{
			get
			{
				return _isValid;
			}
		}

		private bool _isValid = false;

		private Dictionary<string, string> _argDictionary = new Dictionary<string, string>();
	}
}
