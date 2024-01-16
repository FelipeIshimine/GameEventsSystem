using System;
using System.Collections.Generic;
using System.Text;
using GameEventsSystem.Events;
using ScriptableEventsSystem.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace GameEventSystem.Editor.Events
{
	internal class GameEventInspectorElement : VisualElement
	{
		private readonly List<Object> assetsListFiltered = new List<Object>();
		private readonly List<Object> contextAssetListFiltered = new List<Object>();
		
		private readonly List<Object> allAssetsList = new List<Object>();
		private readonly List<Component> allContextAssetList = new List<Component>();
	
		private VisualElement currentPanel;
		
		private ListView assetsReferencesListView;
		private ListView contextReferencesListView;
		
		private BaseGameEvent myEvent;
	
		private Foldout contextPanelFoldout;
		private string filter = string.Empty;

		private readonly List<Type> types = new();
		private readonly HashSet<Type> typesSet = new();
		private int selectedTypeIndex;
		private DropdownField typeDropdown;
		private VisualElement filtersContainer;
		
		private PropertyField propertyField;
		private VisualElement currentRaiseField;
		private VisualElement buttonContainer;
		private object raiseValue;
		private Action currentRaiseUnregisterCallback;
		private ObjectField scriptField;

		/*public GameEventInspectorElement(bool showScriptField)
		{
			this.ShowScriptField = showScriptField;
			Initialize();
		}*/

		public GameEventInspectorElement(BaseGameEvent gameEvent)
		{
			Initialize();
			SetGameEvent(gameEvent);
		}
		
		private void Initialize()
		{
			scriptField = new ObjectField("Script")
			{
				objectType = typeof(MonoScript),
			};
			scriptField.SetEnabled(false);
			Add(scriptField);
			
			style.flexGrow = 1;

			filtersContainer = new VisualElement()
			{
				style = { flexDirection = FlexDirection.Row}
			};
			
			var toolbarSearchField = new ToolbarSearchField()
			{
				style =
				{
					flexShrink = 1,
					width = -1
				}
			};
			
			
			toolbarSearchField.RegisterValueChangedCallback(OnFilterChange);
			
			filtersContainer.Add(toolbarSearchField);
		
			Add(filtersContainer);
			
			Add(CreateContextPanel());
			Add(CreateAssetsPanel());

			RefreshTypesDropdown();

			RegisterEvents();


			Add(new VisualElement()
			{
				style =
				{
					height = 20
				}
			});
			buttonContainer = new VisualElement()
			{
				style =
				{
					flexDirection = FlexDirection.Row
				}
			};
			
			buttonContainer.Add(new Button(Raise)
			{
				text = "Raise",
				style =
				{
					height = 20,flexGrow = 1,flexShrink = 1
				}
			});
			
			Add(buttonContainer);
		}


		private void RefreshTypesDropdown()
		{
			typeDropdown?.RemoveFromHierarchy();
			
			types.Clear();
			typesSet.Clear();

			foreach (var element in allAssetsList)
			{
				typesSet.Add(element.GetType());
			}
			
			foreach (var element in allContextAssetList)
			{
				typesSet.Add(element.GetType());
			}

			types.AddRange(typesSet);
			types.Sort((x,y) => String.Compare(x.Name, y.Name, StringComparison.Ordinal));

			var names = types.ConvertAll(x => x.Name);
			names.Add("All");
			typeDropdown = new DropdownField(names, names.Count-1, FormatSelectedValueCallback);
			filtersContainer.Add(typeDropdown);
		}

		private string FormatSelectedValueCallback(string arg)
		{
			selectedTypeIndex = types.FindIndex(x => x.Name == arg);
			RefreshFilteredLists();
			return arg;
		}

		private void OnFilterChange(ChangeEvent<string> evt)
		{
			filter = evt.newValue.ToLower();
			RefreshFilteredLists();
		}

		private void RefreshFilteredLists()
		{
			RefreshFilteredContextList();
			RefreshFilteredAssetsList();
		}

		private void RegisterEvents()
		{
			//Debug.Log("RegisterEvents");
			EditorSceneManager.sceneOpened += OnSceneOpened;
			PrefabStage.prefabStageOpened += OnPrefabStageOpened;
			PrefabStage.prefabStageClosing += OnPrefabStageClosing;
			this.RegisterCallback<DetachFromPanelEvent>(UnregisterEvents);
		
			this.UnregisterCallback<AttachToPanelEvent>(RegisterEvents);
		}

		private void UnregisterEvents()
		{
			//Debug.Log("UnregisterEvents");
			this.UnregisterCallback<DetachFromPanelEvent>(UnregisterEvents);
		
			this.RegisterCallback<AttachToPanelEvent>(RegisterEvents);

			EditorSceneManager.sceneOpened -= OnSceneOpened;
			PrefabStage.prefabStageOpened -= OnPrefabStageOpened;
			PrefabStage.prefabStageClosing -= OnPrefabStageClosing;
		}

		private void RegisterEvents(AttachToPanelEvent evt) => RegisterEvents();

		private void UnregisterEvents(DetachFromPanelEvent evt) => UnregisterEvents();

		public virtual void SetGameEvent(BaseGameEvent selectedEvent)
		{
			var parameterType = selectedEvent.GetParameterType();
			if (parameterType != null)
			{
				if (parameterType == typeof(int))
				{
					var field = new IntegerField();
					field.RegisterValueChangedCallback(SetRaiseValue);
					SetRaiseField(field, () => field.UnregisterValueChangedCallback(SetRaiseValue));
				}
				else if (parameterType == typeof(string))
				{
					var field = new TextField()
					{
						style =
						{
							minWidth = 100
						}
					};
					field.RegisterValueChangedCallback(SetRaiseValue);
					SetRaiseField(field, () => field.UnregisterValueChangedCallback(SetRaiseValue));
				}
				else if (parameterType == typeof(float))
				{
					var field = new FloatField();
					field.RegisterValueChangedCallback(SetRaiseValue);
					SetRaiseField(field, () => field.UnregisterValueChangedCallback(SetRaiseValue));
				}
				else if (parameterType == typeof(Vector2))
				{
					var field = new Vector2Field();
					field.RegisterValueChangedCallback(SetRaiseValue);
					SetRaiseField(field, () => field.UnregisterValueChangedCallback(SetRaiseValue));

				}
				else if (parameterType == typeof(Vector3))
				{
					var field = new Vector3Field();
					field.RegisterValueChangedCallback(SetRaiseValue);
					SetRaiseField(field, () => field.UnregisterValueChangedCallback(SetRaiseValue));

				}
				else if (parameterType == typeof(Vector2Int))
				{
					var field = new Vector2IntField();
					field.RegisterValueChangedCallback(SetRaiseValue);
					SetRaiseField(field, () => field.UnregisterValueChangedCallback(SetRaiseValue));

				}
				else if (parameterType == typeof(Vector3Int))
				{
					var field = new Vector3IntField();
					field.RegisterValueChangedCallback(SetRaiseValue);
					SetRaiseField(field, () => field.UnregisterValueChangedCallback(SetRaiseValue));
				}
				else if (parameterType == typeof(bool))
				{
					var field = new Toggle();
					field.RegisterValueChangedCallback(SetRaiseValue);
					SetRaiseField(field, () => field.UnregisterValueChangedCallback(SetRaiseValue));
				}
				else
				{
					var field = new ObjectField()
					{
						style = {flexShrink = 1}
					};
					field.objectType = parameterType;
					field.RegisterValueChangedCallback(SetRaiseValue);
					SetRaiseField(field, () => field.UnregisterValueChangedCallback(SetRaiseValue));
				}
			}
			else
			{
				SetRaiseField(null,null);
			}
			
			myEvent = selectedEvent;
			
			typesSet.Clear();
			
			RefreshContextList();
			RefreshAssetsList();
			
			RefreshFilteredContextList();
			RefreshFilteredAssetsList();

			if (myEvent != null && scriptField !=null)
			{
				scriptField.value = MonoScript.FromScriptableObject(myEvent);
			}
		
		
		}

		private void SetRaiseValue(ChangeEvent<Object> evt) => raiseValue = evt.newValue;
		private void SetRaiseValue(ChangeEvent<float> evt) =>  raiseValue = evt.newValue;
		private void SetRaiseValue(ChangeEvent<string> evt) => raiseValue = evt.newValue;
		private void SetRaiseValue(ChangeEvent<int> evt) => raiseValue = evt.newValue;
		private void SetRaiseValue(ChangeEvent<bool> evt) => raiseValue = evt.newValue;
		private void SetRaiseValue(ChangeEvent<Vector2> evt) => raiseValue = evt.newValue;
		private void SetRaiseValue(ChangeEvent<Vector3> evt) => raiseValue = evt.newValue;
		private void SetRaiseValue(ChangeEvent<Vector2Int> evt) => raiseValue = evt.newValue;
		private void SetRaiseValue(ChangeEvent<Vector3Int> evt) => raiseValue = evt.newValue;
		private void SetRaiseValue(ChangeEvent<Vector4> evt) => raiseValue = evt.newValue;

		private void SetRaiseField(VisualElement nField, Action unregisterCallback)
		{
			currentRaiseField?.RemoveFromHierarchy();
			currentRaiseUnregisterCallback?.Invoke();

			currentRaiseField = nField;
			currentRaiseUnregisterCallback = unregisterCallback;
			raiseValue = null;
			
			if (currentRaiseField != null)
			{
				buttonContainer.Add(currentRaiseField);
			}

		}

		private void Raise() => myEvent.Raise(raiseValue);


		private void OnPrefabStageOpened(PrefabStage obj) => RefreshContextList();

		private void OnPrefabStageClosing(PrefabStage obj) => RefreshContextList();

		private void OnSceneOpened(Scene arg0, OpenSceneMode mode)
		{
			//Debug.Log($"SceneOpened:{arg0} Mode:{mode}");
			RefreshContextList();
		}

		private VisualElement CreateAssetsPanel()
		{
			var container = new Foldout()
			{
				text = "Project References",
				style =
				{
					unityFontStyleAndWeight = FontStyle.BoldAndItalic
				}
			};
		
			assetsReferencesListView = new ListView(assetsListFiltered, 16,MakeAssetReferenceItem, BindAssetReferenceItem)
			{
				style = { flexGrow = 1},
				selectionType = SelectionType.Single,
				delegatesFocus = true
			};
			container.Add(assetsReferencesListView);
			assetsReferencesListView.selectionChanged += PingSelection; 
		
			RefreshAssetsList();
			return container;
		}

		private void PingSelection(IEnumerable<object> obj)
		{
			foreach (object o in obj)
			{
				if (o is Object unityObject)
				{
					EditorGUIUtility.PingObject(unityObject);
					assetsReferencesListView.ClearSelection();
					contextReferencesListView.ClearSelection();
					break;
				}
			}
		}

		private void RefreshAssetsList()
		{
			if (!myEvent)
			{
				return;
			}
		
			allAssetsList.Clear();
			allAssetsList.AddRange(AssetsUtils.GetReferencingObjectsFromAssets(myEvent));
			allAssetsList.Sort(ObjectReferencesSorter);

			RefreshTypesDropdown();

		}

		private void RefreshFilteredAssetsList()
		{
			bool useFilter = !string.IsNullOrEmpty(filter);

			assetsListFiltered.Clear();
			if (useFilter)
			{
				foreach (Object asset in allAssetsList)
				{
					var path = AssetDatabase.GetAssetPath(asset).ToLower();
					if (path.Contains(filter))
					{
						assetsListFiltered.Add(asset);
					}
				}	
			}
			else
			{
				assetsListFiltered.AddRange(allAssetsList);
			}

			if (selectedTypeIndex != -1 && selectedTypeIndex < types.Count)
			{
				var type = types[selectedTypeIndex];

				for (var index = assetsListFiltered.Count - 1; index >= 0; index--)
				{
					var asset = assetsListFiltered[index];
					if (asset.GetType() != type)
					{
						assetsListFiltered.RemoveAt(index);
					}
				}
			}
			
			assetsReferencesListView.ClearSelection();
			assetsReferencesListView.RefreshItems();
		}

		private VisualElement CreateContextPanel()
		{
			contextPanelFoldout = new Foldout()
			{
				text = "Active Context References",
				style =
				{
					unityFontStyleAndWeight = FontStyle.BoldAndItalic
				}
			};

			contextReferencesListView = new ListView(contextAssetListFiltered, 16,MakeContextReferenceItem, BindContextReferenceItem)
			{
				style = { flexGrow = 1},
				selectionType = SelectionType.Single,
				delegatesFocus = true
			};

			contextReferencesListView.selectionChanged += PingSelection;
		
			contextPanelFoldout.Add(contextReferencesListView);

			RefreshContextList();

			return contextPanelFoldout;
		}


		private void RefreshContextList()
		{
			if (!myEvent)
			{
				return;
			}
		
			allContextAssetList.Clear();
			allContextAssetList.AddRange(AssetsUtils.GetReferencingComponentsFromOpenedSContext(myEvent));
			allContextAssetList.Sort(ObjectReferencesSorter);
		
			RefreshTypesDropdown();
		}

		private void RefreshFilteredContextList()
		{
			contextAssetListFiltered.Clear();
			
			if (string.IsNullOrEmpty(filter))
			{
				contextAssetListFiltered.AddRange(allContextAssetList);
			}
			else
			{
				foreach (Component asset in allContextAssetList)
				{
					if (GetHierarchy(asset.transform).ToLower().Contains(filter))
					{
						contextAssetListFiltered.Add(asset);
					}
				}
			}

			if (selectedTypeIndex != -1 && selectedTypeIndex < types.Count)
			{
				var type = types[selectedTypeIndex];
				
				Debug.Log(type.Name);
				
				for (int i = contextAssetListFiltered.Count - 1; i >= 0; i--)
				{
					if (contextAssetListFiltered[i].GetType() != type)
					{
						contextAssetListFiltered.RemoveAt(i);
					}
				}
			}
			
			contextReferencesListView.ClearSelection();
			contextReferencesListView.RefreshItems();
		}

		private int ObjectReferencesSorter(Object x, Object y) => String.Compare(x.GetType().Name, y.GetType().Name, StringComparison.Ordinal);

		private VisualElement MakeContextReferenceItem()
		{
			VisualElement container = new VisualElement()
			{
				style =
				{
					flexDirection = FlexDirection.Row,
					justifyContent = Justify.SpaceBetween
				},
			};
			container.Add(new Label()
			{
				name = "label",
				style = 
				{
					flexGrow = 1,
					flexShrink = 1,
					overflow = Overflow.Hidden
				}
			});
			container.Add(new Label()
			{
				name = "type",
				style =
				{
					color = Color.cyan,
					fontSize = 10,
					unityTextAlign = TextAnchor.MiddleCenter,
					unityFontStyleAndWeight = FontStyle.Italic,
					overflow = Overflow.Hidden
				}
			});
		
			container.Add(new Button()
			{
				name = "button",
				style = { height = 16}
			});
		
			return container;
		}

		private void BindContextReferenceItem(VisualElement visualElement, int index)
		{
			Label label = visualElement.Q<Label>("label");
			var refObject = contextAssetListFiltered[index];
		
			if (refObject is Component component)
			{
				label.text = GetHierarchy(component.transform);
			}
			else
			{
				label.text = refObject.name;
			}
		
			Label typeLabel = visualElement.Q<Label>("type");
			typeLabel.text = refObject.GetType().Name;
		
			Button button = visualElement.Q<Button>("button");
			button.text = "Select";
			button.clicked += ()=>
			{
				EditorGUIUtility.PingObject(refObject);
				Selection.SetActiveObjectWithContext(refObject, null);
				if (SceneView.currentDrawingSceneView)
				{
					SceneView.currentDrawingSceneView.FrameSelected(true);
				}
			};
		}

		private static string GetHierarchy(Transform transform)
		{
			StringBuilder builder = new StringBuilder();
			var root = transform;
			builder.Append($"/{root.name}");
			while (root.parent != null)
			{
				root = root.parent;
				builder.Insert(0, $"/{root.name}");
			}
			builder.Insert(0, root.gameObject.scene.name);
			var hierarchy = builder.ToString();
			return hierarchy;
		}

		private VisualElement MakeAssetReferenceItem()
		{
			
			VisualElement container = new VisualElement()
			{
				style =
				{
					flexDirection = FlexDirection.Row,
					justifyContent = Justify.SpaceBetween
				},
			};
			container.Add(new Label()
			{
				name = "label",
				style =
				{
					flexGrow = 1,
					flexShrink = 1,
					overflow = Overflow.Hidden
				}
			});
			container.Add(new Label()
			{
				name = "type",
				style =
				{
					color = Color.cyan,
					fontSize = 10,
					unityTextAlign = TextAnchor.MiddleCenter,
					unityFontStyleAndWeight = FontStyle.Italic,
					overflow = Overflow.Hidden
				}
			});
		
			container.Add(new Button()
			{
				name = "button",
				style = { height = 16}
			});
		
			return container;
		}

		private void BindAssetReferenceItem(VisualElement visualElement, int index)
		{
			Label label = visualElement.Q<Label>("label");
			var refObject = assetsListFiltered[index];

			label.text = AssetDatabase.GetAssetPath(refObject);
		
			Label typeLabel = visualElement.Q<Label>("type");
			typeLabel.text = refObject.GetType().Name;
		
			Button button = visualElement.Q<Button>("button");
			button.text = "Open";
			button.clicked += ()=>
			{
				AssetDatabase.OpenAsset(refObject);
				RefreshContextList();
			};
		}
	}
}