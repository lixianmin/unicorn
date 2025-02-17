/********************************************************************
created:    2014-01-03
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;

namespace Unicorn
{
    public static partial class ExtendedTransform
    {
        public static void Destroy(this Transform my)
        {
            if (my != null)
            {
                UnityEngine.Object.Destroy(my.gameObject);
            }
        }

        public static Component DigComponent(this Transform father, string name, Type type)
        {
            if (null != type)
            {
                var trans = father.Dig(name);
                if (null != trans && type != typeof(Transform))
                {
                    var component = trans.GetComponent(type);
                    return component;
                }

                return trans;
            }

            return null;
        }

//        public static Transform DeepFindEx (this Transform father, string name)
//        {
//			if (null == father || null == name)
//			{
//				return null;
//			}
//
//			if (father.name == name)
//			{
//				return father;
//			}
//
//			_bfsQueue.PushBack(father);
//			while (_bfsQueue.Count > 0)
//			{
//				father = _bfsQueue.PopFront() as Transform;
//				var childCount = father.childCount;
//				if (childCount > 0)
//				{
//					var targetChild = father.Find (name);
//					if (null != targetChild)
//					{
//						_bfsQueue.Clear();
//						return targetChild;
//					}
//
//					for (int i= 0; i< childCount; ++i)
//					{
//						var child = father.GetChild(i);
//						_bfsQueue.PushBack(child);
//					}
//				}
//			}
//
//			return null;
//        }

        public static Transform Dig(this Transform father, string name)
        {
            if (null == father || null == name)
            {
                return null;
            }

            if (father.name == name)
            {
                return father;
            }

            var found = _InnerDeepFind(father, name);
            return found;
        }

        private static Transform _InnerDeepFind(Transform father, string name)
        {
            var targetChild = father.Find(name);
            if (null != targetChild)
            {
                return targetChild;
            }

            var childCount = father.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                var child = father.GetChild(i);
                var grandson = _InnerDeepFind(child, name);
                if (null != grandson)
                {
                    return grandson;
                }
            }

            return null;
        }

        public static string GetScenePath(this Transform transform)
        {
            if (null != transform)
            {
                var path = transform.name;
                while ((transform = transform.parent) != null)
                {
                    path = transform.name + "/" + path;
                }

                return path;
            }

            return string.Empty;
        }

        public static string GetFindPath(this Transform transform)
        {
            if (null != transform)
            {
                var root = transform.root;
                if (transform != root)
                {
                    var sbPath = StringBuilderPool.Get();
                    _CollectFindPath(transform, root, sbPath);
                    var path = StringBuilderPool.Return(sbPath).ToString();
                    return path;
                }
            }

            return string.Empty;
        }

        private static void _CollectFindPath(Transform transform, Transform root, System.Text.StringBuilder sbPath)
        {
            var parent = transform.parent;
            if (null != parent && parent != root)
            {
                _CollectFindPath(parent, root, sbPath);
                sbPath.Append("/");
            }

            sbPath.Append(transform.name);
        }

        internal static IEnumerable<Transform> EnumerateChildren(this Transform father)
        {
            if (null != father)
            {
                yield return father;

                var childCount = father.childCount;
                for (var i = 0; i < childCount; ++i)
                {
                    var child = father.GetChild(i);
                    foreach (var node in EnumerateChildren(child))
                    {
                        yield return node;
                    }
                }
            }
        }
    }
}