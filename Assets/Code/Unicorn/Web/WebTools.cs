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
                    _ReloadRendererShader(renderer);
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

            // Terrain
            // todo terrainData中的grass目前还无法处理shader, 没找到获取到相关material的接口
            var terrains = goAsset.GetComponentsInChildren<Terrain>(true);
            if (terrains != null)
            {
                foreach (var terrain in terrains)
                {
                    _ReloadMaterialShader(terrain.materialTemplate);
                }
            }
        }

        private static void _ReloadRendererShader(Renderer renderer)
        {
            if (renderer == null)
            {
                return;
            }
            
            var sharedMaterials = renderer.sharedMaterials;
            if (sharedMaterials == null)
            {
                return;
            }

            foreach (var material in sharedMaterials)
            {
                _ReloadMaterialShader(material);
            }
        }

        private static void _ReloadMaterialShader(Material material)
        {
            if (material == null)
            {
                // Logo.Info("material is null");
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
            var nextRenderQueue = material.renderQueue;

            // we can not change the renderQueue of font manually, because InputField.text may disappear.
            // renderQueue is changed when manually setting shader, and should be restored
            if (lastRenderQueue != nextRenderQueue && !shaderName.StartsWith("TextMeshPro/"))
            {
                material.renderQueue = lastRenderQueue;
                // Logo.Info($"material.name={material.name}, shader.name={shaderName}, last={lastRenderQueue}, next={nextRenderQueue}");
            }
        }

        internal static int GetNextId()
        {
            return ++_idGenerator;
        }

        private static int _idGenerator;
    }
}