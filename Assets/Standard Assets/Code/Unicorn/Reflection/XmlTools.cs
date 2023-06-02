
/********************************************************************
created:    2014-04-16
author:     lixianmin

purpose:    assert
Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.IO;

namespace Unicorn
{
    public static class XmlTools
    {
        public static void Serialize<T> (string path, T item)
        {
            using var writer = new StreamWriter(path);
            writer.NewLine = "\n";
            Serialize(writer, item);
        }

        public static void Serialize<T> (TextWriter writer, T item)
        {
			if (null == _lpfnSerialize)
			{
				TypeTools.CreateDelegate(_GetTypeEditorXmlTools(), "_Serialize", out _lpfnSerialize);
			}

			if (null != _lpfnSerialize)
			{
				_lpfnSerialize(typeof(T), writer, item);
			}
        }

        public static T Deserialize<T> (string path)
        {
            StreamReader reader = null;
            T result = default;

            try 
            {
                reader = new StreamReader(path);
                result = Deserialize<T>(reader);
            }
            catch (Exception ex)
            {
                Logo.Error("path={0}, ex={1}", path, ex);
            }
            finally
            {
                if (null != reader)
                {
                    reader.Close();
                }
            }

            return result;
        }
        
        public static T Deserialize<T> (TextReader reader)
        {
			if (null == _lpfnDeserialize)
			{
				TypeTools.CreateDelegate(_GetTypeEditorXmlTools(), "_Deserialize", out _lpfnDeserialize);
			}

			if (null != _lpfnDeserialize)
			{
				return (T) _lpfnDeserialize(typeof(T), reader);
			}

			return default;
        }

        public static T OpenOrCreate<T> (string path) where T : new()
        {
            if (File.Exists(path))
            {
                var xml = Deserialize<T>(path);

                if (null != xml)
                {
                    return xml;
                }
            }

            return new T();
        }

		private static Type _GetTypeEditorXmlTools ()
		{
			var type = TypeTools.SearchType("Unicorn.EditorXmlTools");
			return type;
		}

		private static Action<Type, TextWriter, object> _lpfnSerialize;
		private static Func<Type, TextReader, object> _lpfnDeserialize;
    }
}