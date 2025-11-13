using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Systems.Core;
using Systems.Prototype_05.Building;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Prototype_05.UI
{
    public class PackController : MonoBehaviour
    {
        [SerializeField] private List<ProductionPack> packs;


        public VisualElement Root => root;
        private VisualElement root;
        private PackSelectorScreen screen;

        public void Initialize()
        {
            root = new()
            {
                pickingMode = PickingMode.Ignore
            };
            root.style.flexGrow = 1;

            screen = new();
            screen.style.visibility = Visibility.Hidden;
            screen.OnPackSelection += HandlePackSelection;
            root.Add(screen);

            EventBus<PackSelected>.Event += HandlePackOpen;
        }

        private void HandlePackSelection(Guid id)
        {
            EventBus<PackOpened>.Raise(new PackOpened()
            {
                PackId = id
            });
            screen.style.visibility = Visibility.Hidden;
        }

        private void HandlePackOpen(PackSelected selected)
        {
            List<PackCardData> data = PickUpToThree();
            screen.Update(data);
            screen.style.visibility = Visibility.Visible;
        }

        private List<PackCardData> PickUpToThree()
        {
            int count = Mathf.Max(3, packs.Count);
            return packs.OrderBy(_ => Guid.NewGuid()).Take(count).Select(el =>
            {
                return new PackCardData()
                {
                    Title = el.PackName,
                    Icon = el.Icon,
                    UntilNextUnlock = "10/100 Wheat",
                    Id = el.Id
                };
            }).ToList();
        }
    }


}