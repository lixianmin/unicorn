
/*********************************************************************
created:    2017-02-16
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/
using System;

namespace Metadata
{
	public static class ExtendedIMetadata
	{
//        public static ushort GetMetadataTypeEx (this IMetadata metadata)
//        {
//            if (null != metadata)
//            {
//                var type = metadata.GetType();
//                var creator = MetaFactory.GetMetaCreator(type);
//                if (null != creator)
//                {
//                    var metadataType = creator.GetMetadataType3();
//                    return metadataType;
//                }
//            }
//
//            return 0;
//        }

		public static string ToStringEx (this IMetadata metadata)
		{
			if (null != metadata)
			{
				try
				{
					var text = metadata.ToString();
					return text;
				}
				catch (Exception)
				{
					return "Exception occurred for metatata.GetType()= " + metadata.GetType().ToString();
				}
			}

			return string.Empty;
		}
	}
}