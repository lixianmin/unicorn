
/********************************************************************
created:    2017-07-03
author:     lixianmin
            
Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Metadata
{
    [Serializable]
    public struct Float_t
    #if UNITY_EDITOR
        : System.Xml.Serialization.IXmlSerializable
    #endif
    {
        public static implicit operator float (Float_t self)
        {
            unsafe
            {
                int intValue = self._value ^ _factor;
                return *(float*)&intValue;
            }
        }

        public static implicit operator Float_t (float val)
        {
            var result = new Float_t();
            unsafe
            {
                int intValue = *(int*)& val;
                result._value = intValue ^ _factor;
            }

            return result;
        }

        public override int GetHashCode ()
        {
            return _value.GetHashCode();
        }

        public bool Equals (Float_t other)
        {
            const float eps = 0.00001f;
            var delta = _value - other._value;
            return delta < eps && delta > -eps;
        }

        public override bool Equals (object other)
        {
            if (null == other || !(other is Float_t))
            {
                return false;
            }

            return Equals((Float_t) other);
        }

        public static bool operator == (Float_t left, Float_t right)
        {
            return left.Equals(right);
        }

        public static bool operator != (Float_t left, Float_t right)
        {
            return !(left == right);
        }

        public override string ToString ()
        {
            return string.Format("_value={0}, result={1}", _value.ToString(), ((float)this).ToString());
        }

        #if UNITY_EDITOR

        public System.Xml.Schema.XmlSchema GetSchema ()
        {
            return null;
        }

        public void WriteXml (System.Xml.XmlWriter writer)
        {
            var output = (float) this;
            writer.WriteValue (output);
        }

        public void ReadXml (System.Xml.XmlReader reader)
        {
            var content = (Float_t) reader.ReadElementContentAsFloat();
            _value = content._value;
        }

        #endif

        private const int _factor = -84850506;
        private int _value;
    }
}