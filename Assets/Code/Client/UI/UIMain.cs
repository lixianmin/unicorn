// /********************************************************************
// created:    2022-08-17
// author:     lixianmin
//
// Copyright (C) - All Rights Reserved
// *********************************************************************/
//
// using Unicorn.UI;
// using UnityEngine;
//
// namespace Client.UI
// {
//     public class UIMain : UIWindowBase
//     {
//         public override string GetAssetPath()
//         {
//             return "Assets/res/prefabs/ui/uimain.prefab";
//         }
//         
//         protected override void OnLoaded()
//         {
//             _btnOpenBag.UI.onClick.AddListener(() =>
//             {
//                 UIManager.Instance.OpenWindow(typeof(UIBag));
//             });
//             
//             _btnCloseBag.UI.onClick.AddListener(() =>
//             {
//                 UIManager.Instance.CloseWindow(typeof(UIBag));
//             });
//
//             // Logo.Info(_btnCloseBagObject.UI.localPosition);
//             Logo.Info("main is OnLoaded");
//             // UIManager.Instance.OpenWindow(GetType());
//             // UIManager.Instance.CloseWindow(GetType());
//         }
//
//         protected override void OnOpened()
//         {
//             Logo.Info("main is OnOpened");
//             // UIManager.Instance.OpenWindow(GetType());
//             // UIManager.Instance.CloseWindow(GetType());
//         }
//
//         protected override void OnActivated()
//         {
//             Logo.Info("main is OnActivated");
//             // UIManager.Instance.OpenWindow(GetType());
//             // UIManager.Instance.CloseWindow(GetType());
//         }
//
//         protected override void OnDeactivating()
//         {
//             Logo.Info("main is OnDeactivating, state={0}, foreground={1}", GetFetus().GetState(), UIManager.Instance.GetForegroundWindow(GetRenderQueue()));
//             // UIManager.Instance.OpenWindow(GetType());
//             // UIManager.Instance.CloseWindow(GetType());
//         }
//
//         protected override void OnClosing()
//         {
//             Logo.Info("main is OnClosing");
//             // UIManager.Instance.OpenWindow(GetType());
//             // UIManager.Instance.CloseWindow(GetType());
//         }
//
//         protected override void OnUnloading()
//         {
//             Logo.Info("main is OnUnloading");
//             UIManager.Instance.OpenWindow(GetType());
//             // UIManager.Instance.CloseWindow(GetType());
//         }
//
//         private readonly UIWidget<UIButton> _btnOpenBag = new( "btn_open_bag");
//         private readonly UIWidget<UIButton> _btnCloseBag = new( "btn_close_bag");
//         private readonly UIWidget<Transform> _btnCloseBagObject = new("btn_close_bag");
//     }
// }