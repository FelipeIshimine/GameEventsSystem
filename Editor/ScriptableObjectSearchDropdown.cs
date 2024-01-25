using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace GameEventsSystem.Editor
{
	public class ScriptableObjectSearchDropdown : AdvancedDropdown
	{
		private readonly Type type;
		private readonly Action<ScriptableObject> callback;
		private List<ScriptableObject> assets;

		public ScriptableObjectSearchDropdown(AdvancedDropdownState state, Type type, Action<ScriptableObject> callback) : base(state)
		{
			this.type = type;
			this.callback = callback;
		}

		protected override AdvancedDropdownItem BuildRoot()
		{
			var guids = AssetDatabase.FindAssets($"t:{type.Name}");

			assets = new List<ScriptableObject>();
			foreach (string guid in guids)
			{
				var asset = AssetDatabase.LoadAssetAtPath<ScriptableObject>(AssetDatabase.GUIDToAssetPath(guid));

				var assetType = asset.GetType();
				if (assetType == type || type.IsAssignableFrom(assetType))
				{
					assets.Add(asset);
				}
			}

			AdvancedDropdownItem root = new AdvancedDropdownItem($"{type.Name}");

			var noneValue = new AdvancedDropdownItem("None");
			noneValue.id = -1;
			root.AddChild(noneValue);
			assets.Sort((x,y) => String.Compare(x.name, y.name, StringComparison.Ordinal));


			for (var index = 0; index < assets.Count; index++)
			{
				var scriptableEvent = assets[index];
				var item = new AdvancedDropdownItem(scriptableEvent.name)
				{
					id = index
				};
				root.AddChild(item);
			}

			return root;
		}

		protected override void ItemSelected(AdvancedDropdownItem item)
		{
			if (item.id == -1)
			{
				callback?.Invoke(null);
			}
			else
			{
				callback?.Invoke(assets[item.id]);
			}
		}
	}
}