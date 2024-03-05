
/********************************************************************
created:    2017-01-24
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

namespace Metadata
{
    public enum BasicType : byte
    {
        String, // string和int用的最多, 所以放到最前面
        
        Int32,
        Single,
        Boolean,
        Int16,
        Int64,
        Byte,
        Double,
        UInt16,
        UInt32,
        UInt64,
        SByte,

        Vector2,
        Vector3,
        Vector4,
        Color,

        Null,
        Array,
        List,
        Enum,

        LocaleText,
        Int32_t,
        Int64_t,
        Float_t,
        VInt3,

        Metadata,
    }
}