
/********************************************************************
created:    2017-07-15
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using Unicorn;
using Unicorn.AutoCode;

namespace Metadata.LuaCode
{
    class LocaleTextLua: LuaBase
    {
        public override void WriteLoad (CodeWriter writer)
        {
            writer.WriteLine("item.{0} = aid.ReadLocaleText ()", _name);
//            writer.WriteLine();
        }
    }
}