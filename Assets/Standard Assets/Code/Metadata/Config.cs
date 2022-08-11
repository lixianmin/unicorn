
/********************************************************************
created:    2014-01-13
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
    public abstract class Config: IMetadata
    {
        public Config()
        {
        }
    }
}