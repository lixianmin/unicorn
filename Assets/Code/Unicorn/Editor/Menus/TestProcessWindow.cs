
/********************************************************************
created:    2015-03-13
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System.Diagnostics;
using UnityEngine;
using UnityEditor;

namespace Unique.Menus
{
    internal class TestProcessWindow: EditorWindow
    {
        [MenuItem("*Metadata/Test Process", false, 602)]
        private static void _Execute()
        {
			EditorWindow.GetWindow<TestProcessWindow>(false, "Test Process");
        }

        private void OnEnable()
        {
            
        }
 
        private void OnGUI()
        {
			_LeaveSpace();
			_LeaveSpace();

			// using (new LayoutHorizontalScope(0))
			{
				GUILayout.Label("Process filename:", GUILayout.Width(_buttonWidth));
				_processFileName = GUILayout.TextField (_processFileName);
			}
			
			// using (new LayoutHorizontalScope(0))
			{
				GUILayout.Label("Process arguments:", GUILayout.Width(_buttonWidth));
				_processArguments = GUILayout.TextField(_processArguments);
			}
			
			if (GUILayout.Button("Test Process", GUILayout.Width(_buttonWidth)))
			{
				var process = new Process();
				var psi = process.StartInfo;
				
				psi.FileName = _processFileName;
				psi.Arguments= _processArguments;
				psi.UseShellExecute = true;
				
				var title = "Test Process";
				Console.WriteLine("[{0}] process start: {1} {2}", title, psi.FileName, psi.Arguments);
				process.Start ();
				process.WaitForExit();
				Console.WriteLine("[{0}] process exit: {1} {2}", title, psi.FileName, psi.Arguments);
			}
        }

        private void _LeaveSpace()
        {
            const float spacePixels = 10f;
            GUILayout.Space(spacePixels);
        }
        
		private const float _buttonWidth = 128.0f;

		private string _processFileName		= string.Empty;
		private string _processArguments	= string.Empty;
    }
}
