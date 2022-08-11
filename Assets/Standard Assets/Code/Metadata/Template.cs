
/********************************************************************
created:    2013-12-14
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;
using System.IO;
using Unicorn;
using Unicorn.IO;

namespace Metadata
{
    [Serializable]
    public abstract class Template: IMetadata
    {
        public Template ()
        {
        }

        public int id;
    }
}