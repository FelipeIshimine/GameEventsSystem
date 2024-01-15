using GameEventsSystem.Events;
using ScriptableEventsSystem.Editor.Events;
using ScriptableEventsSystem.Events;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ScriptableEventsSystem.Editor
{
	[CustomEditor(typeof(BaseGameEvent),true)]
	public class BaseGameEventEditor : UnityEditor.Editor
	{
		private BaseGameEvent gameEvent;

		private GameEventInspectorElement gameEventInspectorElement;

		private void OnEnable()
		{
			gameEvent = (BaseGameEvent)target;
		}

		private void OnDisable()
		{
			gameEventInspectorElement?.RemoveFromHierarchy();
		}

		public override VisualElement CreateInspectorGUI()
		{
			gameEventInspectorElement ??=  new GameEventInspectorElement();
			gameEventInspectorElement.SetGameEvent(gameEvent);
			return gameEventInspectorElement;
		}

	}
}