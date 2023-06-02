
/********************************************************************
created:    2016-10-09
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using Unicorn;

namespace Metadata
{
    public class MetaCreator
    {
        public MetaCreator (Func<IMetadata> creator, bool editorOnlyCreator= false)
		{
			_lpfnCreator = creator;
            _isEditorOnlyCreator = editorOnlyCreator;
		}

        internal Type GetMetadataType ()
        {
            if (null == _metadataType)
            {
                var metadata  = Create();
                _metadataType = metadata.GetType();
            }

            return _metadataType;
        }

		internal IMetadata Create ()
		{
			if (null != _lpfnCreator)
			{
                var metadata = _lpfnCreator();
                return metadata;
			}

			return null;
		}

        internal byte[] GetLayout ()
        {
            if (null == _layout)
            {
                var layout = Array.Empty<byte>();
                var metadataType = GetMetadataType();
                if (null != metadataType)
                {
                    var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public;
                    var fields = TypeTools.GetSortedFields(metadataType, flags);
                    var fieldCount = (ushort) fields.Length;
                    layout = new byte[fieldCount];

                    for (int i= 0; i< fieldCount; ++i)
                    {
                        var field = fields[i];
                        var basicType = MetaTools.GetBasicType(field.FieldType);
                        layout[i] = (byte) basicType;
                    }
                }
                else
                {
                    Logo.Error("Create() should not create null metadata");
                }

                SetLayout(layout);
            }

            return _layout;
        }

        internal void SetLayout (byte[] layout)
        {
            _layout = layout;
        }

        internal bool IsEditorOnlyCreator ()
        {
            return _isEditorOnlyCreator;
        }

        private Type _metadataType;
		private Func<IMetadata> _lpfnCreator;
        private byte[] _layout;
        private bool _isEditorOnlyCreator;
    }
}