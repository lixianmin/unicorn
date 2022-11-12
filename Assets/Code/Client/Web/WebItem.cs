//
// /********************************************************************
// created:    2022-08-12
// author:     lixianmin
//
// Copyright (C) - All Rights Reserved
// *********************************************************************/
//
// using System;
// using System.Collections;
// using Unicorn;
// using Unicorn.Web;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;
//
// namespace Client.Web
// {
//     using UObject = UnityEngine.Object;
//
//     public class WebItem : Disposable, IWebNode
//     {
//         internal WebItem(WebArgument argument, Action<WebItem> handler)
//         {
//             _argument = argument;
//             CoroutineManager.Instance.StartCoroutine(_CoLoad(argument, handler));
//         }
//
//         private IEnumerator _CoLoad(WebArgument argument, Action<WebItem> handler)
//         {
//             var loadHandle = Addressables.LoadAssetAsync<UObject>(argument.key);
//             _loadHandle = loadHandle;
//
//             while (!loadHandle.IsDone)
//             {
//                 yield return null;
//             }
//
//             IsDone = true;
//
//             if (loadHandle.Status == AsyncOperationStatus.Succeeded)
//             {
//                 IsSucceeded = true;
//                 CallbackTools.Handle(ref handler, this, string.Empty);
//             }
//         }
//
//         protected override void _DoDispose(bool isManualDisposing)
//         {
//             Addressables.Release(_loadHandle);
//             // Console.WriteLine("[_DoDispose()] {0}", this.ToString());
//         }
//         
//         // public override string ToString()
//         // {
//         //     return $"WebItem: id={_id.ToString()}, localPath={_argument.key}";
//         // }
//
//         public bool IsDone      { get; private set; }
//         public bool IsSucceeded { get; private set; }
//         public string Key => _argument.key;
//
//         public UObject Asset
//         {
//             get
//             {
//                 if (IsSucceeded)
//                 {
//                     return _loadHandle.Result as UObject;
//                 }
//
//                 return null;
//             }
//         }
//
//         private WebArgument _argument;
//         private AsyncOperationHandle _loadHandle;
//     }
// }