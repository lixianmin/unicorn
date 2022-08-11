
/********************************************************************
created:    2017-06-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Metadata
{
    [System.Serializable]
    public struct VInt3
    {
        public VInt3 (int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString ()
        {
            return string.Format("[x={0}, y={1}, z={1}]", x.ToString(), y.ToString(), z.ToString());
        }

        public override int GetHashCode ()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        }

        public bool Equals (VInt3 other)
        {
            return x == other.x && y == other.y && z == other.z;            
        }

        public override bool Equals (object other)
        {
            if (null == other || !(other is VInt3))
            {
                return false;
            }

            return Equals((VInt3) other);
        }

        public static bool operator == (VInt3 left, VInt3 right)
        {
            return left.Equals(right);
        }

        public static bool operator != (VInt3 left, VInt3 right)
        {
            return !(left == right);
        }

        public static VInt3 zero
        {
            get
            {
                return new VInt3(0, 0, 0);
            }
        }

        public int x;
        public int y;
        public int z;
    }
}