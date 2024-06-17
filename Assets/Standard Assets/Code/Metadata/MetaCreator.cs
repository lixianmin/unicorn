/********************************************************************
created:    2016-10-09
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Reflection;
using Unicorn;

namespace Metadata
{
    public class MetaCreator
    {
        public MetaCreator(Func<IMetadata> creator, bool editorOnlyCreator = false)
        {
            _lpfnCreator = creator;
            _isEditorOnlyCreator = editorOnlyCreator;
        }

        internal Type GetMetadataType()
        {
            if (null == _metadataType)
            {
                var metadata = Create();
                _metadataType = metadata.GetType();
            }

            return _metadataType;
        }

        internal IMetadata Create()
        {
            var metadata = _lpfnCreator?.Invoke();
            return metadata;
        }

        internal byte[] GetLayout()
        {
            if (null == _layout)
            {
                var layout = Array.Empty<byte>();
                var metadataType = GetMetadataType();
                if (null != metadataType)
                {
                    var fields = TypeTools.GetSortedFields(metadataType);
                    var fieldCount = fields.Length;

                    layout = new byte[fieldCount];
                    for (var i = 0; i < fieldCount; ++i)
                    {
                        var field = fields[i];
                        var basicType = MetaTools.GetBasicType(field.FieldType);
                        layout[i] = (byte)basicType;
                    }
                }
                else
                {
                    Logo.Error("metadataType is null");
                }

                SetLayout(layout);
            }

            return _layout;
        }

        internal string[] GetFieldNames()
        {
            var fieldNames = Array.Empty<string>();

            var metadataType = GetMetadataType();
            if (null != metadataType)
            {
                var fields = TypeTools.GetSortedFields(metadataType);
                var fieldCount = fields.Length;

                fieldNames = new string[fieldCount];

                for (var i = 0; i < fieldCount; ++i)
                {
                    var field = fields[i];
                    fieldNames[i] = field.Name;
                }
            }
            else
            {
                Logo.Error("metadataType is null");
            }

            return fieldNames;
        }

        internal void SetLayout(byte[] layout)
        {
            _layout = layout;
        }

        internal bool IsEditorOnlyCreator()
        {
            return _isEditorOnlyCreator;
        }

        private Type _metadataType;
        private Func<IMetadata> _lpfnCreator;
        private byte[] _layout;
        private bool _isEditorOnlyCreator;
    }
}