﻿
/********************************************************************
created:    2014-03-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;

namespace Unicorn
{
    public static class Constants
    {
        public static readonly string LogPath = Application.persistentDataPath + "/panda.log";
        public static readonly string LastLogPath = Application.persistentDataPath + "/last-panda.log";

        public const string LocalGFXActionDatabasePath = "gfxactiondatabase.raw";
        public const string LocalGFXActionDatabaseIncrementPath = "gfxactiondatabase@.raw";
        public const string LocalSkillDatabasePath = "skilldatabase.raw";
        public const string LocalSkillDatabaseIncrementPath = "skilldatabase@.raw";

        public const string LocalMetadataPath = "metadata.raw";
        public const string LocalIncrementMetadataPath = "metadata@.raw";
        public const string LocalLocaleTextPath = "locale.zh_cn.raw";

        public const string LocalGlobalResourcesPath = "globals" + BundleExtension;
        public const string GlobalResourcesDirectory = "Assets/GlobalResources";

        public const string LocalSharedDirectory = "prefabs/shared";
        public const string LocalSharedAtlasDirectory = LocalSharedDirectory + "/" + AtlasType;
        public const string AtlasType = "uiatlas";
        public const string BundleExtension = ".ab";    // file extension for AssetBundle files     

        internal const string BuiltinMappingPath = "mapping";
        public const string LocalApkDirectory = "assets/Raw";
        public static readonly string[] BuiltinFileExtensions = new string[] { BundleExtension, ".raw", ".pck", ".mp4", ".png" };

        public static readonly byte[] EmptyBytes = EmptyArray<byte>.Instance;

        public static class LotAsset
        {
            internal const string IncreamentBundleTail = "@.ab";            // extended AssetBundle file tail
            public const ushort Version = 0;
            public const string MetaName = "__LotAssetMeta"; // in increment bundle
        }

        public static class WebPrefab
        {
            // version changes:
            // 0->1 导出WebPrefab时,shared part那部分原本一个bool变量表示是否真正的导出了那个shared part
            //      ，新的设计中partsCount将是真正导出的个数
            public const ushort Version = 1;
            public const string MetaName = "850506";	// Today is 2015-05-06, happy birthday.
        }

        public static class Tag
        {
            public const string MayDiscard = "MayDiscard";
            public const string EmptyUITexture = "EmptyUITexture";
        }
    }
}