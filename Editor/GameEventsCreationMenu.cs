using System;
using System.Collections.Generic;
using GameEventsSystem.Events;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace GameEventSystem.Editor
{
	public static class GameEventsCreationMenu
	{
		[MenuItem("Assets/Create/GameEvent", priority = 15)]
		public static void CreateGameEvent()
		{
			EditorApplication.update += OnEditorUpdate;
		}

		private static void OnEditorUpdate()
		{
			EditorApplication.update -= OnEditorUpdate;
			new GameEventCreationDropdown(new AdvancedDropdownState()).Show(new Rect(Vector2.zero, new Vector2(20,200)));
		}
	}


	public class GameEventCreationDropdown : AdvancedDropdown
	{
		private List<Type> types;

		public GameEventCreationDropdown(AdvancedDropdownState state) : base(state)
		{
		}

		protected override AdvancedDropdownItem BuildRoot()
		{
			types = new List<Type>();

			foreach (Type type in TypeCache.GetTypesDerivedFrom<BaseGameEvent>())
			{
				if (type.IsAbstract)
				{
					continue;
				}
				types.Add(type);
			}

			var root = new AdvancedDropdownItem("GameEvents");
			for (var index = 0; index < types.Count; index++)
			{
				var type = types[index];
				var item = new AdvancedDropdownItem(type.Name) { id = index };
				root.AddChild(item);
			}
			return root;
		}

		/*protected override void ItemSelected(AdvancedDropdownItem item)
		{
			AssetDatabase.CreateAsset();
		}*/
	}
}