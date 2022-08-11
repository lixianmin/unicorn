
/********************************************************************
created:    2016-08-27
author:     lixianmin

Copyright (C) - All Rights Reserved
*********************************************************************/

using UnityEditor;

namespace Unicorn
{
	internal static class iOSTargetTools
	{
#if UNITY_5_3_OR_NEWER
		internal static BuildTarget GetBuildTarget()
		{
			return BuildTarget.iOS;
		}

		internal static BuildTargetGroup GetBuildTargetGroup()
		{
			return BuildTargetGroup.iOS;
		}

		internal static int GetUniversalIPhoneArchitecture()
		{
			return 1;
		}

#else
		internal static BuildTarget GetBuildTarget ()
		{
			return BuildTarget.iPhone;
		}

		internal static BuildTargetGroup GetBuildTargetGroup ()
		{
			return BuildTargetGroup.iPhone;
		}

		internal static int GetUniversalIPhoneArchitecture ()
		{
			return (int) iPhoneArchitecture.Universal;
		}

#endif
	}
}