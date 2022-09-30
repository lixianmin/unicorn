﻿
/********************************************************************
created:    2017-07-08
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UI;
using DefaultControls = Unicorn.UI.Internal.DefaultControls;

namespace Unicorn.UI
{
    internal static class MenuOptions
    {
        [MenuItem(MenuRoot + "Legacy/UI Text", false, 1800)]
        private static void _AddText (MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateText(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            var label = go.GetComponent<UIText1>();
            if (null != label)
            {
                label.raycastTarget = false;
                label.text = "panda";
            }
        }

        [MenuItem(MenuRoot + "UI Image", false, 1801)]
        private static void _AddImage (MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateImage(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            var image = go.GetComponent<UIImage>();
            if (null != image)
            {
                image.raycastTarget = false;
            }
        }

        [MenuItem(MenuRoot + "UI Raw Image", false, 1802)]
        private static void _AddRawImage (MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateRawImage(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);

            var rawImage = go.GetComponent<UIRawImage>();
            if (null != rawImage)
            {
                rawImage.raycastTarget = false;
            }
        }

        [MenuItem(MenuRoot + "Legacy/UI Button", false, 1830)]
        private static void _AddButton (MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateButton(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem(MenuRoot + "UI Toggle", false, 1831)]
        private static void _AddToggle (MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateToggle(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem(MenuRoot + "UI Slider", false, 1833)]
        private static void _AddSlider (MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateSlider(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem(MenuRoot + "UI Scrollbar", false, 1834)]
        private static void _AddScrollbar (MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateScrollbar(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem(MenuRoot + "Legacy/UI Dropdown", false, 1835)]
        private static void _AddDropdown(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateDropdown(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        [MenuItem(MenuRoot + "Legacy/UI Input Field", false, 1836)]
        private static void _AddInputField (MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateInputField(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

		[MenuItem("GameObject/UI/UI Canvas", false, 1860)]
		public static void AddCanvas (MenuCommand menuCommand)
		{
			var go = CreateNewUI();
			GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
			if (go.transform.parent as RectTransform)
			{
				RectTransform rect = go.transform as RectTransform;
				rect.anchorMin = Vector2.zero;
				rect.anchorMax = Vector2.one;
				rect.anchoredPosition = Vector2.zero;
				rect.sizeDelta = Vector2.zero;
			}
			Selection.activeGameObject = go;
		}
		
		[MenuItem("GameObject/UI/UI Panel", false, 1861)]
		public static void AddPanel (MenuCommand menuCommand)
		{
			GameObject go = DefaultControls.CreatePanel(GetStandardResources());
			PlaceUIElementRoot(go, menuCommand);
			
			// Panel is special, we need to ensure there's no padding after repositioning.
			RectTransform rect = go.GetComponent<RectTransform>();
			rect.anchoredPosition = Vector2.zero;
			rect.sizeDelta = Vector2.zero;
		}

        [MenuItem(MenuRoot + "UI Scroll View", false, 1862)]
        public static void AddScrollView(MenuCommand menuCommand)
        {
            GameObject go = DefaultControls.CreateScrollView(GetStandardResources());
            PlaceUIElementRoot(go, menuCommand);
        }

        // [MenuItem(MenuRoot + "----------", false, 1999)]
        // private static void _CreateSeparator (MenuCommand menuCommand)
        // {
        //     
        // }

        public static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null || parent.GetComponentInParent<Canvas>() == null)
            {
                parent = GetOrCreateCanvasGameObject();
            }

            string uniqueName = GameObjectUtility.GetUniqueNameForSibling(parent.transform, element.name);
            element.name = uniqueName;
            Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
            Undo.SetTransformParent(element.transform, parent.transform, "Parent " + element.name);
            GameObjectUtility.SetParentAndAlign(element, parent);
            if (parent != menuCommand.context) // not a context click, so center in sceneview
                SetPositionVisibleinSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

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
            Vector2 localPlanePosition;
            Camera camera = sceneView.camera;
            Vector3 position = Vector3.zero;
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

        // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
        public static GameObject GetOrCreateCanvasGameObject()
        {
            GameObject selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in selection or its parents? Then use just any canvas..
            canvas = Object.FindObjectOfType(typeof(Canvas)) as Canvas;
            if (canvas != null && canvas.gameObject.activeInHierarchy)
                return canvas.gameObject;

            // No canvas in the scene at all? Then create a new one.
            return MenuOptions.CreateNewUI();
        }

        public static GameObject CreateNewUI()
        {
            // Root for the UI
            var root = new GameObject("Canvas");
            root.layer = LayerMask.NameToLayer(kUILayerName);
            Canvas canvas = root.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            root.AddComponent<CanvasScaler>();
            root.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);

            // if there is no event system add one...
            CreateEventSystem(false);
            return root;
        }

        private static void CreateEventSystem (bool select)
        {
            CreateEventSystem(select, null);
        }

        private static void CreateEventSystem (bool select, GameObject parent)
        {
            var esys = Object.FindObjectOfType<EventSystem>();
            if (esys == null)
            {
                var eventSystem = new GameObject("EventSystem");
                GameObjectUtility.SetParentAndAlign(eventSystem, parent);
                esys = eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && esys != null)
            {
                Selection.activeGameObject = esys.gameObject;
            }
        }

        private static DefaultControls.Resources s_StandardResources;
        private static DefaultControls.Resources GetStandardResources ()
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

        private const string kUILayerName = "UI";

        private const string kStandardSpritePath       = "UI/Skin/UISprite.psd";
        private const string kBackgroundSpritePath     = "UI/Skin/Background.psd";
        private const string kInputFieldBackgroundPath = "UI/Skin/InputFieldBackground.psd";
        private const string kKnobPath                 = "UI/Skin/Knob.psd";
        private const string kCheckmarkPath            = "UI/Skin/Checkmark.psd";
        private const string kDropdownArrowPath        = "UI/Skin/DropdownArrow.psd";
        private const string kMaskPath                 = "UI/Skin/UIMask.psd";

        public const string MenuRoot = "GameObject/UI/";
    }
}
