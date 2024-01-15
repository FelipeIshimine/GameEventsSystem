using System;
using System.IO;
using GameEventsSystem.Events;
using ScriptableEventsSystem.Events;
using UnityEditor;
using UnityEngine;

namespace ScriptableEventsSystem.Editor
{
	public static class GameEventsEditorUtils
	{
		public static T FindOrCreateTargetEvent<T>(string eventName) where T : BaseGameEvent
		{
			var guids=AssetDatabase.FindAssets($"{eventName} t:{typeof(T)}");
			T targetEvent = null;

			foreach (string guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var fileName = Path.GetFileName(path);

				if (fileName == eventName)
				{
					targetEvent = AssetDatabase.LoadAssetAtPath<T>(path);
				}
			}

			if (targetEvent == null)
			{
				targetEvent = ScriptableObject.CreateInstance<T>();
				AssetDatabase.CreateAsset(targetEvent, $"Assets/{eventName}.asset");
			}

			return targetEvent;
		}
		
		public static ScriptableObject FindOrCreateTargetEvent(string eventName, Type type)
		{
			var guids=AssetDatabase.FindAssets($"{eventName} t:{type}");
			ScriptableObject targetEvent = null;

			foreach (string guid in guids)
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var fileName = Path.GetFileName(path);

				if (fileName == eventName)
				{
					targetEvent = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
				}
			}

			if (targetEvent == null)
			{
				targetEvent = ScriptableObject.CreateInstance<ScriptableObject>();
				AssetDatabase.CreateAsset(targetEvent, $"Assets/{eventName}.asset");
			}

			return targetEvent;
		}
	}
}