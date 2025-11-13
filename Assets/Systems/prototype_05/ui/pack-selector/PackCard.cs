using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Prototype_05.UI
{
    public class PackCard : VisualElement
    {
        public event Action<Guid> OnClick;
        private readonly Label title = new();
        private readonly Label icon = new();
        private readonly Label untilNextUnlock = new();
        private Guid id = Guid.NewGuid();
        public PackCard()
        {
            style.width = 200;
            style.height = 400;

            Button container = new(() => OnClick?.Invoke(id));
            Add(container);
            container.style.backgroundColor = Colors.PRIMARY;
            container.style.paddingTop = 24;
            container.style.paddingRight = 24;
            container.style.paddingBottom = 24;
            container.style.paddingLeft = 24;
            container.style.alignItems = Align.Center;
            container.style.flexGrow = 1;


            container.Add(title);
            title.text = "---";
            title.style.color = Colors.TEXT;
            title.style.unityFontStyleAndWeight = FontStyle.Bold;
            title.style.fontSize = 24;

            container.Add(icon);
            icon.text = "###";
            icon.style.color = Colors.TEXT;
            icon.style.unityFontStyleAndWeight = FontStyle.Bold;
            icon.style.flexGrow = 1;
            icon.style.fontSize = 64;

            container.Add(untilNextUnlock);
            untilNextUnlock.text = "## / ## ----";
            untilNextUnlock.style.color = Colors.TEXT;
        }

        public void Update(PackCardData data)
        {
            icon.text = data.Icon;
            title.text = data.Title;
            untilNextUnlock.text = data.UntilNextUnlock;
            id = data.Id;
        }
    }
    public struct PackCardData
    {
        public string Title;
        public string Icon;
        public string UntilNextUnlock;
        public Guid Id;
    }
}