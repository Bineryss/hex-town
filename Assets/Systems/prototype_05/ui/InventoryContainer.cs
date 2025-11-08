using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Systems.Prototype_05.UI
{
    public class InventoryContainer : VisualElement
    {

        public InventoryContainer(List<InventoryElementDO> data)
        {
            style.flexDirection = FlexDirection.Row;
            style.backgroundColor = Colors.PRIMARY;
            Update(data);
        }

        public void Update(List<InventoryElementDO> data)
        {
            Clear();
            data.ForEach(element =>
            {
                Add(new InventoryElement(element));
            });
        }
    }
}