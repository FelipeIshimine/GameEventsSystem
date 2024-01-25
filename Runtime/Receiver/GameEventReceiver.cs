using System;
using GameEventsSystem.Events;
using UnityEngine;
using UnityEngine.Events;

namespace GameEventSystem.Receiver
{
	[System.Serializable]
	public class GameEventReceiver<T, TB> : BaseScriptableEventReceiver where T : GameEvent<TB>
	{
		[field:SerializeField] public T TargetEvent { get; set; }
		[field:SerializeField] public UnityEvent<TB> Reaction { get; set; }

		private void OnEnable() => TargetEvent.OnRaise += React;

		private void OnDisable() => TargetEvent.OnRaise -= React;
		private void React(TB value) => Reaction?.Invoke(value);
	}
	
	[System.Serializable]
	public class GameEventReceiver<T> : BaseScriptableEventReceiver
	{
		[field:SerializeField] public GameEvent<T> TargetEvent { get; set; }
		[field:SerializeField] public UnityEvent<T> Reaction { get; set; }

		private void OnEnable() => TargetEvent.OnRaise += React;

		private void OnDisable() => TargetEvent.OnRaise -= React;
		private void React(T value) => Reaction?.Invoke(value);
	}

	[System.Serializable]
	public class GameEventReceiver : BaseScriptableEventReceiver
	{
		public event Action OnRaise { add => Target.OnRaise+= value; remove => Target.OnRaise -= value; }
		public GameEvent Target;
		public Optional<uint> TriggerLimit = new Optional<uint>(1, false);
		public UnityEvent Reactions;

		public int Counter { get; private set; }
		
		private void OnEnable() => Target.OnRaise += React;

		private void OnDisable() => Target.OnRaise -= React;

		private void React()
		{
			if (TriggerLimit && TriggerLimit >= Counter)
			{
				return;
			}
			Counter++;
			Reactions?.Invoke();
		}
	}
}