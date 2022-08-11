
// /********************************************************************
// created:    2018-03-15
// author:     lixianmin

// Copyright (C) - All Rights Reserved
// *********************************************************************/

// using System;
// using Unicorn.Collections;

// namespace Unicorn
// {
//     internal class PartTable : SortedTable<PartKey, Part>
//     {
//         public PartTable () : base(PartKeyComparer.Instance)
//         {

//         }

//         public int GetVersion ()
//         {
//             return _version;
//         }

//         public struct Snapshot
//         {
//             public void Take (PartTable table)
//             {
//                 if (null != table && _version != table._version)
//                 {
//                     var capacity = table._capacity;
//                     var values = _values;
//                     if (null == values || values.Length != capacity)
//                     {
//                         _values = new Part[capacity];
//                     }

//                     Array.Copy(table._values, 0, _values, 0, capacity);
//                     _version = table._version;
//                     _size = table._size;
//                 }
//             }

//             public int GetVersion ()
//             {
//                 return _version;
//             }

//             public Part this [int index]
//             {
//                 get
//                 {
//                     var result = _values[index];
//                     return result;
//                 }
//             }

//             public int Count { get { return _size; } }

//             private Part[] _values;
//             private int _size;
//             private int _version;
//         }
//     }
// }