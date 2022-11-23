
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
            
            // managed stripping level：以High最狠，但目前只能支持到Low，因为到了Medium级别UI库中的反射就不能用了
            // https://docs.unity3d.com/ScriptReference/ManagedStrippingLevel.html
            public string managedStrippingLevel = "Low";
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