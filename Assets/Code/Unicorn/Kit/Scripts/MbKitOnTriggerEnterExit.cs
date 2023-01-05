// /*********************************************************************
// created:    2023-01-04
// author:     lixianmin
//
// Copyright (C) - All Rights Reserved
// *********************************************************************/
//
// using System;
// using UnityEngine;
//
// namespace Unicorn.Kit
// {
//     internal class MbKitOnTriggerEnterExit : MonoBehaviour
//     {
//         private void OnTriggerEnter(Collider other)
//         {
//             if (kit is IOnTriggerEnter item)
//             {
//                 try
//                 {
//                     item.OnTriggerEnter(other);
//                 }
//                 catch (Exception ex)
//                 {
//                     Console.Error.WriteLine($"[OnTriggerEnter()] ex= {ex},\n\n StackTrace={ex.StackTrace}");
//                 }
//             }
//         }
//         
//         private void OnTriggerExit(Collider other)
//         {
//             if (kit is IOnTriggerExit item)
//             {
//                 try
//                 {
//                     item.OnTriggerExit(other);
//                 }
//                 catch (Exception ex)
//                 {
//                     Console.Error.WriteLine($"[OnTriggerEnter()] ex= {ex},\n\n StackTrace={ex.StackTrace}");
//                 }
//             }
//         }
//
//         public KitBase kit;
//     }
// }