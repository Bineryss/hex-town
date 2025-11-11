using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Core;
using Systems.Prototype_04;
using Systems.Prototype_05.Building;
using Systems.Prototype_05.Score;
using Systems.Prototype_05.Transport;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Prototype_05.UI
{
    public class UIController : SerializedMonoBehaviour
    {
        [SerializeField] private UIDocument document;
        [SerializeField] private PreviewOrchestrator previewManager;
        [SerializeField] private InventoryController inventoryController;
        [SerializeField] private TransportController transportController;
        [SerializeField] private List<ProductionPack> packs;
        [SerializeField] private ProductionPack selection; //only for testing

        private VisualElement container;
        private ScoreIndicator scoreIndicator;
        private InventoryContainer inventory;
        private ScoreDatasource scoreDatasource = ScoreDatasource.Instance; //TODO add service locator
        private InventoryDS buildingInventory = InventoryDS.Instance; //TODO add service locator

        [OdinSerialize] private Dictionary<Guid, WorldTile> idToTile = new();
        private readonly Guid packButtonId = Guid.NewGuid();
        private void BuildUI(VisualElement root)
        {
            VisualElement footer = new();
            footer.style.alignSelf = Align.FlexEnd;
            footer.style.paddingBottom = 32;
            footer.style.paddingRight = 32;
            footer.style.paddingLeft = 32;
            footer.style.flexDirection = FlexDirection.Row;
            footer.style.justifyContent = Justify.SpaceBetween;
            footer.style.flexGrow = 1;
            container = new();
            container.Add(footer);
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.FlexEnd;
            container.style.flexGrow = 1;
            container.style.alignContent = Align.Stretch;
            root.Add(container);
            scoreIndicator = new ScoreIndicator(new ScoreIndicatorDO()
            {
                overallScore = scoreDatasource.TotalScore,
                currentScore = scoreDatasource.Progress,
                pointsUntilNext = scoreDatasource.PackUnlockThreshold
            });
            footer.Add(scoreIndicator);
            inventory = new InventoryContainer(new List<InventoryElementDO>());
            inventory.ItemClicked += (id) =>
            {
                if (id == packButtonId)
                {
                    HandlePackOpen();
                    return;
                }
                EventBus<InventoryElementSelected>.Raise(new InventoryElementSelected()
                {
                    tile = idToTile[id]
                });
            };
            UpdateInventoryUI();
            footer.Add(inventory);
            footer.Add(new VisualElement());
        }

        private void HandlePackOpen()
        {
            EventBus<PackOpened>.Raise(new PackOpened()
            {
                PackId = selection.Id
            });
        }

        private void Setup()
        {
            if (document == null) return;
            VisualElement root = document.rootVisualElement;
            root.Clear();
            inventoryController.Initialize();
            transportController.Initialize();
            previewManager.Initialize();
            root.Add(previewManager.Root);
            EventBus<ScoreChanged>.Event += (data) => scoreIndicator.Update(new ScoreIndicatorDO()
            {
                currentScore = scoreDatasource.Progress,
                overallScore = scoreDatasource.TotalScore,
                pointsUntilNext = scoreDatasource.PackUnlockThreshold
            });
            BuildUI(root);
        }

        void OnEnable()
        {
            Setup();
            EventBus<BuildingInventoryChanged>.Event += UpdateInventoryUI;
        }

        private void UpdateInventoryUI(BuildingInventoryChanged data = default)
        {
            idToTile.Clear();
            List<InventoryElementDO> items = buildingInventory.BuildingInventory.Select(el =>
                {
                    Guid id = Guid.NewGuid();
                    idToTile[id] = el.Key;
                    return new InventoryElementDO()
                    {
                        id = id,
                        icon = el.Key.name,
                        quantity = el.Value
                    };
                }).ToList();
            if (buildingInventory.PacksLeft > 0)
            {
                items.Add(new InventoryElementDO()
                {
                    id = packButtonId,
                    icon = "Pack",
                    quantity = buildingInventory.PacksLeft
                });
            }
            inventory.Update(items);
        }
    }
    public struct InventoryElementSelected : IEvent
    {
        public WorldTile tile;
    }
}