//
///********************************************************************
//created:    2014-01-13
//author:     lixianmin
//
//Copyright (C) - All Rights Reserved
//*********************************************************************/
//
//using UnityEditor;
//using System.IO;
//using System.Xml;
//using Unicorn;
//
//namespace Metadata.Menus
//{
//    class GenerateGUID
//    {
//		[MenuItem(MetaCommon.MenuRoot + "Generate GUID", false, 106)]
//        public static void Generate ()
//        {
//			var directory = UnicornManifest.GetMetadataRoot();
//
//			var title = "Scanning metadata files...";
//			var xmlPaths = os.walk(directory, "*.xml");
//			ScanTools.ScanAll(title, xmlPaths, _OnScanFile);
//        }
//
//		private static void _OnScanFile (string path)
//		{
//			var doc = new XmlDocument();
//			doc.Load(path);
//			var isChanged = _ReadXml(doc.DocumentElement.ChildNodes, doc);
//
//			if (isChanged)
//			{
//				doc.Save(path);
//			}
//		}
//
//		private static bool _ReadXml (XmlNodeList nodes, XmlDocument doc)
//		{
//			var isChanged = false;
//
//			foreach (XmlNode node in nodes)
//			{					
//				var attributes = node.Attributes;
//				var i18n = null != attributes && null != attributes["i18n"];
//
//				if (i18n)
//				{
//					var guidName = "guid";
//					var guid = attributes[guidName];
//
//					if (null == guid || string.IsNullOrEmpty(guid.Value))
//					{
//						var item = doc.CreateNode(XmlNodeType.Attribute, guidName, null);
//						item.Value = System.Guid.NewGuid().ToString();
//						attributes.SetNamedItem(item);
//
//						isChanged = true;
//					}
//				}
//				else
//				{
//					if (_ReadXml (node.ChildNodes, doc))
//					{
//						isChanged = true;
//					}
//				}
//			}
//
//			return isChanged;
//		}
//    }
//}