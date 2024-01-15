using System;
using UnityEngine;

namespace GameEventsSystem.Events
{
	[CreateAssetMenu(menuName = "Scriptable Events/Default", order = 10)]
	public class GameEvent : BaseGameEvent
	{
		public event Action OnRaise;

		public void Raise() => OnRaise?.Invoke();

		public override Type GetParameterType() => null;
		public override void Raise(object value) => Raise();
	}

	public abstract class GameEvent<T> : BaseGameEvent
	{
		public event Action<T> OnRaise;
		public void Raise(T value) => OnRaise?.Invoke(value);

		public override Type GetParameterType() => typeof(T);
		public override void Raise(object value)
		{
			if (value is T tValue)
			{
				Raise(tValue);
			}
			else
			{
				Raise(default);
			}
		}
	}

	public abstract class BaseGameEvent : ScriptableObject
	{
		public abstract Type GetParameterType();
		public abstract void Raise(object value);
	}
}