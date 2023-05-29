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
//             argument.key ??= string.Empty;
//             _argument = argument;
//             CoroutineManager.Instance.StartCoroutine(_CoLoad(argument, handler));
//         }
//
//         private IEnumerator _CoLoad(WebArgument argument, Action<WebItem> handler)
//         {
//             var loadHandle = Addressables.LoadAssetAsync<UObject>(argument.key);
//             _loadHandle = loadHandle;
//
//             // LoadAssetAsync发生InvalidKeyException异常时，只会打印一条日志，不会真正的抛出异常
//             while (!loadHandle.IsDone)
//             {
//                 yield return null;
//             }
//             
//             // 无论加载是否成功，都需要回调到handler
//             IsDone = true;
//             IsSucceeded = loadHandle.Status == AsyncOperationStatus.Succeeded;
//             CallbackTools.Handle(ref handler, this, string.Empty);
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
//         private readonly WebArgument _argument;
//         private AsyncOperationHandle _loadHandle;
//     }
// }