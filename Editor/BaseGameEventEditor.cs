using GameEventsSystem.Events;
using GameEventSystem.Editor.Events;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameEventSystem.Editor
{
	[CustomEditor(typeof(BaseGameEvent),true)]
	public class BaseGameEventEditor : UnityEditor.Editor
	{
		private BaseGameEvent gameEvent;
		private GameEventInspectorElement inspectorElement;

		private void OnEnable()
		{
			gameEvent = (BaseGameEvent)target;
		}

		private void OnDisable()
		{
			inspectorElement?.RemoveFromHierarchy();
		}

		public override VisualElement CreateInspectorGUI()
		{
			inspectorElement ??=  new GameEventInspectorElement(gameEvent);
			return inspectorElement;
		}
	}
}