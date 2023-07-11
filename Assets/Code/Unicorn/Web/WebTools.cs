/********************************************************************
created:    2022-12-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace Unicorn.Web
{
    public static class WebTools
    {
        public static void ReloadShaders(GameObject goAsset)
        {
            if (!Application.isEditor || goAsset == null)
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
                        _ReloadMaterialShader(material);
                    }
                }
            }

            // UI相关, 一定是界面上的, 而不是3D场景里的
            var graphics = goAsset.GetComponentsInChildren<Graphic>(true);
            if (graphics != null)
            {
                foreach (var graphic in graphics)
                {
                    _ReloadMaterialShader(graphic.material);
                    _ReloadMaterialShader(graphic.materialForRendering);
                }
            }
        }

        private static void _ReloadMaterialShader(Material material)
        {
            if (material == null)
            {
                Logo.Info("material is null");
                return;
            }

            var shader = material.shader;
            if (shader == null)
            {
                Logo.Error($"material.name={material.name}, shader is null");
                return;
            }

            var shaderName = shader.name;
            var lastRenderQueue = material.renderQueue;
            material.shader = Shader.Find(shaderName);
            material.renderQueue = lastRenderQueue; // renderQueue is changed when manually setting shader, and must be restored
            // var nextRenderQueue = material.renderQueue;
            // Logo.Info($"material.name={material.name}, shader.name={shaderName}, lastRenderQueue={lastRenderQueue}, nextRenderQueue={nextRenderQueue}");
        }

        internal static int GetNextId()
        {
            return ++_idGenerator;
        }

        private static int _idGenerator;
    }
}