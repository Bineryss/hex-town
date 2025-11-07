using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Prototype_04.UI
{
    public class BuildingDrawPanel : VisualElement, IUIModeSegment
    {
        public Action<string> OnBuildingPackSelected;
        private readonly VisualElement container;

        private List<Button> deckButtons = new();

        public BuildingDrawPanel()
        {
            container = new VisualElement();
            container.style.paddingTop = 10;
            container.style.paddingBottom = 10;
            container.style.paddingLeft = 15;
            container.style.paddingRight = 15;
            container.style.flexDirection = FlexDirection.Column;
            container.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1);
            Add(container);
        }

        public void EnterMode()
        {
            style.display = DisplayStyle.Flex;
        }

        public void ExitMode()
        {
            style.display = DisplayStyle.None;
        }

        public void SetCurrentDeckOptions(List<BuildingDrawOption> options)
        {
            foreach (BuildingDrawOption option in options)
            {
                var button = new Button(() => { OnBuildingPackSelected?.Invoke(option.Id); })
                {
                    text = option.Label
                };
                deckButtons.Add(button);
                container.Add(button);
            }
        }
    }

    public struct BuildingDrawOption
    {
        public string Id;
        public string Label;
    }
}