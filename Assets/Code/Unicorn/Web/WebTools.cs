/********************************************************************
created:    2022-12-07
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Collections;
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
            CoroutineManager.It.StartCoroutine(_CoReloadRenderersShaders(renderers));

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

        private static IEnumerator _CoReloadRenderersShaders(Renderer[] renderers)
        {
            if (renderers != null)
            {
                // renderers.Length可能相当大, 比如3w的样子, 所以需要分帧执行一下
                for (var i = 0; i < renderers.Length; i++)
                {
                    if (i % 5000 == 0)
                    {
                        yield return null;
                    }

                    var renderer = renderers[i];
                    _ReloadRendererShader(renderer);
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

            // 会报错: Trying to set shader on a Material Variant.
            if (material.isVariant)
            {
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

            // we can't change the renderQueue of font manually, because InputField.text may disappear.
            // renderQueue is changed when manually setting shader, and should be restored
            if (lastRenderQueue != nextRenderQueue && !shaderName.StartsWith("TextMeshPro/"))
            {
                material.renderQueue = lastRenderQueue;
                // Logo.Info($"material.name={material.name}, shader.name={shaderName}, last={lastRenderQueue}, next={nextRenderQueue}");
            }
        }
    }
}