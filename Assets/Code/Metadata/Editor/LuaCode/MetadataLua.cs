
/********************************************************************
created:    2015-03-19
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;
using Unicorn;
using Unicorn.AutoCode;

namespace Metadata.LuaCode
{
    class MetadataLua: LuaBase
    {
		private struct MemberItem
		{
			public MemberItem (Type type, string name)
			{
				this.type = type;
				this.name = name;
			}

			public string name;
			public Type type;
		}

		private void _CheckAddThis (List<LuaBase> members)
		{
			foreach (var member in members)
			{
				if (member.GetMemberType() == _type)
				{
					return;
				}
			}

			members.Add(this);
		}

		public override void CollectCreatorMembers (List<LuaBase> members)
		{
			_CheckAddThis(members);

			foreach (var item in _GetMembers())
			{
				var member = Create(item.type, item.name, _rootMetadataTypeName);
				member.CollectCreatorMembers(members);
			}
		}

		public override void WriteCreatorFunction (CodeWriter writer)
		{
            writer.WriteLine();
			writer.WriteLine("function {0}._Create{1} (aid)", _rootMetadataTypeName, _type.Name);

			using (CodeScope.CreateLuaScope(writer))
			{
                // skip metadataType
                writer.WriteLine("aid.ReadUInt16 ()");
				writer.WriteLine("local item = {}");

				foreach (var item in _GetMembers())
				{
					var member = Create(item.type, item.name, _rootMetadataTypeName);
					member.WriteLoad(writer);
				}

                writer.WriteLine();
                writer.WriteLine("return setmetatable(item, {0}.readonlyMetatable)", LuaCodeMaker.metadataTypeName);
			}
		}

		public override void WriteLoad (CodeWriter writer)
		{
			writer.WriteLine();
			if (_type.IsClass)
			{
				writer.WriteLine("if aid.ReadBoolean() then");
				using (CodeScope.CreateLuaScope(writer))
				{
					_WriteLoadMetadataWithoutMetaType(writer);
				}
			}
			else if (_type.IsValueType)
			{
				_WriteLoadMetadataWithoutMetaType(writer);
			}
		}

		private void _WriteLoadMetadataWithoutMetaType (CodeWriter writer)
		{
			writer.WriteLine("item.{0} = {1}._Create{2} (aid)", _name, _rootMetadataTypeName,_type.Name);
		}

		private IEnumerable<MemberItem> _GetMembers ()
		{
			// private fields will not be serialized into .xml, so we must use public fields.
			var flags = BindingFlags.Instance | BindingFlags.Public;
			
            foreach (var field in TypeTools.GetSortedFields(_type, flags))
			{
				if (!EditorMetaTools.IsAutoCodeIgnore(field))
				{
					yield return new MemberItem(field.FieldType, field.Name);
				}
			}

//			foreach (var property in EditorMetaCommon.GetExportableProperties(_type, flags))
//			{
//				yield return new MemberItem(property.PropertyType, property.Name);
//			}
		}
    }
}