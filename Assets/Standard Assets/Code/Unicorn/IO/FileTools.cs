
/********************************************************************
created:    2015-03-04
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;

namespace Unicorn.IO
{
	public static class FileTools
	{
		public static void WriteAllTextSafely(string path, string contents)
		{
			_WriteAllSafely(path, contents, File.WriteAllText);
		}

		public static void WriteAllLinesSafely(string path, string[] contents)
		{
			_WriteAllSafely(path, contents, File.WriteAllLines);
		}

		public static void WriteAllBytesSafely(string path, byte[] bytes)
		{
			_WriteAllSafely(path, bytes, File.WriteAllBytes);
		}

		private static void _WriteAllSafely<T>(string path, T contents, Action<string, T> writeFunction)
		{
			if (string.IsNullOrEmpty(path))
			{
				return;
			}

			var existence = File.Exists(path);
			if (!existence)
			{
				var dirPath = Path.GetDirectoryName(path);
				Kernel.MakeDirs(dirPath);
			}

			var tempFileName = path + ".tmp.0506";
			writeFunction(tempFileName, contents);

			if (existence)
			{
				File.Delete(path);
			}

			File.Move(tempFileName, path);
		}

		public static void Overwrite(string sourceFileName, string destFileName)
		{
			if (string.IsNullOrEmpty(sourceFileName) || string.IsNullOrEmpty(destFileName))
			{
				return;
			}

			DeleteSafely(destFileName);
			File.Move(sourceFileName, destFileName);
		}

		public static string ShowTempFile(string fname, string contents)
		{
			fname ??= "temp-file-name";
			contents ??= string.Empty;

			var directory = Path.GetTempPath() + "/temp-files";
			Kernel.MakeDirs(directory);

			var filepath = directory + "/" + fname;

			File.WriteAllText(filepath, contents);
			Kernel.StartFile(filepath, null, true);

			return filepath;
		}

		public static string GetHexDigest16(string path, bool checkFileExistence = true)
		{
			if (!string.IsNullOrEmpty(path))
			{
				try
				{
					if (!checkFileExistence || File.Exists(path))
					{
						var md5 = Md5sum.It.GetHexDigest16(File.ReadAllBytes(path));
						return md5;
					}
				}
				catch (Exception)
				{
					// ignored
				}
			}

			return string.Empty;
		}

		public static string[] GetHexDigest16(string[] paths, bool checkFileExistence = true)
		{
			if (null == paths)
			{
				return Array.Empty<string>();
			}

			var count = paths.Length;
			if (count == 0)
			{
				return Array.Empty<string>();
			}

			var digests = new string[count];
			for (int i = 0; i < count; ++i)
			{
				digests[i] = GetHexDigest16(paths[i], checkFileExistence);
			}

			return digests;
		}

		public static bool DeleteSafely(string path)
		{
			if (!string.IsNullOrEmpty(path) && File.Exists(path))
			{
				try
				{
					File.Delete(path);
					return true;
				}
				catch
				{
					// ignored
				}
			}

			return false;
		}
	}
}