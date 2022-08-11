
/********************************************************************
created:    2017-01-13
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using System;
using System.Collections;

namespace Metadata
{
	partial class SaveAid
	{
        private class RemovedData
        {
            public string typeName;
            public int id;
        }

        public class IncrementData
		{
			public IncrementData (string[] baseTexts)
			{
				_baseTexts = baseTexts;
			}

			public string[] GetBaseTexts ()
			{
				return _baseTexts;
			}

			public void AddRemovedMetadata (string typeName, int id)
			{
                if (string.IsNullOrEmpty(typeName))
                {
                    return;
                }

                var key = new RemovedData { typeName = typeName, id = id };
				_removedMetadatas.Add(key);
			}

            public IList GetRemovedMetadatas ()
			{
				return _removedMetadatas;
			}

			private string[] _baseTexts;
            private readonly ArrayList _removedMetadatas = new ArrayList();
		}
	}
}