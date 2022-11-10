
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
            public bool bakeCollisionMeshes = true;
        }
        
        [Serializable]
        public class Paths
        {
            public string editorResourceRoot = string.Empty;
            public string xmlMetadataRoot = string.Empty;
            public string exportMetadataRoot = string.Empty;
        }
        
        public Settings editorSettings = new();

        public Paths relativePaths = new();

        /// <summary>
        /// 美术的项目不能执行MakeAutoCode, ClearAutoCode等自动生成代码的逻辑
        /// </summary>
        public bool makeAutoCode;
    }
}