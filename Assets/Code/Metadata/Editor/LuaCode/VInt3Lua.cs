
/********************************************************************
created:    2017-07-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using Unicorn;
using Unicorn.AutoCode;

namespace Metadata.LuaCode
{
    class VInt3Lua: LuaBase
    {
        public override void WriteLoad (CodeWriter writer)
        {
            writer.WriteLine("item.{0} = aid.ReadVInt3()", _name);
        }
    }
}