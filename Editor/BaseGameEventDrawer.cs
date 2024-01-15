using System.Reflection;
using ScriptableEventsSystem.Events;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UIElements;

namespace ScriptableEventsSystem.Editor
{
	[CustomPropertyDrawer(typeof(BaseGameEvent), true)]
	public class BaseGameEventDrawer : PropertyDrawer 
	{
		private Button button;
		private TextField textField;
		private UnityEngine.UIElements.Image icon;

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var eventNameAttribute = fieldInfo.GetCustomAttribute<AutoEvent>();
			
			bool isFieldEnabled = true;
			if (eventNameAttribute != null)
			{
				var eventName = eventNameAttribute.EventName ?? property.displayName;

				if (property.objectReferenceValue == null || property.objectReferenceValue.name != eventName)
				{
					var targetEvent = GameEventsEditorUtils.FindOrCreateTargetEvent(eventName, fieldInfo.FieldType);
					SetValue(property, targetEvent);
				}
				isFieldEnabled = false;
			}

			
			VisualElement container = new VisualElement()
			{
				style =
				{
					flexDirection = FlexDirection.Row,
					alignItems = Align.Center
				}
			};
			
			container.Add(new Label(property.displayName)
			{
				style =
				{
					flexGrow = 1,
				}
			});

			VisualElement rightContainer = new VisualElement()
			{
				style =
				{
					flexDirection = FlexDirection.Row ,
					justifyContent = Justify.SpaceBetween,
					//flexGrow = 1
				}
			};
			container.Add(rightContainer);

			icon = new UnityEngine.UIElements.Image
			{
				style =
				{
					height = 16,
					width = 16,
					minWidth = 16
				}
			};
			
			icon.image =	RefreshIcon(property);
			
			textField = new TextField()
			{
				value = property.objectReferenceValue != null ? $"{property.objectReferenceValue.name} ({property.objectReferenceValue.GetType().Name})" : "None",
				textEdition =
				{
					isReadOnly = true,
				},
				textSelection =
				{
					isSelectable = false
				},
				style =
				{
					flexShrink = 1,
				}
			};
	
			textField.ElementAt(0).Insert(0, icon);
			
			textField.AddManipulator(new Clickable(()=> EditorGUIUtility.PingObject(property.objectReferenceValue)));
			rightContainer.Add(textField);
			
			button = new Button(()=>
			{
				void Callback(ScriptableObject x)
				{
					SetValue(property, x);
				}
				var dropdown = new ScriptableObjectSearchDropdown(new AdvancedDropdownState(),fieldInfo.FieldType, Callback);
				dropdown.Show(new Rect(textField.worldBound.position, button.worldBound.size + textField.worldBound.size));
			})
			{
				style = { height = 16}
			};
			button.Add(new UnityEngine.UIElements.Image()
			{
				image = EditorGUIUtility.IconContent("d_icon dropdown").image
			});
		
			rightContainer.Add(button);
	
			//rightContainer.Add(button);
			rightContainer.SetEnabled(isFieldEnabled);
			return container;
		}

		private static Texture RefreshIcon(SerializedProperty property)
		{
			return property.objectReferenceValue != null?EditorGUIUtility.IconContent("d_ScriptableObject Icon").image:null;
		}

		private void SetValue(SerializedProperty property, ScriptableObject targetEvent)
		{
			property.objectReferenceValue = targetEvent;
			property.serializedObject.ApplyModifiedPropertiesWithoutUndo();
			textField.value = targetEvent!= null?$"{targetEvent.name} ({targetEvent.GetType().Name})":$"None ({fieldInfo.FieldType.Name})";
			icon.image = RefreshIcon(property);
		}
	}
}