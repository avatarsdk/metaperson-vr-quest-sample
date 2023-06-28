/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, June 2023
*/

#if VUPLEX_PLUGIN
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuplex.WebView;

namespace AvatarSDK.MetaPerson.QuestSample
{
	public class VuplexKeyboardExtension : MonoBehaviour
	{
		public CanvasWebViewPrefab canvasWebViewPrefab;

		public CanvasKeyboard canvasKeyboard;

		private Dictionary<string, string> keyShiftPairs = new Dictionary<string, string>()
		{
			{ "~", "`" },
			{ "!", "1" },
			{ "@", "2" },
			{ "#", "3" },
			{ "$", "4" },
			{ "%", "5" },
			{ "^", "6" },
			{ "&", "7" },
			{ "*", "8" },
			{ "(", "9" },
			{ ")", "0" },
			{ "_", "-" },
			{ "+", "=" },
			{ "{", "[" },
			{ "}", "]" },
			{ "|", "\\" },
			{ ":", ";" },
			{ "\"", "'" },
			{ "<", "," },
			{ ">", "." },
			{ "?", "/" }
		};

		private async void Start()
		{
			if (canvasKeyboard != null && canvasWebViewPrefab != null)
			{
				await canvasWebViewPrefab.WaitUntilInitialized();
				var webViewWithKeyDown = canvasWebViewPrefab.WebView as IWithKeyDownAndUp;
				if (webViewWithKeyDown != null)
				{
					canvasKeyboard.KeyPressed += (s, e) =>
					{
						Debug.LogWarningFormat("[DBG] KeyPressed: {0}", e.Value);
						if (e.Value.Length == 1)
						{
							char key = e.Value[0];
							if (char.IsLetter(key))
							{
								if (char.IsUpper(key))
								{
									webViewWithKeyDown.KeyDown(Convert.ToString(char.ToLower(key)), KeyModifier.Shift);
									Debug.LogWarningFormat("[DBG] KeyDown1: {0}", Convert.ToString(char.ToLower(key)));
								}
							}
							else if (keyShiftPairs.ContainsKey(e.Value))
							{
								webViewWithKeyDown.KeyDown(Convert.ToString(keyShiftPairs[e.Value]), KeyModifier.Shift);
								Debug.LogWarningFormat("[DBG] KeyDown2: {0}", Convert.ToString(keyShiftPairs[e.Value]));
							}
						}
					};
				}
			}
		}
	}
}
#endif