﻿using ScriptableEventsSystem.Events;
using ScriptableEventsSystem.Receiver;
using UnityEngine;

namespace ScriptableEventsSystem.Listeners
{
	public class ScriptableEventListener : BaseScriptableEventListener
	{
		[field:SerializeField] public ScriptableEventReceiver Receiver { get; private set; }

		private void OnEnable() => Receiver.Target.OnRaise += React;

		private void OnDisable() => Receiver.Target.OnRaise -= React;

		private void React() => Receiver.Reactions?.Invoke();
	}
	public abstract class ScriptableEventListener<T> : BaseScriptableEventListener
	{
		[field: SerializeField] public ScriptableEventReceiver<T> Receiver { get; private set; } = new ();

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