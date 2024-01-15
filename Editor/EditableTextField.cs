using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace ScriptableEventsSystem.Editor
{
	public class EditableTextField : VisualElement
	{
		public event Action<string> OnTextCallback;
		private Label displayLabel;
		private TextField editField;

		private bool isEditing = true;
		public string Text
		{
			get => displayLabel.text;
			set => displayLabel.text = value;
		}

		public bool IsEditing => isEditing;

		public EditableTextField(string initialText) : this(initialText,null)
		{
		}
		public EditableTextField(string initialText, Action<string> onTextCallback)
		{
			this.focusable = false;
			this.OnTextCallback = onTextCallback;
			displayLabel = new Label(initialText)
			{
				style =
				{
					unityTextAlign = TextAnchor.MiddleLeft,
					height = 20
				},
				focusable = false
			};
			editField = new TextField
			{
				value = initialText,
				style =
				{
					unityTextAlign = TextAnchor.MiddleLeft,
					height = 16
				},
				delegatesFocus = true,
				focusable = true,
				selectAllOnFocus = true,
				selectAllOnMouseUp = true,
			};

			editField.RegisterValueChangedCallback(OnTextEdit);
			editField.RegisterCallback<AttachToPanelEvent>(OnEditFieldAttach);
			
			style.flexDirection = FlexDirection.Row;

			Add(editField);
			SetNormal();
			
			/*RegisterCallback<FocusOutEvent>(_ => Debug.Log("FocusOutEvent"));
			*/
			/*RegisterCallback<FocusInEvent>(_ =>
			{
				Debug.Log($"FocusInEvent:{focusController.focusedElement}");
			});*/

			RegisterCallback<KeyDownEvent>(OnKeyDown);
		}

		private void OnTextEdit(ChangeEvent<string> evt)
		{
			editField.SetValueWithoutNotify(evt.newValue.Replace("/", "-"));
		}

		private void OnEditFieldAttach(AttachToPanelEvent evt)
		{
			editField.Focus();
			//editField.SelectAll();
		}

		private void OnKeyDown(KeyDownEvent evt)
		{
			Debug.Log(evt.keyCode);
		}


		private void SetNormal()
		{
			if (!isEditing)
			{
				return;
			}
			Blur();
			//Debug.Log("SetNormal");
	
			UnregisterCallback<KeyUpEvent>(OnKeyUp, TrickleDown.TrickleDown);
			//editField.UnregisterCallback<FocusOutEvent>(OnFocusLost);
			
			//Debug.Log("MouseDown Registered");
			RegisterCallback<MouseDownEvent>(OnMouseDown);
			
			
			delegatesFocus = false;

			isEditing = false;

			displayLabel.text = editField.value;
			editField.value = string.Empty;
			

			Remove(editField);
			Add(displayLabel);
			
			OnTextCallback?.Invoke(displayLabel.text);
			
	
		}

	
		private void OnMouseDown(MouseDownEvent evt)
		{
			//Debug.Log($"MouseDown:{evt.clickCount} IsEditing:{isEditing}");
			if (evt.clickCount == 2 && !isEditing)
			{
				SetEditing();
				evt.StopImmediatePropagation();
				evt.PreventDefault();	
			}
		}


		public void SetEditing()
		{
			if (isEditing)
			{
				return;
			}
			//Debug.Log("SetEditing");

			Debug.Log("MouseDown Unregistered");
			UnregisterCallback<MouseDownEvent>(OnMouseDown);

			Remove(displayLabel);
			Add(editField);
			
			focusable = delegatesFocus = true;
			
			editField.value = displayLabel.text;
			isEditing = true;
			displayLabel.text = string.Empty;

			RegisterCallback<KeyUpEvent>(OnKeyUp, TrickleDown.TrickleDown);
		}

		private void OnKeyUp(KeyUpEvent evt)
		{
			//Debug.Log($"{name}> {evt}");
			if (isEditing && evt.keyCode is KeyCode.Return or KeyCode.KeypadEnter)
			{
				evt.StopImmediatePropagation();
				evt.PreventDefault();
				SetNormal();
			}
		}
		
		/*private void OnFocusLost(FocusOutEvent evt)
		{
			if (isEditing)
			{
				evt.StopImmediatePropagation();
				evt.PreventDefault();
				SetNormal();
			}
		}*/
	}
}