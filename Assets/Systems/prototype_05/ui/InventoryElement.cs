using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Prototype_05.UI
{
    public class InventoryElement : VisualElement
    {
        public Action onClick;

        private Label icon = new();
        private Label quantity = new();

        public InventoryElement(InventoryElementDO data)
        {
            style.height = 150;
            style.width = 150;
            style.color = Colors.TEXT;

            Button button = new(onClick);
            Add(button);
            button.Add(icon);
            button.Add(quantity);
            quantity.style.unityFontStyleAndWeight = FontStyle.Bold;

            Update(data);
        }

        public void Update(InventoryElementDO data)
        {
            icon.text = data.icon;
            quantity.text = data.quantity.ToString();
        }
    }

    public struct InventoryElementDO
    {
        public string icon;
        public int quantity;
    }
}