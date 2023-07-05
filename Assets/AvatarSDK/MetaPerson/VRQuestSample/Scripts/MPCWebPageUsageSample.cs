/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, June 2023
*/

using AvatarSDK.MetaPerson.ModelLoader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if VUPLEX_PLUGIN
using Vuplex.WebView;
#endif

namespace AvatarSDK.MetaPerson.QuestSample
{
#if VUPLEX_PLUGIN
	public class MPCWebPageUsageSample : MonoBehaviour
	{
		public AccountCredentials credentials;

		public GameObject canvas;

		public GameObject canvasWebViewObject;

		public Text progressText;

		public Text errorText;

		public MetaPersonLoader metaPersonLoader;

		private CanvasWebViewPrefab canvasWebViewPrefab;

		private bool isWebViewInitialized = false;

		private void Start()
		{
			canvasWebViewPrefab = canvasWebViewObject.GetComponent<CanvasWebViewPrefab>();
		}

		public void OnImportAvatarButtonClick()
		{
			if (credentials.IsEmpty())
			{
				errorText.text = "Credentials are not provided.";
				return;
			}

			ShowWebView();
		}

		public void OnGetDetailsButtonClick()
		{
			Application.OpenURL("https://github.com/avatarsdk/metaperson-vr-quest-sample#importing-avatar-from-web-page");
		}

		public void OnCloseButtonClick()
		{
			canvas.SetActive(false);
		}

		private async void ShowWebView()
		{
			canvas.SetActive(true);

			if (!isWebViewInitialized)
			{
				string url = "https://metaperson.avatarsdk.com/iframe.html";

				Web.SetUserAgent(false);

				canvasWebViewPrefab.LogConsoleMessages = true;

				await canvasWebViewPrefab.WaitUntilInitialized();
				canvasWebViewPrefab.WebView.LoadUrl(url);
				canvasWebViewPrefab.DragMode = DragMode.DragWithinPage;

				ConfigureJSApi();

				isWebViewInitialized = true;
			}
		}

		private void ConfigureJSApi()
		{
			string javaScriptCode = @"
					const CLIENT_ID = '" + credentials.clientId + @"';
					const CLIENT_SECRET = '" + credentials.clientSecret + @"';

					function onWindowMessage(evt) {
						if (evt.type === 'message') {
							if (evt.data?.source === 'metaperson_creator') {
								let data = evt.data;
								let evtName = data?.eventName;
								if (evtName === 'unity_loaded') {
									onUnityLoaded(evt, data);
								} else if (evtName === 'model_exported') {
									console.log('model url: ' + data.url);
									window.vuplex.postMessage(evt.data);
								}
							}
						}
					}

					function onUnityLoaded(evt, data) {
						let authenticationMessage = {
							'eventName': 'authenticate',
							'clientId': CLIENT_ID,
							'clientSecret': CLIENT_SECRET,
							'exportTemplateCode': '',
						};
						window.postMessage(authenticationMessage, '*');

						let exportParametersMessage = {
							'eventName': 'set_export_parameters',
							'format': 'glb',
							'lod': 2,
							'textureProfile': '1K.jpg'
						};
						evt.source.postMessage(exportParametersMessage, '*');

						let uiParametersMessage = {
							'eventName': 'set_ui_parameters',
							'isExportButtonVisible' : true,
							'closeExportDialogWhenExportComlpeted' : true,
						  };
						evt.source.postMessage(uiParametersMessage, '*');
					}

					window.addEventListener('message', onWindowMessage);
				";

			canvasWebViewPrefab.WebView.MessageEmitted += OnWebViewMessageReceived;
			canvasWebViewPrefab.WebView.ExecuteJavaScript(javaScriptCode, OnJavaScriptExecuted);
		}

		private void OnJavaScriptExecuted(string executionResult)
		{
			Debug.LogFormat("JS execution result: {0}", executionResult);
		}

		private async void OnWebViewMessageReceived(object sender, EventArgs<string> args)
		{
			Debug.LogFormat("Got WebView message: {0}", args.Value);

			try
			{
				ModelExportedEvent modelExportedEvent = JsonUtility.FromJson<ModelExportedEvent>(args.Value);
				if (modelExportedEvent.eventName == "model_exported" && modelExportedEvent.source == "metaperson_creator")
				{
					Debug.LogFormat("Model exported: {0}", modelExportedEvent.url);
					canvas.SetActive(false);
					await metaPersonLoader.LoadModelAsync(modelExportedEvent.url, p => progressText.text = string.Format("Downloading avatar: {0}%", (int)(p * 100)));
					progressText.text = string.Empty;
				}
			}
			catch (Exception exc)
			{
				Debug.LogErrorFormat("Unable to parse message: {0}. Exception: {1}", args.Value, exc);
				errorText.text = "Unable to load the model";
				canvas.SetActive(false);
			}
		}
	}
#else
	public class MPCWebPageUsageSample : MonoBehaviour
	{
		public AccountCredentials credentials;

		public GameObject canvas;

		public GameObject canvasWebViewObject;

		public Text progressText;

		public Text errorText;

		public MetaPersonLoader metaPersonLoader;

		public void OnImportAvatarButtonClick()
		{
			errorText.text = "Vuplex Web View plugin is required to show MetaPerson Creator page.";
		}

		public void OnGetDetailsButtonClick()
		{
			Application.OpenURL("https://github.com/avatarsdk/metaperson-vr-quest-sample#importing-avatar-from-web-page");
		}
	}
#endif
}
