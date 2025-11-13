using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Systems.Prototype_05.UI
{
    public class PackSelectorScreen : VisualElement
    {
        public event Action<Guid> OnPackSelection;

        private List<PackCard> cards = new();

        public PackSelectorScreen()
        {
            style.flexDirection = FlexDirection.Row;
            style.alignItems = Align.Center;
            style.justifyContent = Justify.Center;
        }

        public void Update(List<PackCardData> cards)
        {
            Clear();
            foreach (PackCardData data in cards)
            {
                PackCard card = new();
                card.Update(data);
                card.OnClick += OnPackSelection;
                this.cards.Add(card);
                Add(card);
            }
        }
    }
}