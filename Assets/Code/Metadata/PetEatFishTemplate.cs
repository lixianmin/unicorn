//
///********************************************************************
//created:    2015-6-30
//author:     fangxun
//
//Copyright (C) - All Rights Reserved
// *********************************************************************/
//
//using System.Collections.Generic;
//
//namespace Metadata
//{
//	/*
//	 * 宠物的 鱼属性提高表
//	 * 这张表是Template，主索引ID是鱼的fish_id；
//	 */
//	#if UNITY_EDITOR
//	[Export(ExportFlags.ExportAll)]
//	#endif
//    [System.Serializable]
//	public partial class PetEatFishTemplate : Template
//	{
//		public partial class StatUp : IMetadata
//		{
//            public float stat_value;
//			
//			public int stat_type;		//属性类型1;
//			
//			private float _stat_value;    //属性值1;
//		}
//
//		public List<StatUp> stat_up = new List<StatUp>();	//eg. 每个鱼有4个提升;
//        public string hello;
//        public LocaleText world;
//        public LocaleText[] locales;
//        public Int32_t height;
//        public Int32_t[] ages;
//        public int[] another;
//        public Float_t f32;
//        public float normal_float;
//        public Int64_t i64;
//	}
//}
