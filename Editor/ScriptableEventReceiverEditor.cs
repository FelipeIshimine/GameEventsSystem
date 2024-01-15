using System.Reflection;
using GameEventsSystem.Events;
using ScriptableEventsSystem.Events;
using ScriptableEventsSystem.Receiver;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ScriptableEventsSystem.Editor
{
	[CustomPropertyDrawer(typeof(ScriptableEventReceiver))]
	public class ScriptableEventReceiverEditor : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			ScriptableEventReceiver receiver = (ScriptableEventReceiver)fieldInfo.GetValue(property.serializedObject.targetObject);


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
			var targetField = new PropertyField(property.FindPropertyRelative(nameof(ScriptableEventReceiver.Target)), string.Empty);
			targetField.SetEnabled(isTargetFieldEnable);
			container.Add(targetField);
			container.Add(new PropertyField(property.FindPropertyRelative(nameof(ScriptableEventReceiver.TriggerLimit))));
			container.Add(new PropertyField(property.FindPropertyRelative(nameof(ScriptableEventReceiver.Reactions))));

			return container;
		}

	
	}
}