
/********************************************************************
created:    2016-01-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.IO;

namespace Unicorn
{
    public static class PlatformTools
    {
		/// <summary>
		/// Dispatch current platform file to other platforms
		/// </summary>
		/// <param name="localPath">Local path.</param>
		public static void DispatchFile (string localPath)
		{
			var platforms = new TargetPlatform[]
			{
				TargetPlatform.Android
				, TargetPlatform.iPhone
			};
			
			var srcFilePath = PathTools.GetExportPath(localPath);
			
			try 
			{
				for (int i= 0; i< platforms.Length; ++i)
				{
					var destFilePath = os.path.join(PathTools.GetExportResourceRoot(platforms[i]), localPath);
					if (srcFilePath != destFilePath)
					{
						File.Copy(srcFilePath, destFilePath, true);
					}
				}
			}
			catch (System.Exception ex)
			{
				Console.Error.WriteLine(ex);
			}
		}
    }
}