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
//         public override string GetResourcePath()
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
//             Console.WriteLine(_btnCloseBagObject.UI.localPosition);
//         }
//         
//         private readonly UIWidget<UIButton> _btnOpenBag = new( "btn_open_bag");
//         private readonly UIWidget<UIButton> _btnCloseBag = new( "btn_close_bag");
//         private readonly UIWidget<Transform> _btnCloseBagObject = new("btn_close_bag");
//     }
// }