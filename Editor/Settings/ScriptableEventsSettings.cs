using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace ScriptableEventsSystem.Editor.Settings
{
	class ScriptableEventsSettings : ScriptableObject
	{
		public const string WINDOW_SHORTCUT_ID = "Window/ScriptableEvents";
		public static string FilePath => $"Assets/Editor/{nameof(ScriptableEventsSettings)}.asset";

		public KeyCombination shortcut = new(KeyCode.F6, ShortcutModifiers.Control | ShortcutModifiers.Shift); // I don't know how to add more than one modifiers

		public bool pingEventAssetWhenSelected = true;
		
		public bool pingElementsInContextWhenSelected = true;
		public bool pingElementsInAssetsWhenSelected = true;

		internal static ScriptableEventsSettings GetOrCreateSettings()
		{
			var settings = AssetDatabase.LoadAssetAtPath<ScriptableEventsSettings>(FilePath);
			if (settings == null)
			{
				settings = ScriptableObject.CreateInstance<ScriptableEventsSettings>();
				Debug.Log($"{nameof(ScriptableEventsSettings)} Created at:{FilePath}");
				AssetDatabase.CreateAsset(settings, FilePath);
				AssetDatabase.SaveAssets();
			}

			return settings;
		}


		private void OnEnable()
		{
			ShortcutManager.instance.RebindShortcut(WINDOW_SHORTCUT_ID, new ShortcutBinding(shortcut));
		}
	}
}