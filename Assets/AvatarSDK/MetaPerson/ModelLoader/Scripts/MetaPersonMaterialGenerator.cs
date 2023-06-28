/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, May 2023
*/

using GLTFast;
using GLTFast.Logging;
using GLTFast.Materials;
using GLTFast.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarSDK.MetaPerson.ModelLoader
{
	public class MetaPersonMaterialGenerator : MonoBehaviour, IMaterialGenerator
	{
		public UnityEngine.Material defaultMaterial;

		public UnityEngine.Material eyelashesMaterial;

		public UnityEngine.Material haircutMaterial;

		private List<Texture2D> texturesToDestroy = new List<Texture2D>();

		#region IMaterialGenerator
		public UnityEngine.Material GenerateMaterial(GLTFast.Schema.Material gltfMaterial, IGltfReadable gltf, bool pointsSupport = false)
		{
			if (gltfMaterial.name == "AvatarEyelashes")
				return GenerateEyelashesMaterial(gltfMaterial, gltf);
			if (gltfMaterial.name == "haircut")
				return GenerateHaircutMaterial(gltfMaterial, gltf);
			return GenerateDefaultMaterial(gltfMaterial, gltf);
		}

		public UnityEngine.Material GetDefaultMaterial(bool pointsSupport = false)
		{
			return Instantiate(defaultMaterial);
		}

		public void SetLogger(ICodeLogger logger)
		{
			
		}
		#endregion

		public void DestroyUnusedTextures()
		{
			foreach (Texture2D texture in texturesToDestroy)
				Destroy(texture);
		}

		private UnityEngine.Material GenerateHaircutMaterial(GLTFast.Schema.Material gltfMaterial, IGltfReadable gltf)
		{
			UnityEngine.Material material = Instantiate(haircutMaterial);
			if (gltfMaterial.pbrMetallicRoughness.baseColorTexture.index >= 0)
				material.mainTexture = gltf.GetTexture(gltfMaterial.pbrMetallicRoughness.baseColorTexture.index);
			if (gltfMaterial.normalTexture.index >= 0)
				material.SetTexture("_BumpMap", gltf.GetTexture(gltfMaterial.normalTexture.index));
			if (gltfMaterial.occlusionTexture.index >= 0)
				material.SetTexture("_OcclusionMap", gltf.GetTexture(gltfMaterial.occlusionTexture.index));
			return material;
		}

		private UnityEngine.Material GenerateEyelashesMaterial(GLTFast.Schema.Material gltfMaterial, IGltfReadable gltf)
		{
			UnityEngine.Material material = Instantiate(eyelashesMaterial);
			if (gltfMaterial.pbrMetallicRoughness.baseColorTexture.index >= 0)
				material.mainTexture = gltf.GetTexture(gltfMaterial.pbrMetallicRoughness.baseColorTexture.index);
			return material;
		}

		private UnityEngine.Material GenerateDefaultMaterial(GLTFast.Schema.Material gltfMaterial, IGltfReadable gltf)
		{
			UnityEngine.Material material = Instantiate(defaultMaterial);
			if (gltfMaterial.pbrMetallicRoughness.baseColorTexture.index >= 0)
				material.mainTexture = gltf.GetTexture(gltfMaterial.pbrMetallicRoughness.baseColorTexture.index);
			if (gltfMaterial.normalTexture.index >= 0)
				material.SetTexture("_BumpMap", gltf.GetTexture(gltfMaterial.normalTexture.index));
			if (gltfMaterial.occlusionTexture.index >= 0)
				material.SetTexture("_OcclusionMap", gltf.GetTexture(gltfMaterial.occlusionTexture.index));

			if (gltfMaterial.pbrMetallicRoughness.metallicRoughnessTexture.index >= 0)
			{
				Texture2D metallicRoughnessTexture = gltf.GetTexture(gltfMaterial.pbrMetallicRoughness.metallicRoughnessTexture.index);
				Texture2D unityMetallicSmoothnessTexture = ConvertGltfMetallicRoughnessToUnityMetallicSmoothness(metallicRoughnessTexture);
				material.SetTexture("_MetallicGlossMap", unityMetallicSmoothnessTexture);
				if (!texturesToDestroy.Contains(metallicRoughnessTexture))
					texturesToDestroy.Add(metallicRoughnessTexture);
			}

			return material;
		}

		private Texture2D ConvertGltfMetallicRoughnessToUnityMetallicSmoothness(Texture2D gltfMetallicRougnessTexture)
		{
			RenderTexture dstRenderTexture = RenderTexture.GetTemporary(gltfMetallicRougnessTexture.width, gltfMetallicRougnessTexture.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);

			Graphics.Blit(gltfMetallicRougnessTexture, dstRenderTexture);

			var unityMetallicSmoothnessTexture = new Texture2D(gltfMetallicRougnessTexture.width, gltfMetallicRougnessTexture.height);
			unityMetallicSmoothnessTexture.ReadPixels(new Rect(0, 0, dstRenderTexture.width, dstRenderTexture.height), 0, 0);
			unityMetallicSmoothnessTexture.Apply();

			Color32[] pixels = unityMetallicSmoothnessTexture.GetPixels32();
			for(int i=0; i < pixels.Length; i++)
			{
				pixels[i].a = (byte)(255 - pixels[i].g);
				pixels[i].r = pixels[i].b;
				pixels[i].g = pixels[i].b;
			}
			unityMetallicSmoothnessTexture.SetPixels32(pixels);
			unityMetallicSmoothnessTexture.Apply(true, true);

			RenderTexture.ReleaseTemporary(dstRenderTexture);

			return unityMetallicSmoothnessTexture;
		}
	}
}
