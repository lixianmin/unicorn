
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
using UnityEngine;

namespace Metadata.LuaCode
{
    abstract class LuaBase
    {
		public static LuaBase Create (Type type, string name, string rootMetadataTypeName)
        {
            LuaBase member = null;

            if (type.IsPrimitive || type == typeof(Int32_t) || type == typeof(Int64_t) || type == typeof(Float_t))
			{
				member = new PrimitiveLua();
			}
            else if (type == typeof (string))
			{
				member = new StringLua();
			}
            else if (type == typeof (LocaleText))
            {
                member = new LocaleTextLua();
            }
            else if (type == typeof(VInt3))
            {
                member = new VInt3Lua();
            }
            else if (type == typeof(Vector2))
			{
				member = new Vector2Lua();
			}
			else if (type == typeof(Vector3))
			{
				member = new Vector3Lua();
			}
			else if (type == typeof(Vector4))
			{
				member = new Vector4Lua();
			}
            else if (type == typeof(Color))
			{
				member = new ColorLua();
			}
            else if (type.IsArray)
            {
                member = new ArrayLua();
            }
            else if (type.IsEnum)
            {
                member = new EnumLua();
            }
            else if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition();

                if (typeof(List<>) == genericTypeDefinition)
                {
                    member = new ListLua();
                }
            } 
            else if (MetaTools.IsMetadata(type))
            {
                member = new MetadataLua();
            }

            if (null != member)
            {
				member._type	= type;
				member._name	= name;
				member._rootMetadataTypeName = rootMetadataTypeName;
				// member._prefix	= prefix;
				// member._postfix	= postfix;
            }
            else
            {
                var text = string.Format("Invalid type found: typeof({0}) = {1}", name, type);
                Logo.Error(text);
            }

            return member;
        }

		public virtual void CollectCreatorMembers (List<LuaBase> members)
		{

		}

		public virtual void WriteCreatorFunction (CodeWriter writer)
		{

		}

		public Type GetMemberType ()
		{
			return _type;
		}

		public abstract void WriteLoad (CodeWriter writer);
        
		protected Type     	_type;
		protected string	_name;
		protected string	_rootMetadataTypeName;
    }
}