
/********************************************************************
created:    2017-11-10
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Metadata
{
    internal class MetadataPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets (string[] importedAssets
            , string[] deletedAssets
            , string[] movedAssets
            , string[] movedFromAssetPaths)
        {
            _ProcessAssets (importedAssets);
        }

        private static void _ProcessAssets (string[] assetPaths)
        {
            var assetPathsLength = assetPaths.Length;
            if (assetPathsLength == 0)
            {
                return;
            }

            for (int i= 0; i< assetPaths.Length; ++i)
            {
                var assetPath = assetPaths[i];
                if (!_EndsWithEx(assetPath, ".cs"))
                {
                    continue;
                }

                var isFind = _IsMetadataFile(assetPath);
                if (isFind)
                {
                    Debug.LogFormat("Detected metadata code change: assetPath={0}", assetPath);
                    var builtFile = new Build.MetadataBuiltFile();
                    builtFile.Clear();
                    return;
                }
            }
        }

        private static bool _EndsWithEx (string text, string candidate)
        {
            if (null != text && null != candidate)
            {
                return _currentCompareInfo.IsSuffix(text, candidate, CompareOptions.OrdinalIgnoreCase);
            }

            return false;
        }

        private static bool _IsMetadataFile (string assetPath)
        {
            try
            {
                var text = File.ReadAllText(assetPath);
                if (_metadataPattern.IsMatch(text))
                {
                    return true;
                }
            }
            finally
            {

            }

            return false;
        }

        private static readonly Regex _metadataPattern = new Regex(@"namespace\s+Metadata");
        private static readonly CompareInfo _currentCompareInfo = CultureInfo.CurrentCulture.CompareInfo;
    }
}
