/********************************************************************
created:    2015-10-23
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Metadata
{
    partial class LoadAid
    {
        [Flags]
        internal enum NodeFlags : byte
        {
            None = 0,
            Increment = 1,
        }

//		public struct NodeKey
//		{
//			internal static NodeKey CreateMin (ushort metadataType)
//			{
//				var key = new NodeKey { type = metadataType, id = int.MinValue };
//				return key;
//			}
//
//			public override string ToString ()
//			{
//				return string.Format ("[NodeKey type={0}, id={1}]", type.ToString(), id.ToString());
//			}
//
//			public ushort type;		// metadataType
//			public int id;			// template id
//		}

        public struct NodeValue
        {
            internal int offset;
            internal NodeFlags flags;
        }

//		private class NodeKeyComparer : IComparer<NodeKey>
//		{
//			public int Compare (NodeKey a, NodeKey b)
//			{
//				if (a.type < b.type)
//				{
//					return -1;
//				}
//				else if (a.type > b.type)
//				{
//					return 1;
//				}
//				else if (a.id < b.id)
//				{
//					return -1;
//				}
//				else if (a.id > b.id)
//				{
//					return 1;
//				}
//				else
//				{
//					return 0;
//				}
//
//                // 下面这个办法数学原理上没有问题，问题出在减法可能会导致整数溢出.
////				var delta = a.type - b.type;
////				if (delta == 0)
////				{
////					delta = a.id - b.id;
////				}
////
////				return delta;
//			}
//		}
//
//		public class NodeTable : SortedTable<NodeKey, NodeValue>
//		{
//			internal NodeTable (int capacity) : base (capacity, new NodeKeyComparer())
//			{
//
//			}
//
//			internal void Merge (NodeTable other)
//			{
//				if (null == other || other.Count == 0)
//				{
//					return;
//				}
//
//				var counter1 = Count;
//				var counter2 = other.Count;
//				var capacity = counter1 + counter2;
//
//				var keys = new NodeKey[capacity];
//				var values = new NodeValue[capacity];
//
//				int currentIndex = 0;
//				int index1 = 0;
//				int index2 = 0;
//
//				while (index1 < counter1 && index2 < counter2)
//				{
//					var key1 = _keys[index1];
//					var key2 = other._keys[index2];
//
//					var flags = _comparer.Compare(key1, key2);
//					if (flags < 0)
//					{
//						keys[currentIndex] = key1;
//						values[currentIndex] = _values[index1];
//						++index1;
//					}
//					else if (flags > 0)
//					{
//						keys[currentIndex] = key2;
//						values[currentIndex] = other._values[index2];
//						++index2;
//					}
//					else
//					{
//						keys[currentIndex] = key2;
//						values[currentIndex] = other._values[index2];
//						++index2;
//						++index1;
//					}
//
//					++currentIndex;
//				}
//
//				while (index1 < counter1)
//				{
//					keys[currentIndex] = _keys[index1];
//					values[currentIndex] = _values[index1];
//					++index1;
//					++currentIndex;
//				}
//
//				while (index2 < counter2)
//				{
//					keys[currentIndex] = other._keys[index2];
//					values[currentIndex] = other._values[index2];
//					++index2;
//					++currentIndex;
//				}
//
//				_keys = keys;
//				_values = values;
//				_size = currentIndex;
//				_capacity = capacity;
//				++_version;
//			}
//		}
    }
}