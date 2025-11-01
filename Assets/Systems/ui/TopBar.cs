using System.Collections.Generic;
using Systems.Building;
using UnityEngine;
using UnityEngine.UIElements;

public class TopBar
{
    public VisualElement Root { get; }
    private readonly Dictionary<ResourceType, Label> valueLabels;

    public TopBar(Dictionary<ResourceType, int> initialValues = default)
    {
        Root = new VisualElement();
        Root.style.height = 60;
        Root.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        Root.style.flexDirection = FlexDirection.Row;
        Root.style.justifyContent = Justify.SpaceAround;
        Root.style.alignItems = Align.Center;
        
        valueLabels = new Dictionary<ResourceType, Label>();
        
        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            var (container, valueLabel) = CreateResourceDisplay(type);
            Root.Add(container);
            valueLabels[type] = valueLabel;
        }
    }

    private (VisualElement container, Label valueLabel) CreateResourceDisplay(ResourceType type)
    {
        var container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.alignItems = Align.Center;
        
        var nameLabel = new Label($"{type}:");
        nameLabel.style.color = Color.white;
        nameLabel.style.marginRight = 5;
        
        var valueLabel = new Label("0");
        valueLabel.style.color = Color.yellow;
        valueLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
        
        container.Add(nameLabel);
        container.Add(valueLabel);
        
        return (container, valueLabel);
    }

    public void UpdateValue(ResourceType type, int amount)
    {
        if (valueLabels.TryGetValue(type, out var label))
        {
            label.text = amount.ToString();
        }
    }
}