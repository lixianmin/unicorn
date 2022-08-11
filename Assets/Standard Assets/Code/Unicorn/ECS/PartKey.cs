
// /********************************************************************
// created:    2018-03-15
// author:     lixianmin

// Copyright (C) - All Rights Reserved
// *********************************************************************/

// using System;
// using System.Collections.Generic;

// namespace Unicorn
// {
//     internal struct PartKey : IEquatable<PartKey>
//     {
//         public int typeIndex;
//         public int partId;

//         public static PartKey Create (int typeIndex, int partId)
//         {
//             return new PartKey { typeIndex = typeIndex, partId = partId };
//         }

//         public override int GetHashCode ()
//         {
//             unchecked
//             {
//                 return typeIndex + partId;
//             }
//         }

//         public override bool Equals (object other)        
//         {           
//             if (!(other is PartKey))
//             {
//                 return false;
//             }

//             return Equals((PartKey)other);        
//         }         

//         public bool Equals (PartKey other)        
//         {            
//             return typeIndex == other.typeIndex && partId == other.partId;        
//         }         

//         public static bool operator == (PartKey lhs, PartKey rhs)        
//         {            
//             return lhs.Equals(rhs);        
//         }         

//         public static bool operator != (PartKey lhs, PartKey rhs)        
//         {            
//             return !lhs.Equals(rhs);        
//         }    
//     }
// }