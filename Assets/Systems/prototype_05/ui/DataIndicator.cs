using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Prototype_05.UI
{
    public class DataIndicator : VisualElement
    {
        private VisualElement container = new();
        private VisualElement pointer = new();
        private Label pointsLabel = new();
        private Label icon = new();
        private int offset;

        public DataIndicator(int offset = 10, DataIndicatorDO? data = null)
        {
            this.offset = offset;

            int width = 64;
            Color bgColor = Colors.PRIMARY;

            style.position = Position.Absolute;
            style.left = 0;
            style.bottom = 100;
            style.width = width;
            style.minWidth = width;

            container.style.backgroundColor = bgColor;
            container.style.fontSize = 24;
            container.style.color = Colors.TEXT;
            container.style.alignItems = Align.Center;
            container.style.alignSelf = Align.Center;
            container.style.paddingTop = 8;
            container.style.paddingRight = 8;
            container.style.paddingBottom = 8;
            container.style.paddingLeft = 8;

            container.Add(icon);
            container.Add(pointsLabel);
            Add(container);

            pointer.style.borderBottomWidth = width / 2;
            pointer.style.borderLeftWidth = width / 2;
            pointer.style.borderRightWidth = width / 2;
            pointer.style.borderTopWidth = width / 2;
            pointer.style.borderBottomColor = Colors.TRANSPARENT;
            pointer.style.borderLeftColor = Colors.TRANSPARENT;
            pointer.style.borderRightColor = Colors.TRANSPARENT;
            pointer.style.borderTopColor = bgColor;
            Add(pointer);

            Update(data);
        }

        public void Place(Vector2 screenPosition)
        {
            Vector2 pos = new(screenPosition.x - resolvedStyle.width / 2, screenPosition.y);

            pos.x = Mathf.Clamp(pos.x, 0, Screen.width - resolvedStyle.width);
            pos.y = Mathf.Clamp(pos.y, 0, Screen.height);

            style.left = pos.x;
            style.bottom = pos.y + offset;
        }

        public void Update(DataIndicatorDO? data)
        {
            if (data.Equals(null))
            {
                pointsLabel.text = "+00";
                icon.text = "###";
                return;
            }

            char indicator = data?.points >= 0 ? '+' : '-';
            pointsLabel.text = $"{indicator}{data?.points}";
            icon.text = data?.icon.ToString();
        }

        public void Hide()
        {
            style.visibility = Visibility.Hidden;
        }
        public void Show()
        {
            style.visibility = Visibility.Visible;
        }
    }

    [System.Serializable]
    public struct DataIndicatorDO
    {
        public int points;
        public string icon; //TODO change to sprite
    }
}