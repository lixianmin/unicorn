
/********************************************************************
created:    2017-07-25
author:     lixianmin

Copyright (C) - All Rights Reserved

<?xml version="1.0" encoding="utf-8"?>
<XmlLocaleTextTranslator xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <items>
    <LocaleText guid="guid-hello">hello</LocaleText>
    <LocaleText guid="guid-world">world</LocaleText>
  </items>
</XmlLocaleTextTranslator>

*********************************************************************/
using System;

namespace Metadata
{
	[Serializable]
    public class XmlLocaleTextTranslator
    {
        public LocaleText[] items;
    }
}