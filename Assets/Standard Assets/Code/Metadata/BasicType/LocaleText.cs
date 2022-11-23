
/********************************************************************
created:    2014-03-20
author:     lixianmin
description:

    used for localization, all text that may need to be translated
    should be defined as LocaleText.
            
Copyright (C) - All Rights Reserved
*********************************************************************/

using System;

namespace Metadata
{
    // todo 这里UNITY_EDITOR中的System.Xml.Serialization.IXmlSerializable有可能是引入System.Xml.dll的原因
    [Serializable]
    public struct LocaleText
    #if UNITY_EDITOR
        : System.Xml.Serialization.IXmlSerializable
    #endif
    {
        public string   guid;
        public string   text;

        public override string ToString ()
        {
            return text ?? string.Empty;
        }

        public string Format (object arg0)
        {
            string ret = text;
            try
            {
                ret = string.Format(text, arg0);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("{0}. text = [{1}], arg0 = [{2}]", e.Message, text, arg0);
            }

            return ret;
        }

        public string Format (object arg0, object arg1)
        {
            string ret = text;
            try
            {
                ret = string.Format(text, arg0, arg1);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("{0}. text = [{1}], arg0 = [{2}], arg1 = [{3}]", e.Message, text, arg0, arg1);
            }

            return ret;
        }

        public string Format (object arg0, object arg1, object arg2)
        {
            string ret = text;
            try
            {
                ret = string.Format(text, arg0, arg1, arg2);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("{0}. text = [{1}], arg0 = [{2}], arg1 = [{3}], arg2 = [{4}]", e.Message, text, arg0, arg1, arg2);
            }

            return ret;
        }

        public string Format (params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return text;
            }

            string ret = text;

            try
            {
                ret = string.Format(text, args);
            }
            catch (Exception e)
            {
                string argsDebugText = string.Empty;

                for (int i = 0; i < args.Length; ++i)
                {
                    argsDebugText += args[i].ToString();
                }

                Console.Error.WriteLine("{0}. text = [{1}], argLength = {2}, args = [{3}], arg2 = [{4}]", e.Message, text, args.Length, argsDebugText);
            }

            return ret;
        }

        public static implicit operator string (LocaleText self)
        {
            return self.text ?? string.Empty;
        }

		public override int GetHashCode ()
		{
			if (null != text)
			{
				return text.GetHashCode();
			}
			
			return base.GetHashCode();
		}
		
		public bool Equals (LocaleText other)
		{
			if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(other.text))
			{
				return true;
			}

			return text == other.text;
		}
		
		public override bool Equals (object other)
		{
			if (null == other || !(other is LocaleText))
			{
				return false;
			}

			return Equals((LocaleText) other);
		}
		
		public static bool operator == (LocaleText left, LocaleText right)
		{
			return left.Equals(right);
		}
		
		public static bool operator != (LocaleText left, LocaleText right)
		{
			return !(left == right);
		}

        public string GetGUID ()
        {
            if (!string.IsNullOrEmpty(guid))
            {
                return guid;
            }

            return text ?? string.Empty;
        }

        #if UNITY_EDITOR

        public System.Xml.Schema.XmlSchema GetSchema ()
        {
            return null;
        }

        public void WriteXml (System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("guid", guid ?? string.Empty);
            writer.WriteValue (text ?? string.Empty);
        }

        public void ReadXml (System.Xml.XmlReader reader)
        {
            this.guid = reader.GetAttribute("guid");
            this.text = reader.ReadElementContentAsString();
            OnDeserialize?.Invoke(this);
        }

        #endif

        public static event Action<LocaleText> OnDeserialize;
    }
}