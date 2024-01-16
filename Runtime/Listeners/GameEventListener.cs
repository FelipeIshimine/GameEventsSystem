using GameEventSystem.Receiver;
using ScriptableEventsSystem.Listeners;
using UnityEngine;

namespace GameEventSystem.Listeners
{
	public class GameEventListener : BaseGameEventListener
	{
		[field:SerializeField] public GameEventReceiver Receiver { get; private set; }

		private void OnEnable() => Receiver.Target.OnRaise += React;

		private void OnDisable() => Receiver.Target.OnRaise -= React;

		private void React() => Receiver.Reactions?.Invoke();
	}
	public abstract class GameEventListener<T> : BaseGameEventListener
	{
		[field: SerializeField] public GameEventReceiver<T> Receiver { get; private set; } = new ();

		private void OnEnable() => Receiver.TargetEvent.OnRaise += React;

		private void OnDisable() => Receiver.TargetEvent.OnRaise -= React;

		private void React(T value) => Receiver.Reaction?.Invoke(value);
	}
	/*
	public abstract class ScriptableEventListener<T,TB> : BaseScriptableEventListener where T : ScriptableEvent<TB>, new()
	{
		[field: SerializeField] public ScriptableEventReceiver<T, TB> Receiver { get; private set; } = new ();

		private void OnEnable() => Receiver.TargetEvent.OnRaise += React;

		private void OnDisable() => Receiver.TargetEvent.OnRaise -= React;

		private void React(TB value) => Receiver.Reaction?.Invoke(value);
	}*/
}