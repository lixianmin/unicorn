
/********************************************************************
created:    2017-07-03
author:     lixianmin
            
Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Metadata
{
    [Serializable]
    public struct Int32_t
    #if UNITY_EDITOR
        : System.Xml.Serialization.IXmlSerializable
    #endif
    {
        public static implicit operator int (Int32_t self)
        {
            return self._value ^ _factor;
        }

        public static implicit operator Int32_t (int val)
        {
            return new Int32_t
            {
                _value = val ^ _factor,
            };
        }

        public override int GetHashCode ()
        {
            return _value.GetHashCode();
        }

        public bool Equals (Int32_t other)
        {
            return _value == other._value;
        }

        public override bool Equals (object other)
        {
            if (null == other || !(other is Int32_t))
            {
                return false;
            }

            return Equals((Int32_t) other);
        }

        public static bool operator == (Int32_t left, Int32_t right)
        {
            return left.Equals(right);
        }

        public static bool operator != (Int32_t left, Int32_t right)
        {
            return !(left == right);
        }

        public override string ToString ()
        {
            return string.Format("_value={0}, result={1}", _value.ToString(), (_value^_factor).ToString());
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
            var content = reader.ReadElementContentAsInt();
            _value = content ^ _factor;
        }

        #endif

        private const int _factor = -84850506;
        private int _value;
    }
}