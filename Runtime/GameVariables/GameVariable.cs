using System;
using GameEventsSystem.Events;
using UnityEngine;

namespace GameEventSystem.GameVariables
{
	public abstract class GameVariable<T> : GameEvent<T>
	{
		[SerializeField] private T StartValue;
		[SerializeField] private T CurrentValue;

		private void OnEnable()
		{
			CurrentValue = StartValue;
			Debug.Log($"{this.name} Init: {CurrentValue}");
		}

		private void OnDisable()
		{
			CurrentValue = StartValue;
			Debug.Log($"{this.name} Terminate: {CurrentValue}");
		}
	}
}