﻿
/********************************************************************
created:    2022-08-12
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using Client.Web;
using UnityEngine;
using Unicorn;
using Metadata;
using Unicorn.UI;

public class MbUnicornGame : MonoBehaviour
{
	class GameMetadataManager : MetadataManager
	{
		public new static readonly GameMetadataManager It = new();
	}

	private void Start()
	{
		// 避免Game对象在场景切换的时候被干掉
		GameObject.DontDestroyOnLoad(gameObject);

		UnicornMain.It.Init();

		UIManager.It.OpenWindow(typeof(Client.UI.UIMain));
	}

	// Update is called once per frame
	private void Update()
	{
		UnicornMain.It.ExpensiveUpdate(Time.deltaTime);
		UnicornMain.It.SlowUpdate(Time.deltaTime);
	}
	
	private readonly GameWebManager _webManager = GameWebManager.It;  // 初始化基类中的Instance引用
}
