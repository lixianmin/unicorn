
/********************************************************************
created:    2022-08-13
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace Unicorn.Web
{
    public partial class WebPrefab
    {
        private static void _ProcessDependenciesInEditor (GameObject goAsset)
        {
			// 1. Currently all _partGroup are textures, fonts, atlases, so they do not need to reassgin shaders.
            // 2. We must use CollectDependencies(), because GetComponents() will not collect dependencies attached in script.
            
			if (!Application.isEditor ||  goAsset is null)
            {
                return;
            }
            
            ReassignShaders(goAsset);
        }
        
        internal static void ReassignShaders(GameObject goAsset)
        {
            if (!Application.isEditor)
            {
                return;
            }
            
            var renderers = goAsset.GetComponentsInChildren<Renderer>(true);
            if (renderers != null)
            {
                foreach (var renderer in renderers)
                {
                    var sharedMaterials = renderer.sharedMaterials;
                    if (sharedMaterials == null)
                    {
                        continue;
                    }

                    foreach (var material in sharedMaterials)
                    {
                        _ReassignMaterialShader(material);
                    }
                }
            }

            var graphics = goAsset.GetComponentsInChildren<Graphic>(true);
            if (graphics != null)
            {
                foreach (var graphic in graphics)
                {
                    _ReassignMaterialShader(graphic.material);
                    _ReassignMaterialShader(graphic.materialForRendering);
                }
            }
        }

        private static void _ReassignMaterialShader(Material material)
        {
            if (material?.shader != null)
            {
                material.shader = Shader.Find(material.shader.name);
            }
        }
    }
}