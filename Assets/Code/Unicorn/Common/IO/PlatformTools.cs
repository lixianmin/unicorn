
/********************************************************************
created:    2016-01-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

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
			var platforms = new[]
			{
				TargetPlatform.Android
				, TargetPlatform.iOS
				, TargetPlatform.StandaloneWindows64
			};
			
			var srcFilePath = PathTools.GetExportPath(localPath);
			
			try
			{
				foreach (var platform in platforms)
				{
					var destFilePath = os.path.join(PathTools.GetExportResourceRoot(platform), localPath);
					if (srcFilePath != destFilePath)
					{
						File.Copy(srcFilePath, destFilePath, true);
					}
				}
			}
			catch (System.Exception ex)
			{
				Logo.Error(ex);
			}
		}
    }
}