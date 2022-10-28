
/********************************************************************
created:    2022-08-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.IO;
using System.Collections;
using UnityEngine;
using Unicorn;
using Metadata;

//using Unicorn.Web;

public class MbUnicornGame : MonoBehaviour
{
	class GameMetadataManager : MetadataManager
	{
		public static readonly GameMetadataManager Instance = new();
	}

	private void Start()
	{
		// 避免Game对象在场景切换的时候被干掉
		GameObject.DontDestroyOnLoad(gameObject);

		UnicornMain.Instance.Init();
		// CoroutineManager.StartCoroutine(_CoLoadMetadata());


		//DependencyManager.Instance.Init();
		// AssetManager.Instance.RequestAsset("globals.ab", "", (assetReference)=>{
		// 	Debug.LogError("assetReference : " + assetReference);
		// 	Debug.LogError("assetReference.assetBundle : " + assetReference.assetBundle);
		// });
		//WebManager.SetBaseUrl(PathTools.DefaultBaseUrl);
		// AssetManager.Instance.RequestGameObject("prefabs/ui/_battle.ab", "uibattle_stage", (go)=>
		// {
		// 	Console.Error.WriteLine("go : " + go);
		// });		

		// AssetManager.Instance.RequestAsset("prefabs/atlas/_chapter.ab", "chapter_checkbox_b05_background", (obj)=>{
		// 	assetReference = obj;
		// 	Console.Error.WriteLine("obj : " + obj + " " + assetReference.asset);
		// 	assetReference.Dispose();
		// });
		//WebManager.LoadWebPrefab("prefabs/ui/_battle.ab/uibattle_stage", (webItem) =>
		//{
		//	Console.Error.WriteLine("webItem : " + webItem + " " + webItem.mainAsset);
		//	webItem.CloneMainAsset();
		//	// GameObject.Instantiate(webItem.mainAsset);
		//});
	}

	// Update is called once per frame
	private void Update()
	{
		//AssetManager.Instance.Tick();
		UnicornMain.Instance.ExpensiveUpdate(Time.deltaTime);
		UnicornMain.Instance.SlowUpdate(Time.deltaTime);
	}

	private IEnumerator _CoLoadMetadata()
	{
		var metadataManager = MetadataManager.Instance;
		var fullpath = PathTools.GetFullPath("metadata.raw");
		if (!string.IsNullOrEmpty(fullpath))
		{
			try
			{
				var stream = File.OpenRead(fullpath);
				metadataManager.LoadRawStream(stream);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine("[_CoLoadMetadata()] load metadata failed, ex={0}", ex.ToString());
			}
		}


		//var table = metadataManager.GetTemplateTable(typeof(PetEatFishTemplate));
		//foreach (var template in table.Values)
		//{
		//	Console.WriteLine("template={0}", template);
		//}

		var version = metadataManager.GetMetadataVersion();
		Console.WriteLine("[_CoLoadMetadata()] Metadata Loaded, metadataVersion={0}.", version.ToString());
		yield return null;

		// UIManager.OpenWindow(typeof(Client.UI.UIMain));
	}
}
