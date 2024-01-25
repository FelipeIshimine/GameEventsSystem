using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameEventsSystem.Editor
{
	public static class AssetsUtils
	{
		public static UnityEngine.Object[] GetReferencingObjectsFromAssets(Object asset)
		{
			var referencingObjects = new List<Object>();

			string assetPath = AssetDatabase.GetAssetPath(asset);
			string[] guids = AssetDatabase.FindAssets("t:Prefab t:Scene");
			int processedCount = 0;

			//Debug.Log($"AssetPath:{assetPath}");

			EditorUtility.DisplayCancelableProgressBar("Scanning Assets", "Scanning...", 0f);

			foreach (var guid in guids)
			{
				StringBuilder builder = new StringBuilder();
				string path = AssetDatabase.GUIDToAssetPath(guid);
				var dependencies = AssetDatabase.GetDependencies(path);

				builder.AppendLine($"OtherAsset: {path}");
				foreach (var dependency in dependencies)
				{
					builder.AppendLine($"{dependency} : {dependency == assetPath}");
					if (dependency == assetPath)
					{
						var prefabOrScene = AssetDatabase.LoadAssetAtPath<Object>(path);
						if (prefabOrScene != null)
						{
							referencingObjects.Add(prefabOrScene);
							break;
						}
					}
				}

				//Debug.Log(builder.ToString());
				processedCount++;
				float progress = processedCount / (float)guids.Length;
				if (EditorUtility.DisplayCancelableProgressBar("Scanning Assets", "Scanning...", progress))
					break;
			}

			EditorUtility.ClearProgressBar();

			return referencingObjects.ToArray();
		}

		public static Component[] GetReferencingComponentsFromOpenedSContext(Object asset)
		{
			PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
			if (prefabStage)
			{
				var transforms = prefabStage.prefabContentsRoot.GetComponentsInChildren<Transform>();
				var gameObjects = Array.ConvertAll(transforms, x => x.gameObject);
				return new List<Component>(FindReferencingComponents(asset,gameObjects)).ToArray();
			}
			return new List<Component>(GetReferencingComponentsFromOpenedScenes(asset)).ToArray();
		}
	
	
		public static IEnumerable<Component> GetReferencingComponentsFromOpenedScenes(Object asset)
		{
			GameObject[] allGameObjects = Object.FindObjectsOfType<GameObject>();
			var referencingGameObjects = FindReferencingComponents(asset, allGameObjects);
			return referencingGameObjects;
		}

		private static IEnumerable<Component> FindReferencingComponents(Object asset, GameObject[] allGameObjects)
		{
			int processedCount = 0;

			EditorUtility.DisplayProgressBar("Scanning Scene", "Scanning...", 0f);

			foreach (var gameObject in allGameObjects)
			{
				var components = gameObject.GetComponents<Component>();
				foreach (var component in components)
				{
					if (component == null)
						continue;

					SerializedObject serializedObject = new SerializedObject(component);
					SerializedProperty property = serializedObject.GetIterator();
					while (property.NextVisible(true))
					{
						if (property.propertyType == SerializedPropertyType.ObjectReference)
						{
							if (property.objectReferenceValue == asset)
							{
								yield return component;
								break;
							}
						}
					}
				}

				processedCount++;
				float progress = processedCount / (float)allGameObjects.Length;
				EditorUtility.DisplayProgressBar("Scanning Scene", "Scanning...", progress);
			}
			EditorUtility.ClearProgressBar();
		}
	}
}