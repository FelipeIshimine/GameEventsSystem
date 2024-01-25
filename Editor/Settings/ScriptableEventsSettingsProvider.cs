using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameEventsSystem.Editor.Settings
{
	public class ScriptableEventsSettingsProvider : SettingsProvider
	{
		public ScriptableEventsSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) :
			base(path, scopes, keywords)
		{
		}

		[SettingsProvider]
		public static SettingsProvider CreateMyCustomSettingsProvider()
		{
			var settings = ScriptableEventsSettings.GetOrCreateSettings();
			if (settings)
			{
				var serializedObject = new SerializedObject(settings);
				var provider =
					new ScriptableEventsSettingsProvider($"Project/{nameof(GameEventsSystem)}",
						SettingsScope.Project)
					{
						label = nameof(GameEventsSystem),
						// activateHandler is called when the user clicks on the Settings item in the Settings window.
						activateHandler = (searchContext, rootElement) =>
						{
							var title = new Label()
							{
								text = nameof(GameEventsSystem),
								style =
								{
									fontSize = 25
								}
							};
							rootElement.Add(title);
							rootElement.Add(new VisualElement() { style = { height = 20 } });
							var properties = new VisualElement()
							{
								style =
								{
									flexDirection = FlexDirection.Column
								}
							};
							properties.AddToClassList("property-list");
							rootElement.Add(properties);

							var shortcutProperty = serializedObject.FindProperty($"{nameof(settings.shortcut)}");

							var shortcutContainer = UIToolkitUtils.CreateShortcutField(
								shortcutProperty,
								ScriptableEventsSettings.WINDOW_SHORTCUT_ID,
								()=>settings.shortcut);
							
							rootElement.Add(shortcutContainer);

							rootElement.Add(new PropertyField(serializedObject.FindProperty($"{nameof(settings.pingEventAssetWhenSelected)}")));
							rootElement.Add(new PropertyField(serializedObject.FindProperty($"{nameof(settings.pingElementsInAssetsWhenSelected)}")));
							rootElement.Add(new PropertyField(serializedObject.FindProperty($"{nameof(settings.pingElementsInContextWhenSelected)}")));
							
							rootElement.Bind(serializedObject);
						},

					};

				return provider;
			}

			return null;
		}

		

	}
}