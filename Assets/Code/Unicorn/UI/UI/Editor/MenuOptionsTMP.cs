
/********************************************************************
created:    2022-08-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine.UI;
using TMP_DefaultControls = Unicorn.UI.Internal.TMP_DefaultControls;

namespace Unicorn.UI
{
    internal static class MenuOptionsTMP
    {
        [MenuItem(MenuRoot + "UI Text", false, 1800)]
        private static void CreateTextMeshProGuiObjectPerform(MenuCommand menuCommand)
        {
            GameObject go = TMP_DefaultControls.CreateText(GetStandardResources());

            // Override text color and font size
            UIText textComponent = go.GetComponent<UIText>();
            
            // var isWaitingOnResourceLoad = textComponent.m_isWaitingOnResourceLoad == false;
            var isWaitingOnResourceLoad = IsWaitingOnResourceLoad(textComponent);
            if (isWaitingOnResourceLoad == false)
            {
                // Get reference to potential Presets for <TextMeshProUGUI> component
                var presets = UnityEditor.Presets.Preset.GetDefaultPresetsForObject(textComponent);
            
                if (presets == null || presets.Length == 0)
                {
                    textComponent.fontSize = TMP_Settings.defaultFontSize;
                    textComponent.color = Color.white;
                    textComponent.text = "Hello Panda";
                }
            
                if (TMP_Settings.autoSizeTextContainer)
                {
                    Vector2 size = textComponent.GetPreferredValues(TMP_Math.FLOAT_MAX, TMP_Math.FLOAT_MAX);
                    textComponent.rectTransform.sizeDelta = size;
                }
                else
                {
                    textComponent.rectTransform.sizeDelta = TMP_Settings.defaultTextMeshProUITextContainerSize;
                }
            }
            else
            {
                textComponent.fontSize = -99;
                textComponent.color = Color.white;
                textComponent.text = "New Text";
            }

            PlaceUIElementRoot(go, menuCommand);
        }
        
        [MenuItem(MenuRoot+"/UI Button", false, 1830)]
        public static void AddButton(MenuCommand menuCommand)
        {
            var go = TMP_DefaultControls.CreateButton(GetStandardResources());

            // Override font size
            var textComponent = go.GetComponentInChildren<TMP_Text>();
            textComponent.fontSize = 24;
            textComponent.raycastTarget = false; // 设置raycastTarget=false，减少cpu成本

            PlaceUIElementRoot(go, menuCommand);
        }
        
        [MenuItem(MenuRoot+"UI Input Field", false, 1836)]
        static void AddTextMeshProInputField(MenuCommand menuCommand)
        {
            GameObject go = TMP_DefaultControls.CreateInputField(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }
        
        [MenuItem(MenuRoot+"UI Dropdown", false, 1835)]
        public static void AddDropdown(MenuCommand menuCommand)
        {
            GameObject go = TMP_DefaultControls.CreateDropdown(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        private static bool IsWaitingOnResourceLoad(UIText script)
        {
            var type = script.GetType();
            var fieldInfo = type.GetField("m_isWaitingOnResourceLoad", BindingFlags.NonPublic | BindingFlags.Instance);
            var value = (bool) fieldInfo?.GetValue(script)!;
            return value;
        }

        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            bool explicitParentChoice = true;
            if (parent == null)
            {
                parent = TMPro_CreateObjectMenu.GetOrCreateCanvasGameObject();
                explicitParentChoice = false;

                // If in Prefab Mode, Canvas has to be part of Prefab contents,
                // otherwise use Prefab root instead.
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if (prefabStage != null && !prefabStage.IsPartOfPrefabContents(parent))
                    parent = prefabStage.prefabContentsRoot;
            }

            if (parent.GetComponentsInParent<Canvas>(true).Length == 0)
            {
                // Create canvas under context GameObject,
                // and make that be the parent which UI element is added under.
                GameObject canvas = TMPro_CreateObjectMenu.CreateNewUI();
                Undo.SetTransformParent(canvas.transform, parent.transform, "");
                parent = canvas;
            }

            GameObjectUtility.EnsureUniqueNameForSibling(element);

            SetParentAndAlign(element, parent);
            if (!explicitParentChoice) // not a context click, so center in sceneview
                SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

            // This call ensure any change made to created Objects after they where registered will be part of the Undo.
            Undo.RegisterFullObjectHierarchyUndo(parent == null ? element : parent, "");

            // We have to fix up the undo name since the name of the object was only known after reparenting it.
            Undo.SetCurrentGroupName("Create " + element.name);

            Selection.activeGameObject = element;
        }
        
        private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            // Find the best scene view
            SceneView sceneView = SceneView.lastActiveSceneView;

            if (sceneView == null && SceneView.sceneViews.Count > 0)
                sceneView = SceneView.sceneViews[0] as SceneView;

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
                return;

            // Create world space Plane from canvas position.
            Camera camera = sceneView.camera;
            Vector3 position = Vector3.zero;
            Vector2 localPlanePosition;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2, camera.pixelHeight / 2), camera, out localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
                position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
                minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
                maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            itemTransform.anchoredPosition = position;
            itemTransform.localRotation = Quaternion.identity;
            itemTransform.localScale = Vector3.one;
        }

        
        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            Undo.SetTransformParent(child.transform, parent.transform, "");

            RectTransform rectTransform = child.transform as RectTransform;
            if (rectTransform)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                Vector3 localPosition = rectTransform.localPosition;
                localPosition.z = 0;
                rectTransform.localPosition = localPosition;
            }
            else
            {
                child.transform.localPosition = Vector3.zero;
            }
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;

            SetLayerRecursively(child, parent.layer);
        }
        
        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        
        private static TMP_DefaultControls.Resources GetStandardResources()
        {
            if (s_StandardResources.standard == null)
            {
                s_StandardResources.standard = AssetDatabase.GetBuiltinExtraResource<Sprite>(kStandardSpritePath);
                s_StandardResources.background = AssetDatabase.GetBuiltinExtraResource<Sprite>(kBackgroundSpritePath);
                s_StandardResources.inputField = AssetDatabase.GetBuiltinExtraResource<Sprite>(kInputFieldBackgroundPath);
                s_StandardResources.knob = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
                s_StandardResources.checkmark = AssetDatabase.GetBuiltinExtraResource<Sprite>(kCheckmarkPath);
                s_StandardResources.dropdown = AssetDatabase.GetBuiltinExtraResource<Sprite>(kDropdownArrowPath);
                s_StandardResources.mask = AssetDatabase.GetBuiltinExtraResource<Sprite>(kMaskPath);
            }
            return s_StandardResources;
        }
        
        private static TMP_DefaultControls.Resources s_StandardResources;

        private const string MenuRoot = "GameObject/UI/";
        
        private const string kStandardSpritePath = "UI/Skin/UISprite.psd";
        private const string kBackgroundSpritePath = "UI/Skin/Background.psd";
        private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
        private const string kKnobPath = "UI/Skin/Knob.psd";
        private const string kCheckmarkPath = "UI/Skin/Checkmark.psd";
        private const string kDropdownArrowPath = "UI/Skin/DropdownArrow.psd";
        private const string kMaskPath = "UI/Skin/UIMask.psd";
    }
}
