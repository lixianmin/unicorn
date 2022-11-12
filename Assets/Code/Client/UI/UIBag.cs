// /********************************************************************
// created:    2022-08-16
// author:     lixianmin
//
// Copyright (C) - All Rights Reserved
// *********************************************************************/
//
// using Unicorn.UI;
//
// namespace Client.UI
// {
//     public class UIBag: UIWindowBase
//     {
//         public override string GetAssetPath()
//         {
//             return "uibag";
//         }
//
//         public override RenderQueue GetRenderQueue()
//         {
//             return RenderQueue.Background;
//         }
//
//         protected override void OnLoaded()
//         {
//             _btnClickBag.UI.onClick.AddListener(() =>
//             {
//                 Console.WriteLine("bag button is clicked");
//             });
//             
//             Console.WriteLine("bag is OnLoaded");
//         }
//         
//         protected override void OnOpened()
//         {
//             Console.WriteLine("bag is OnOpened");
//         }
//
//         protected override void OnActivated()
//         {
//             Console.WriteLine("bag is OnActivated");
//         }
//
//         protected override void OnDeactivating()
//         {
//             Console.WriteLine("bag is OnDeactivating");
//         }
//
//         protected override void OnClosing()
//         {
//             Console.WriteLine("bag is OnClosing");
//         }
//
//         protected override void OnUnloading()
//         {
//             Console.WriteLine("bag is OnUnloading");
//         }
//
//         private readonly UIWidget<UIButton> _btnClickBag = new("btn_click_bag");
//     }
// }