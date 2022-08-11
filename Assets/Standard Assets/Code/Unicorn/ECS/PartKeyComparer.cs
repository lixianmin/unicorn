
// /********************************************************************
// created:    2018-03-15
// author:     lixianmin

// Copyright (C) - All Rights Reserved
// *********************************************************************/

// using System;
// using System.Collections.Generic;

// namespace Unicorn
// {
//     internal class PartKeyComparer : IComparer<PartKey>
//     {
//         static PartKeyComparer ()
//         {

//         }

//         private PartKeyComparer ()
//         {

//         }

//         public int Compare (PartKey a, PartKey b)
//         {
//             if (a.typeIndex < b.typeIndex)
//             {
//                 return -1;
//             }
//             else if (a.typeIndex > b.typeIndex)
//             {
//                 return 1;
//             }
//             else if (a.partId < b.partId)
//             {
//                 return -1;
//             }
//             else if (a.partId > b.partId)
//             {
//                 return 1;
//             }
//             else
//             {
//                 return 0;
//             }
//         }

//         public static readonly PartKeyComparer Instance = new PartKeyComparer();
//     }
// }