
/********************************************************************
created:    2014-03-28
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;

namespace Metadata
{
    /// <summary>
    /// 用于标记field不被序列化。
	/// 1. 为什么不使用NonSerializedAttribute ？因为它不支持Property，如果以后我们需要支持Property序列化的话，自定义的标记灵活性强；另外， NonSerializedAttribute是sealed类。
	/// 2. 为什么不使用XmlIgnoreAttribute ？这个标记会直接引入System.Xml.dll，应该尽量避免。
	/// </summary>
    [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property )]
    public class AutoCodeIgnoreAttribute: Attribute
    {
        
    }
}
