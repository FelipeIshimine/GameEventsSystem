using System;
using UnityEngine;

namespace GameEventsSystem.Editor
{
	
	/// <summary>
	/// Automaticli Assigns the event with the provided name.
	/// Creates one if cannot find it. 
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class AutoEvent : PropertyAttribute
	{
		public readonly string EventName;

		public AutoEvent()
		{
			EventName = null;
		}
		public AutoEvent(string eventName)
		{
			EventName = eventName;
		}
	}
}