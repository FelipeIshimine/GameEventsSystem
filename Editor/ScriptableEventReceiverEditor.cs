using System.Reflection;
using GameEventsSystem.Events;
using GameEventSystem.Receiver;
using GameEventsSystem.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
	[CustomPropertyDrawer(typeof(GameEventReceiver))]
	public class ScriptableEventReceiverEditor : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			GameEventReceiver receiver = (GameEventReceiver)fieldInfo.GetValue(property.serializedObject.targetObject);


			var eventNameAttribute = fieldInfo.GetCustomAttribute<AutoEvent>();
			bool isTargetFieldEnable = true;
			if (eventNameAttribute != null)
			{
				var eventName = eventNameAttribute.EventName;
				if (eventName == null)
				{
					eventName = property.displayName;
				}
				
				if (receiver.Target == null || receiver.Target.name != eventName)
				{
					var targetEvent = GameEventsEditorUtils.FindOrCreateTargetEvent<GameEvent>(eventName);

					receiver.Target = targetEvent;
				}
				isTargetFieldEnable = false;
			}
			
			
			Foldout container = new Foldout()
			{
				text = property.displayName
			};
			var targetField = new PropertyField(property.FindPropertyRelative(nameof(GameEventReceiver.Target)), string.Empty);
			targetField.SetEnabled(isTargetFieldEnable);
			container.Add(targetField);
			container.Add(new PropertyField(property.FindPropertyRelative(nameof(GameEventReceiver.TriggerLimit))));
			container.Add(new PropertyField(property.FindPropertyRelative(nameof(GameEventReceiver.Reactions))));

			return container;
		}

	
	}
}