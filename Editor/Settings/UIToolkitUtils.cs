using System;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventsSystem.Editor.Settings
{
	public static class UIToolkitUtils
	{
		public static VisualElement CreateShortcutField(SerializedProperty shortcutProperty, string tag, Func<KeyCombination> getKeyCombination)
		{
			VisualElement shortcutContainer = new VisualElement()
			{
				style = { flexDirection = FlexDirection.Row }
			};


			var keycodeField = new PropertyField(shortcutProperty.FindPropertyRelative("m_KeyCode"));
			keycodeField.label = "Shortcut";
			shortcutContainer.Add(keycodeField);
							
			var modifiersField = new PropertyField(shortcutProperty.FindPropertyRelative("m_Modifiers"))
			{
				style =
				{
					width = 160
				}
			};
			modifiersField.label = string.Empty;
			shortcutContainer.Add(modifiersField);

			shortcutContainer.Add(new Button(() =>
			{
				var keyCombination = getKeyCombination.Invoke();
				ShortcutManager.instance.RebindShortcut(ScriptableEventsSettings.WINDOW_SHORTCUT_ID, new ShortcutBinding(keyCombination));
			})
			{
				text = "Rebind",
				style = { height = 20 }
			});
			return shortcutContainer;
		}

	}
}