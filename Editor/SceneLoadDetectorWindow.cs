using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ScriptableEventsSystem.Editor
{
	public class SceneLoadDetectorWindow : EditorWindow
	{
		[MenuItem("Window/Scene Load Detector")]
		public static void OpenWindow()
		{
			GetWindow<SceneLoadDetectorWindow>("Scene Load Detector");
		}

		private void OnEnable()
		{
			EditorSceneManager.sceneOpened += OnSceneOpened;
		}

		private void OnDisable()
		{
			EditorSceneManager.sceneOpened -= OnSceneOpened;
		}

		private void OnSceneOpened(Scene scene, OpenSceneMode mode)
		{
			Debug.Log("Scene opened: " + scene.name);
			// Your logic when a scene is loaded
		}

		private void OnGUI()
		{
			// Your Editor Window GUI code
		}
	}
}