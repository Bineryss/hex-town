using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Prototype_05.UI
{
    public class DataIndicator : VisualElement
    {
        public DataIndicator()
        {
            style.width = 300;
            style.height = 300;
            style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1);
        }
    }
}