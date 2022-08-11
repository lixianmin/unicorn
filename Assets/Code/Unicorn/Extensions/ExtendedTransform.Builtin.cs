
/********************************************************************
created:    2014-01-03
author:     lixianmin

Copyright (C) - All Rights Reserved
 *********************************************************************/
using UnityEngine;
using System.Text;

namespace Unicorn
{
    //[Obfuscators.ObfuscatorIgnore]
    public static partial class ExtendedTransform
    {
//        public static int GetChildCountEx(this Transform trans)
//        {
//            return trans.childCount;
//        }
//
//        public static void SetEulerAnglesEx(this Transform trans, Vector3 eulerAngles)
//        {
//            trans.eulerAngles = eulerAngles;
//        }
//
//        public static Vector3 GetEulerAnglesEx(this Transform trans)
//        {
//            return trans.eulerAngles;
//        }
//
//        public static void SetForwardEx(this Transform trans, Vector3 forward)
//        {
//            trans.forward = forward;
//        }
//
//        public static Vector3 GetForwardEx(this Transform trans)
//        {
//            return trans.forward;
//        }
//
//        public static void SetHasChangedEx(this Transform trans, bool hasChanged)
//        {
//            trans.hasChanged = hasChanged;
//        }
//
//        public static bool GetHasChangedEx(this Transform trans)
//        {
//            return trans.hasChanged;
//        }
//
//        public static void SetLocalEulerAnglesEx(this Transform trans, Vector3 localEulerAngles)
//        {
//            trans.localEulerAngles = localEulerAngles;
//        }
//
//        public static Vector3 GetLocalEulerAnglesEx(this Transform trans)
//        {
//            return trans.localEulerAngles;
//        }
//
//        public static void SetLocalPositionEx(this Transform trans, Vector3 localPosition)
//        {
//            trans.localPosition = localPosition;
//        }
//
//        public static Vector3 GetLocalPositionEx(this Transform trans)
//        {
//            return trans.localPosition;
//        }
//        
//        public static void SetLocalRotationEx(this Transform trans, Quaternion localRotation)
//        {
//            trans.localRotation = localRotation;
//        }
//
//        public static Quaternion GetLocalRotationEx(this Transform trans)
//        {
//            return trans.localRotation;
//        }
//
//        public static void SetLocalScaleEx(this Transform trans, Vector3 localScale)
//        {
//            trans.localScale = localScale;
//        }
//
//        public static Vector3 GetLocalScaleEx(this Transform trans)
//        {
//            return trans.localScale;
//        }
//
//        public static Matrix4x4 GetLocalToWorldMatrixEx(this Transform trans)
//        {
//            return trans.localToWorldMatrix;
//        }
//
//        public static Vector3 GetLossyScaleEx(this Transform trans)
//        {
//            return trans.lossyScale;
//        }
//
//        public static void SetParentEx(this Transform trans, Transform parent)
//        {
//            trans.parent = parent;
//        }
//
//        public static Transform GetParentEx(this Transform trans)
//        {
//            return trans.parent;
//        }
//
//        public static void SetPositionEx(this Transform trans, Vector3 position)
//        {
//            trans.position = position;
//        }
//
//        public static Vector3 GetPositionEx(this Transform trans)
//        {
//            return trans.position;
//        }
//       
//        public static void SetRightEx(this Transform trans, Vector3 right)
//        {
//            trans.right = right;
//        }
//
//        public static Vector3 GetRightEx(this Transform trans)
//        {
//            return trans.right;
//        }
//
//        public static Transform GetRootEx(this Transform trans)
//        {
//            return trans.root;
//        }
//     
//        public static void SetRotationEx(this Transform trans, Quaternion rotation)
//        {
//            trans.rotation = rotation;
//        }
//
//        public static Quaternion GetRotationEx(this Transform trans)
//        {
//            return trans.rotation;
//        }
//
//        public static void SetUpEx(this Transform trans, Vector3 up)
//        {
//            trans.up = up;
//        }
//
//        public static Vector3 GetUpEx(this Transform trans)
//        {
//            return trans.up;
//        }
//
//        public static Matrix4x4 GetWorldToLocalMatrixEx(this Transform trans)
//        {
//            return trans.worldToLocalMatrix;
//        }
//
//        public static void DetachChildrenEx(this Transform trans)
//        {
//            if (null != trans)
//            {
//                trans.DetachChildren();
//            }
//        }
//
//        public static Transform FindEx(this Transform trans, string name)
//        {
//            return trans.Find(name);
//        }
//
//        public static Transform GetChildEx(this Transform trans, int index)
//        {
//            return trans.GetChild(index);
//        }
//
//        public static Vector3 InverseTransformDirection(this Transform trans, Vector3 direction)
//        {
//            return trans.InverseTransformDirection(direction);
//        }
//
//        public static Vector3 InverseTransformDirection(this Transform trans, float x, float y, float z)
//        {
//            return trans.InverseTransformDirection(x, y, z);
//        }
//
//        public static Vector3 InverseTransformPoint(this Transform trans, Vector3 position)
//        {
//            return trans.InverseTransformPoint(position);
//        }
//
//        public static Vector3 InverseTransformPoint(this Transform trans, float x, float y, float z)
//        {
//            return trans.InverseTransformPoint(x, y, z);
//        }
//
//        public static bool IsChildOfEx(this Transform trans, Transform parent)
//        {
//            return trans.IsChildOf(parent);
//        }
//
//        public static void LookAtEx(this Transform trans, Transform target, Vector3 worldUp)
//        {
//            if (null != trans && null != target)
//            {
//                trans.LookAt(target, worldUp);
//            }
//        }
//
//        public static void LookAtEx(this Transform trans, Transform target)
//        {
//            if (null != trans && null != target)
//            {
//                trans.LookAt(target, Vector3.up);
//            }
//        }
//
//        public static void LookAtEx(this Transform trans, Vector3 worldPosition, Vector3 worldUp)
//        {
//            if (null != trans)
//            {
//                trans.LookAt(worldPosition, worldUp);
//            }
//        }
//
//        public static void LookAtEx(this Transform trans, Vector3 worldPosition)
//        {
//            if (null != trans)
//            {
//                trans.LookAt(worldPosition, Vector3.up);
//            }
//        }
//
//        public static void RotateEx(this Transform trans, Vector3 eulerAngles, Space relativeTo= Space.Self)
//        {
//            if (null != trans)
//            {
//                trans.Rotate(eulerAngles, relativeTo);
//            }
//        }
//
//        public static void RotateEx(this Transform trans, float xAngle, float yAngle, float zAngle, Space relativeTo= Space.Self)
//        {
//            if (null != trans)
//            {
//                trans.Rotate(xAngle, yAngle, zAngle, relativeTo);
//            }
//        }
//
//        public static void RotateEx(this Transform trans, Vector3 axis, float angle, Space relativeTo= Space.Self)
//        {
//            if (null != trans)
//            {
//                trans.Rotate(axis, angle, relativeTo);
//            }
//        }
//
//        public static void RotateAroundEx(this Transform trans, Vector3 point, Vector3 axis, float angle)
//        {
//            if (null != trans)
//            {
//                trans.RotateAround(point, axis, angle);
//            }
//        }
//
//        public static void TransformDirectionEx(this Transform trans, Vector3 direction)
//        {
//            if (null != trans)
//            {
//                trans.TransformDirection(direction);
//            }
//        }
//
//        public static void TransformDirectionEx(this Transform trans, float x, float y, float z)
//        {
//            if (null != trans)
//            {
//                trans.TransformDirection(x, y, z);
//            }
//        }
//
//        public static void TransformPointEx(this Transform trans, Vector3 position)
//        {
//            if (null != trans)
//            {
//                trans.TransformPoint(position);
//            }
//        }
//
//        public static void TransformPointEx(this Transform trans, float x, float y, float z)
//        {
//            if (null != trans)
//            {
//                trans.TransformPoint(x, y, z);
//            }
//        }
//
//        public static void TranslateEx(this Transform trans, Vector3 tranlation, Space relativeTo= Space.Self)
//        {
//            if (null != trans)
//            {
//                trans.Translate(tranlation, relativeTo);
//            }
//        }
//
//        public static void TranslateEx(this Transform trans, float x, float y, float z, Space relativeTo= Space.Self)
//        {
//            if (null != trans)
//            {
//                trans.Translate(x, y, z, relativeTo);
//            }
//        }
//
//        public static void TranslateEx(this Transform trans, Vector3 tranlation, Transform relativeTo)
//        {
//            if (null != trans && null != relativeTo)
//            {
//                trans.Translate(tranlation, relativeTo);
//            }
//        }
//
//        public static void TranslateEx(this Transform trans, float x, float y, float z, Transform relativeTo)
//        {
//            if (null != trans && null != relativeTo)
//            {
//                trans.Translate(x, y, z, relativeTo);
//            }
//        }
    }
}