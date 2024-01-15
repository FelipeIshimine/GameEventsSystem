using ScriptableEventsSystem.Events;
using ScriptableEventsSystem.Receiver;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ScriptableEventsSystem.Editor
{
	/*[CustomPropertyDrawer(typeof(ScriptableEventReceiver))]
	public class ScriptableEventReceiverDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement container = new VisualElement()
			{
				style = { flexGrow = 1}
			};
			Debug.Log(property.serializedObject);
			ScriptableEventReceiver instance = fieldInfo.GetValue(property.serializedObject.targetObject) as ScriptableEventReceiver;

			//container.Add(new PropertyField(property.FindPropertyRelative(nameof(target.Target))));

			container.Add(new PropertyField(property.FindPropertyRelative(nameof(instance.Target))));
			container.Add(new PropertyField(property.FindPropertyRelative(nameof(instance.Reaction))));
			
			return new PropertyField(property.FindPropertyRelative(nameof(instance.Target)));
		}
	}*/
}