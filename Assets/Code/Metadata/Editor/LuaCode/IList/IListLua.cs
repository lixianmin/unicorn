
/********************************************************************
created:    2015-03-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using Unicorn;
using Unicorn.AutoCode;

namespace Metadata.LuaCode
{
    abstract class IListLua: LuaBase
    {
		public override void CollectCreatorMembers (List<LuaBase> members)
		{
			var elementType = _GetElementType();
			var elementName = elementType.Name;
			var member = Create(elementType, elementName, _rootMetadataTypeName);
			member.CollectCreatorMembers(members);
		}
		
		public override void WriteLoad (CodeWriter writer)
		{
			var indexName = "index" + writer.Indent;
            writer.WriteLine("item.{0} = {{ }}", _name);
            writer.WriteLine("aid.ReadByte()", _name);

            writer.WriteLine();
			writer.WriteLine("for {0} = 1, aid.ReadInt32() do", indexName);
			using (CodeScope.CreateLuaScope(writer))
			{
				var elementType = _GetElementType();
				var elementName = string.Format("{0}[{1}]", _name, indexName);
				var member = Create(elementType, elementName, _rootMetadataTypeName);
				member.WriteLoad(writer);
			}
		}

		protected abstract Type _GetElementType ();
    }
}