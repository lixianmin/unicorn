﻿// /********************************************************************
// created:    2025-07-21
// author:     lixianmin
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// *********************************************************************/
//
// using System;
// using UnityEngine.Events;
//
// namespace Unicorn.UI
// {
//     public static class ExtendedButton
//     {
//         public static Action onClick(this UIWidget<UIButton> my, UnityAction fn)
//         {
//             if (my != null && fn != null)
//             {
//                 my.UI.onClick.AddListener(fn);
//                 return () => { my.UI.onClick.RemoveListener(fn); };
//             }
//
//             return null;
//         }
//     }
// }