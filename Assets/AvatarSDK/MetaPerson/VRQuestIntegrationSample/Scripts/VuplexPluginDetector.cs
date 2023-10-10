/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, June 2023
*/

#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace AvatarSDK.MetaPerson.VRQuestIntegrationSample
{
	[InitializeOnLoad]
	public class VuplexPluginDetector : Editor
	{
		private readonly static string VUPLEX_DEFINE_SYMBOL = "VUPLEX_PLUGIN";

		static VuplexPluginDetector()
		{
			string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			List<string> allDefines = definesString.Split(';').ToList();
			if (FindVuplex())
			{
				if (!allDefines.Contains(VUPLEX_DEFINE_SYMBOL))
					allDefines.Add(VUPLEX_DEFINE_SYMBOL);
			}
			else
			{
				if (allDefines.Contains(VUPLEX_DEFINE_SYMBOL))
					allDefines.Remove(VUPLEX_DEFINE_SYMBOL);
			}

			PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,string.Join(";", allDefines.ToArray()));
		}

		private static bool FindVuplex()
		{
			string className = "Vuplex.WebView.CanvasWebViewPrefab";
			Assembly assembly = Assembly.GetExecutingAssembly();
			Type implType = assembly.GetType(className);
			return implType != null;
		}

	}
}
#endif