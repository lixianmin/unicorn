
/********************************************************************
created:    2014-02-18
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.Collections.Generic;
using System.IO;

namespace Metadata
{
	[Serializable]
    public partial class XmlMetadata
    {
        public override string ToString ()
        {
            var sb = new System.Text.StringBuilder(128);
            foreach (var template in Templates)
            {
                sb.Append(template);
                sb.Append('\n');
            }

            foreach (var config in Configs)
            {
                sb.Append(config);
                sb.Append('\n');
            }

            return sb.ToString();
        }

        public readonly List<Template> Templates = new List<Template>();
        public readonly List<Config> Configs = new List<Config>();
    }
}