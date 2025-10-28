using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.UI
{
    public class ResourceOverview : VisualElement
    {
        private readonly List<ResourceInfo> resources = new();
        private readonly List<Label> resourceInfoLabels = new();


        public ResourceOverview()
        {
            CreatePanel();
        }

        private void CreatePanel()
        {
            style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
            style.paddingTop = 10;
            style.paddingBottom = 10;
            style.paddingLeft = 15;
            style.paddingRight = 15;
            style.flexDirection = FlexDirection.Row;

            RefreshDisplay();
        }

        public void UpdateResources(List<ResourceInfo> newResources)
        {
            resources.Clear();
            resources.AddRange(newResources);
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            foreach (var label in resourceInfoLabels)
            {
                Remove(label);
            }
            resourceInfoLabels.Clear();

            foreach (var resource in resources)
            {
                Label label = new() { text = $"{resource.ResourceType}: {resource.Quantity} | ", style = { fontSize = 14, unityFontStyleAndWeight = FontStyle.Bold, color = Color.white, marginLeft = 5, marginRight = 5 } };
                resourceInfoLabels.Add(label);
                Add(label);
            }
        }
    }

    public struct ResourceInfo
    {
        public string ResourceType;
        public int Quantity;
    }
}