using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameEventsSystem.Events;
using GameEventSystem.Editor;
using GameEventSystem.Editor.Events;
using GameEventSystem.GameVariables;
using ScriptableEventsSystem.Editor.Settings;
using ScriptableEventsSystem.Events;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ScriptableEventsSystem.Editor
{
	public class GameEventsWindow : EditorWindow
	{
		public const string EVENTS_FOLDER = "Assets/ScriptableEvents/";
		private readonly List<BaseGameEvent> allEvents = new List<BaseGameEvent>();
		private readonly List<BaseGameEvent> filteredEvents = new List<BaseGameEvent>();
		private readonly List<BaseGameEvent> selectedEvents = new List<BaseGameEvent>();
		private VisualElement leftPane;
		private VisualElement rightPane;
		private ListView filteredEventsListView;
		private readonly List<(Type type, string name)> scriptableEventTypes = new();

		private string filter = string.Empty;
		private int filterTypeIndex;

		private readonly Dictionary<BaseGameEvent, VisualElement> eventToVisualElement = new();
		private VisualElement eventInspector;
		private ScriptableEventsSettings settings;

		private bool isEditingEventName;
		private VisualElement rightPanelContainer;
		private Label eventTitle;
		private Label typeLabel;
		private UnityEditor.Editor cacheEditor;

		[Shortcut(ScriptableEventsSettings.WINDOW_SHORTCUT_ID), MenuItem("Windows/Scriptable Events")]
		public static void Open()
		{
			GetWindow<GameEventsWindow>().Show();
		}

		private void OnEnable()
		{
			settings = ScriptableEventsSettings.GetOrCreateSettings();
			allEvents.Clear();
			var guids = AssetDatabase.FindAssets($"t:{typeof(BaseGameEvent)}");

			foreach (string guid in guids)
			{
				var baseEvent = AssetDatabase.LoadAssetAtPath<BaseGameEvent>(AssetDatabase.GUIDToAssetPath(guid));
				allEvents.Add(baseEvent);
			}
			allEvents.Sort(EventSorter);

		
		
			foreach (Type type in TypeCache.GetTypesDerivedFrom<BaseGameEvent>())
			{
				if(type.IsAbstract) continue;
				var text = type.Name.Replace("ScriptableEvent", string.Empty);
				if (string.IsNullOrEmpty(text))
				{
					text = "-";
				}
				scriptableEventTypes.Add((type, text));
			}
		
			scriptableEventTypes.Sort(TypeSorter);

			scriptableEventTypes.Add((null,"All"));
		
		}

		private int TypeSorter((Type type, string Name) x, (Type type, string Name) y)
		{
			return String.Compare(x.Name, y.Name, StringComparison.Ordinal);
		}

		private int EventSorter(BaseGameEvent x, BaseGameEvent y) => String.Compare(x.name, y.name, StringComparison.Ordinal);

		private void CreateGUI()
		{
			leftPane = new VisualElement();
			rightPane = new VisualElement();
		        
			TwoPaneSplitView eventSelectionSplitView = new TwoPaneSplitView(0,200, TwoPaneSplitViewOrientation.Horizontal);
			eventSelectionSplitView.Add(leftPane);
			eventSelectionSplitView.Add(rightPane);


			VisualElement filtersContainer = new VisualElement()
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

		
			var typeDropdown = new DropdownField(scriptableEventTypes.ConvertAll(x=>x.name), scriptableEventTypes.Count-1, FormatSelectedValueCallback);
			filtersContainer.Add(typeDropdown);
		
			leftPane.Add(filtersContainer);

		
			filteredEventsListView = new ListView(filteredEvents, 16, MakeListItem, BindListItem)
			{
				selectionType = SelectionType.Single,
				style = { flexGrow = 1}
			};
			filteredEventsListView.selectionChanged += OnEventSelection;

			filteredEventsListView.AddManipulator(new ContextualMenuManipulator(ListRightClick));
			
			/*
			filteredEventsListView.RegisterCallback<FocusInEvent>(OnListFocusIn);
			filteredEventsListView.RegisterCallback<FocusOutEvent>(OnListFocusOut);
			*/
			
			
			leftPane.Add(filteredEventsListView);

			rootVisualElement.Add(eventSelectionSplitView);
			RefreshFilteredList();
		}

		/*
		private void OnListFocusIn(FocusInEvent evt)
		{
			filteredEventsListView.RegisterCallback<KeyDownEvent>(OnListKeyDown);
		}

		private void OnListFocusOut(FocusOutEvent evt)
		{
			filteredEventsListView.UnregisterCallback<KeyDownEvent>(OnListKeyDown);
		}

		private void OnListKeyDown(KeyDownEvent evt)
		{
			if (evt.keyCode == KeyCode.F2)
			{
				BaseScriptableEvent scriptableEvent = filteredEventsListView.selectedItem as BaseScriptableEvent;
				if (scriptableEvent)
				{
					filteredEventsListView.Blur();
					eventToVisualElement[scriptableEvent].Q<EditableTextField>().SetEditing();
					eventToVisualElement[scriptableEvent].Focus();
				}
			}
			Debug.Log($"List Key Down:{evt.keyCode} {evt.eventTypeId} Target:{evt.target}");
		}*/


		private string FormatSelectedValueCallback(string selectedName)
		{
			filterTypeIndex = scriptableEventTypes.FindIndex(x => x.name == selectedName);

			RefreshFilteredList();
		
			return selectedName;
		}

		private void ListRightClick(ContextualMenuPopulateEvent evt)
		{
			Debug.Log("Right CLick");

			foreach (Type type in TypeCache.GetTypesDerivedFrom<BaseGameEvent>())
			{
				if (type.IsAbstract)
				{
					continue;
				}
				var optionName = type.Name.Replace("ScriptableEvent", string.Empty);
				evt.menu.AppendAction($"Create Event {optionName}", _ => CreateEvent(type));
			}
		}

		private void CreateEvent(Type type)
		{
			var scriptableEvent = (BaseGameEvent)CreateInstance(type);
			string filePath = EVENTS_FOLDER + $"{type.Name} {allEvents.Count}.asset";

			Debug.Log(filePath);
			AssetDatabase.CreateAsset(scriptableEvent,filePath);
			allEvents.Add(scriptableEvent);
			allEvents.Sort(EventSorter);
			RefreshFilteredList();
		}

		private void OnFilterChange(ChangeEvent<string> evt)
		{
			filter = evt.newValue;
			Debug.Log(filter);
			RefreshFilteredList();
		}

		private void RefreshFilteredList()
		{
			filteredEvents.Clear();
		
			bool useTextFilter = !string.IsNullOrEmpty(filter);
			filter = filter.ToLower();
			if (useTextFilter)
			{
				foreach (BaseGameEvent baseScriptableEvent in allEvents)
				{
					if (baseScriptableEvent.name.ToLower().Contains(filter))
					{
						filteredEvents.Add(baseScriptableEvent);
						Debug.Log($"Filter: {baseScriptableEvent.name}");
					}
				}
			}
			else
			{
				filteredEvents.AddRange(allEvents);
			}
		
		
			bool useTypeFilter = (filterTypeIndex > -1 && filterTypeIndex < scriptableEventTypes.Count-1);

			if(useTypeFilter)
			{
				var filterType = scriptableEventTypes[filterTypeIndex];
				for (int i = filteredEvents.Count - 1; i >= 0; i--)
				{
					if (filteredEvents[i].GetType() !=  filterType.type)
					{
						filteredEvents.RemoveAt(i);
					}
				}
			}
		
		
			filteredEventsListView?.RefreshItems();
		}


		private void OnEventSelection(IEnumerable<object> obj)
		{
			selectedEvents.Clear();
			foreach (BaseGameEvent baseScriptableEvent in obj)
			{
				selectedEvents.Add(baseScriptableEvent);

				if (settings.pingEventAssetWhenSelected)
				{
					EditorGUIUtility.PingObject(baseScriptableEvent);
				}	
			}
			
			UpdateSelectedEvents();
		}

		private void UpdateSelectedEvents()
		{
			rightPane.Clear();

			foreach (var selectedEvent in selectedEvents)
			{
				if (selectedEvent != null)
				{
					rightPane.Add(CreateRightPanel(selectedEvent));
				}
				else
				{
					Close();
					Open();
				}
			}
			
			/*
			try
			{
				foreach (var selectedEvent in selectedEvents)
				{
					if (selectedEvent != null)
					{
						rightPane.Add(CreateRightPanel(selectedEvent));
					}
					else
					{
						Close();
						Open();
					}
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning(e.StackTrace);
				rootVisualElement.Clear();
				CreateGUI();
			}*/
		}

		private VisualElement CreateRightPanel(BaseGameEvent selectedEvent)
		{
			if (rightPanelContainer == null)
			{
				rightPanelContainer ??= new VisualElement();
				
				var titleContainer = new VisualElement()
				{
					style =
					{
						flexShrink = 1,
						flexDirection = FlexDirection.Row,
						justifyContent = Justify.Center,
						paddingBottom = 10,
						paddingLeft = 5,
						paddingTop = 5,
						paddingRight = 5,
						overflow = Overflow.Hidden
					}
				};
			
				eventTitle =  new Label("title")
				{
					style =
					{
						flexGrow = 0,
						justifyContent = Justify.Center,
						unityFontStyleAndWeight = FontStyle.Bold,
						unityTextAlign = TextAnchor.MiddleCenter,
						fontSize = 20
					},
				};
				titleContainer.Add(eventTitle);

				typeLabel = new Label("type")
				{
					style =
					{
						flexGrow = 0,
						justifyContent = Justify.FlexEnd,
						unityFontStyleAndWeight = FontStyle.Italic,
						unityTextAlign = TextAnchor.MiddleRight,
						fontSize = 16,
						color = Color.cyan
					},
				};
				titleContainer.Add(typeLabel);
				rightPanelContainer.Add(titleContainer);
			}

			eventInspector?.RemoveFromHierarchy();

			var eventType = selectedEvent.GetType();
			var gameVariableType = typeof(GameVariable<>);
			
			if (IsSubclassOfRawGeneric(eventType,gameVariableType))
			{
				eventInspector = new GameVariableInspectorElement(selectedEvent, false);
				//eventInspector.styleSheets("unity-property-field__inspector-property");
				//rightPanelContainer.Add(eventInspector);
				rootVisualElement.Add(eventInspector);
				
				eventInspector.Insert(0,new IMGUIContainer(() =>
				{
					//UnityEditor.Editor.CreateCachedEditor(selectedEvent, typeof(BaseGameEventEditor), ref cacheEditor);
					UnityEditor.Editor.CreateCachedEditor(selectedEvent, typeof(GameVariableEditor), ref cacheEditor);
					cacheEditor.OnInspectorGUI();
				}));
			}
			else
			{
				var gameEventInspectorElement = new GameEventInspectorElement(selectedEvent, true);
				eventInspector = gameEventInspectorElement;
				rightPanelContainer.Add(eventInspector);
			}
			
			
			eventTitle.text = selectedEvent.name;
			var parameterType = selectedEvent.GetParameterType();

			if (parameterType != null)
			{
				typeLabel.visible = true;
				typeLabel.text = parameterType.Name;
			}
			else
			{
				typeLabel.visible = false;
				typeLabel.text = string.Empty;
			}
	
			return rightPanelContainer;
		}


		private VisualElement MakeListItem()
		{
			VisualElement container = new VisualElement()
			{
				style =
				{
					flexDirection = FlexDirection.Row,
					justifyContent = Justify.SpaceBetween
				
				}
			};
			var label = new EditableTextField("label")
			{
				name = "label",
				style = 
				{ 
					height = 20,
					flexShrink = 1,
					overflow = Overflow.Hidden 
				},
			};
			var type = new Label("type")
			{
				name = "type",
				style =
				{
					height = 20 ,
					color = Color.cyan,
					unityFontStyleAndWeight = FontStyle.Italic,
					fontSize = 10,
					overflow = Overflow.Hidden,
				},
			};
			container.Add(label);
			container.Add(type);
			return container;
		}

		

		private void BindListItem(VisualElement visualElement, int index)
		{
			var baseScriptableEvent = filteredEvents[index];

			if (baseScriptableEvent == null)
			{
				Debug.Log("Null");
				return;
			}
		
			var labelField = visualElement.Q<EditableTextField>("label");
			labelField.Text = baseScriptableEvent.name;
			labelField.OnTextCallback += newName => Rename(baseScriptableEvent,newName);
		
			var typeLabel = visualElement.Q<Label>("type");
			var parameterType = baseScriptableEvent.GetParameterType();

			typeLabel.text = parameterType == null ? string.Empty : parameterType.Name;
		
			eventToVisualElement[baseScriptableEvent] = visualElement;
		}

		private async void Rename(BaseGameEvent baseScriptableEvent, string newName)
		{
			AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(baseScriptableEvent), newName);
			AssetDatabase.SaveAssets();

			allEvents.Sort(EventSorter);
			filteredEventsListView.RefreshItems();
			filteredEventsListView.Focus();
			await Task.Yield();
			isEditingEventName = false;
		}
		
		public static bool IsSubclassOfRawGeneric(Type toCheck, Type baseType)
		{
			while (toCheck != typeof(object))
			{
				Type cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
				if (baseType == cur)
				{
					return true;
				}

				toCheck = toCheck.BaseType;
			}

			return false;
		}
	}
}