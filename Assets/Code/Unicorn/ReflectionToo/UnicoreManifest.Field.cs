
/********************************************************************
created:    2022-09-29
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Unicorn
{
    public partial class UnicornManifest
    {
        [Serializable]
        public class Settings
        {
            public string colorSpace = "Linear";
        }
        
        public Settings playerSettings = new();

        public string[] relativePaths = {};

        /// <summary>
        /// 控制美术的项目不能执行MakeAutoCode相关逻辑
        /// </summary>
        public bool makeAutoCode;
        public bool clearAutoCode;
		
        // public string spriteDirPath = "Assets/Art/UIResource/UI/Sprite"; // 存放碎图sprite的文件夹地址.
        // public string uiPrefabDirPath = "Assets/prefabs/ui"; // ui界面prefab的文件夹地址.
    }
}