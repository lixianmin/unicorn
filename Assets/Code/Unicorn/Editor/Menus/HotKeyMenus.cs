
/********************************************************************
created:    2022-09-10
author:     lixianmin

https://docs.unity3d.com/ScriptReference/MenuItem.html

Cmd     %
Ctrl	^	~~定义的时候, Ctrl必须在Alt键的前面, 也就是说^&这个顺序~~ 这个顺序可以定义, 但会出问题: Ctrl+Alt+A这个组合, 在只按下A的时候也会触发
Shift   #
Alt     &
英文字母 _字母

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEditor;

namespace Unicorn.Menus
{
	internal class HotKeyMenus
	{
		[MenuItem("Window/Asset Management/Addressables/Groups2 %&a", false, 0)]
		public static void OpenAddressables()
		{
			EditorApplication.ExecuteMenuItem("Window/Asset Management/Addressables/Groups");
		}
		
		[MenuItem("Window/Package Manager2 &%[", false, 1500)]
		public static void OpenPackageManager ()
		{
			EditorApplication.ExecuteMenuItem("Window/Package Manager");
		}
		
		[MenuItem("Edit/Project Settings2... %&s", false, 250)]
		public static void OpenSettings ()
		{
			EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
		}
	}
}