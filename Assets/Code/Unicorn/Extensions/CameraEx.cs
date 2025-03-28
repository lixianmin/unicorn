//
// /********************************************************************
// created:    2016-06-03
// author:     lixianmin
//
// Copyright (C) - All Rights Reserved
// *********************************************************************/
// using UnityEngine;
// using System;
//
// namespace Unicorn
// {
//     //[Obfuscators.ObfuscatorIgnore]
//     public static class CameraEx
//     {
// 		public static Texture2D SnapshotEx (this Camera camera, int width, int height)
// 		{
// 			if (null != camera && width > 0 && height > 0)
// 			{
// 				var rttTemp = RenderTexture.GetTemporary(width, height, 24);
// 				camera.targetTexture = rttTemp;
// 				camera.Render ();
// 				camera.targetTexture = null;
// 				
// 				var screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
// 				RenderTexture.active = rttTemp;
// 				screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
// 				screenShot.Apply();
// 				RenderTexture.active = null;
// 				
// 				RenderTexture.ReleaseTemporary(rttTemp);
// 				return screenShot;
// 			}
//
// 			return null;
// 		}
//     }
// }