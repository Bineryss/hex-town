using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UIElements;

namespace Systems.Prototype_05.UI
{
    public class InventoryContainer : VisualElement
    {
        public event Action<Guid> ItemClicked;

        private readonly List<InventoryElement> elements = new();

        public InventoryContainer(List<InventoryElementDO> data)
        {
            style.flexDirection = FlexDirection.Row;
            style.backgroundColor = Colors.PRIMARY;

            if (data != null)
            {
                Update(data);
            }
        }

        public void Update(List<InventoryElementDO> data)
        {
            if (data is null) return;
            elements.ForEach(el =>
            {
                el.onClick -= HandleClick;
            });
            elements.Clear();

            Clear();
            data.ForEach(element =>
            {
                InventoryElement iElement = new(element);
                iElement.onClick += HandleClick;
                elements.Add(iElement);
                Add(iElement);
            });
        }

        private void HandleClick(Guid id)
        {
            ItemClicked?.Invoke(id);
        }
    }
}