using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.UI
{
    [Serializable]
    public class ModeElement
    {
        [field: SerializeField] public string Name { get; set; }
        [field: SerializeField] public UIState State { get; set; }

        public ModeElement(string name, UIState state)
        {
            Name = name;
            State = state;
        }
    }

    public class ModeSelectionPanel : VisualElement
    {
        public event Action<UIState> OnModeSelected;

        private VisualElement buttonContainer;
        private List<ModeElement> modeButtons = new();

        public ModeSelectionPanel() : this(new List<ModeElement>())
        {
        }

        public ModeSelectionPanel(List<ModeElement> modes)
        {
            modeButtons = modes;
            CreatePanel();
        }

        private void CreatePanel()
        {
            style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
            style.paddingTop = 10;
            style.paddingBottom = 10;
            style.paddingLeft = 15;
            style.paddingRight = 15;

            buttonContainer = new VisualElement();
            buttonContainer.style.flexDirection = FlexDirection.Row;
            buttonContainer.style.flexGrow = 1;
            buttonContainer.style.justifyContent = Justify.SpaceBetween;

            CreateButtons();

            Add(buttonContainer);
        }

        private void CreateButtons()
        {
            buttonContainer.Clear();

            foreach (var modeButton in modeButtons)
            {
                var button = CreateModeButton(modeButton);
                buttonContainer.Add(button);
            }
        }

        private Button CreateModeButton(ModeElement modeButton)
        {
            var button = new Button();
            button.text = modeButton.Name;
            button.style.fontSize = 14;
            button.style.height = 35;
            button.style.flexGrow = 1;
            button.style.marginLeft = 2;
            button.style.marginRight = 2;
            button.style.color = Color.white;
            button.style.backgroundColor = new Color(0.25f, 0.25f, 0.25f, 1f);
            button.style.borderTopWidth = 1;
            button.style.borderBottomWidth = 1;
            button.style.borderLeftWidth = 1;
            button.style.borderRightWidth = 1;
            button.style.borderTopColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            button.style.borderBottomColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            button.style.borderLeftColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            button.style.borderRightColor = new Color(0.4f, 0.4f, 0.4f, 1f);

            button.RegisterCallback<ClickEvent>(evt => OnButtonClicked(modeButton.State));

            return button;
        }

        private void OnButtonClicked(UIState state)
        {
            OnModeSelected?.Invoke(state);
        }

        public void UpdateModes(List<ModeElement> newModes)
        {
            modeButtons = newModes;
            CreateButtons();
        }
    }
}