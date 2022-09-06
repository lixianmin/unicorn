
/********************************************************************
created:    2017-07-03
author:     lixianmin
            
Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Metadata
{
    [Serializable]
    public struct Int64_t
    #if UNITY_EDITOR
        : System.Xml.Serialization.IXmlSerializable
    #endif
    {
        public static implicit operator long (Int64_t self)
        {
            return self._value ^ _factor;
        }

        public static implicit operator Int64_t (long val)
        {
            return new Int64_t
            {
                _value = val ^ _factor,
            };
        }

        public override int GetHashCode ()
        {
            return _value.GetHashCode();
        }

        public bool Equals (Int64_t other)
        {
            return _value == other._value;
        }

        public override bool Equals (object other)
        {
            if (null == other || !(other is Int64_t))
            {
                return false;
            }

            return Equals((Int64_t) other);
        }

        public static bool operator == (Int64_t left, Int64_t right)
        {
            return left.Equals(right);
        }

        public static bool operator != (Int64_t left, Int64_t right)
        {
            return !(left == right);
        }

        public override string ToString ()
        {
            return $"_value={_value.ToString()}, result={(_value ^ _factor).ToString()}";
        }

        #if UNITY_EDITOR

        public System.Xml.Schema.XmlSchema GetSchema ()
        {
            return null;
        }

        public void WriteXml (System.Xml.XmlWriter writer)
        {
            var output = _value ^ _factor;
            writer.WriteValue (output);
        }

        public void ReadXml (System.Xml.XmlReader reader)
        {
            var content = reader.ReadElementContentAsLong();
            _value = content ^ _factor;
        }

        #endif

        private const long _factor = -84850506;
        private long _value;
    }
}