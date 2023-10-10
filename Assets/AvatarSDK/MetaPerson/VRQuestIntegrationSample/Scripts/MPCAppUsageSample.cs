/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, May 2023
*/

using AvatarSDK.MetaPerson.Loader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AvatarSDK.MetaPerson.VRQuestIntegrationSample
{
	public class MPCAppUsageSample : MonoBehaviour
	{
		public AccountCredentials credentials;

		public Text progressText;

		public Text errorText;

		public MetaPersonLoader metaPersonLoader;

		public void OnImportAvatarButtonClick()
		{
			if (credentials.IsEmpty())
			{
				errorText.text = "Credentials are not provided.";
				return;
			}

			try
			{
				Application.OpenURL(string.Format("metaperson://get_avatar?clientId={0}&clientSecret={1}&format=glb&lod=2&textureProfile=1K.jpg", 
					credentials.clientId, credentials.clientSecret));
			}
			catch(Exception exc)
			{
				Debug.LogErrorFormat("Unable to open MetaPerson Creator application: {0}", exc);
				errorText.text = "Unable to open MetaPerson Creator application";
			}
		}

		public void OnGetDetailsButtonClick()
		{
			Application.OpenURL("https://github.com/avatarsdk/metaperson-vr-quest-sample#importing-avatar-from-metaperson-creator-app");
		}

		private void Start()
		{
			Application.deepLinkActivated += onDeepLinkActivated;
			if (!string.IsNullOrEmpty(Application.absoluteURL))
				onDeepLinkActivated(Application.absoluteURL);
		}

		private async void onDeepLinkActivated(string url)
		{
			try
			{
				string[] urlParsed = url.Split('?');
				if (urlParsed.Length == 2)
				{
					await metaPersonLoader.LoadModelAsync(urlParsed[1], p => progressText.text = string.Format("Downloading avatar: {0}%", (int)(p * 100)));
					progressText.text = string.Empty;
				}
			}
			catch (Exception exc)
			{
				Debug.LogErrorFormat("Exception during parsing URL={0}. Error: {1}", url, exc);
				errorText.text = "Unable to parse model's URL";
			}
		}
	}
}
