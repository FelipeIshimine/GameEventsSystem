using GameEventsSystem.Events;
using GameEventSystem.Editor.Events;
using GameEventSystem.GameVariables;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
	[CustomEditor(typeof(GameVariable<>),true)]
	public class GameVariableEditor : BaseGameEventEditor
	{
		private VisualElement inspectorElement;

		/*public override VisualElement CreateInspectorGUI()
		{
			VisualElement valueContainer = new VisualElement()
			{
				style = { flexDirection = FlexDirection.Row }
			};

			if (Application.isPlaying)
			{
				var currentValueProperty = serializedObject.FindProperty("StartValue");
				var field = new PropertyField(currentValueProperty)
				{
					style =
					{
						flexGrow = 1,
						flexShrink = 1
					}
				};
				field.label = "Value";
				valueContainer.Add(field);
			}
			else
			{
				var currentValueProperty = serializedObject.FindProperty("CurrentValue");
				var field = new PropertyField(currentValueProperty)
				{
					style =
					{
						flexGrow = 1,
						flexShrink = 1
					}
				};
				field.label = "Value";
				valueContainer.Add(field);
			}

			var inspectorGUI = base.CreateInspectorGUI();

			inspectorGUI.Insert(1,valueContainer);

			return inspectorGUI;
		}*/

		public override VisualElement CreateInspectorGUI()
		{
			Debug.Log(nameof(GameVariableEditor));
			inspectorElement ??=  new GameVariableInspectorElement((BaseGameEvent)target);
			//inspectorElement.SetGameEvent((BaseGameEvent)target);
			return inspectorElement;
		}
	}
}