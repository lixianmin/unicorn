/********************************************************************
created:    2023-12-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using UnityEngine;

namespace Unicorn
{
    internal struct InstanceKey : IEquatable<InstanceKey>
    {
        public Mesh mesh;

        public Material material;


        public override bool Equals(object obj)
        {
            return obj is InstanceKey key && Equals(key);
        }

        public bool Equals(InstanceKey other)
        {
            return mesh == other.mesh && material == other.material;
        }

        public static bool operator==(InstanceKey lhs, InstanceKey rhs)
        {
            return lhs.Equals(rhs);
        }
        
        public static bool operator!=(InstanceKey lhs, InstanceKey rhs)
        {
            return !lhs.Equals(rhs);
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(mesh.GetHashCode(), material.GetHashCode());
        }
    }
}