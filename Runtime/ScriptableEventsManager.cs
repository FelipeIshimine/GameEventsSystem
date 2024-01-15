using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScriptableEventsManager : MonoBehaviour
{
	public static ScriptableEventsManager Instance { get; private set; }
	
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Init()
	{
		GameObject go = new GameObject();
		DontDestroyOnLoad(go);
		go.AddComponent<ScriptableEventsManager>();
	}

	private void Awake()
	{
		if (Instance)
		{
			Debug.LogError($"Singleton Error.Another instance of {nameof(ScriptableEventsManager)}. This component will be destroyed.", gameObject);
			Destroy(this);
			return;
		}
		Instance = this;
	}
}
