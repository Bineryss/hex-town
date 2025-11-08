using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
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
        [SerializeField] private Camera mainCamera;
        [SerializeField] private Transform target;

        [SerializeField] private BuildingUIController buildingUIController;
        [SerializeField] private TransportController transportController;

        private VisualElement container;
        private DataIndicator dataIndicator;
        private ScoreIndicator scoreIndicator;
        private InventoryContainer inventory;
        private ScoreDatasource scoreDatasource = ScoreDatasource.Instance; //TODO add service locator
        private BuildingInventory buildingInventory = BuildingInventory.Instance; //TODO add service locator

        [OdinSerialize] private Dictionary<Guid, WorldTile> idToTile = new();

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
            dataIndicator = new DataIndicator(new DataIndicatorDO()
            {
                points = 10,
                icon = "W"
            });
            root.Add(dataIndicator);
            root.Add(container);
            scoreIndicator = new ScoreIndicator(new ScoreIndicatorDO()
            {
                overallScore = 564,
                currentScore = 4,
                pointsUntilNext = 90
            });
            footer.Add(scoreIndicator);
            inventory = new InventoryContainer(new List<InventoryElementDO>());
            inventory.ItemClicked += (id) =>
            {
                EventBus<InventoryElementSelected>.Raise(new InventoryElementSelected()
                {
                    tile = idToTile[id]
                });
            };

            footer.Add(inventory);
            footer.Add(new VisualElement());
        }

        void Update()
        {
            scoreIndicator.Update(new ScoreIndicatorDO()
            {
                currentScore = scoreDatasource.CurrentScore,
                overallScore = scoreDatasource.OverallScore,
                pointsUntilNext = scoreDatasource.ScoreToNextDeck
            });
            dataIndicator.Place(WorldToScreen(target.position));
        }

        private Vector2 WorldToScreen(Vector3 vector)
        {
            if (mainCamera == null) return new(0, 0);

            Vector3 screenPos = mainCamera.WorldToScreenPoint(vector);
            return new(screenPos.x, screenPos.y);
        }

        private void Setup()
        {
            if (document == null) return;
            VisualElement root = document.rootVisualElement;
            root.Clear();
            BuildUI(root);
            transportController.Initialize();
            buildingUIController.Initialize();
        }

        void OnEnable()
        {
            Setup();
            EventBus<BuildingInventoryChanged>.Event += UpdateInventoryUI;
        }

        private void UpdateInventoryUI(BuildingInventoryChanged data)
        {
            idToTile.Clear();
            inventory.Update(
                buildingInventory.buildingInventory.Select(el =>
                {
                    Guid id = Guid.NewGuid();
                    idToTile[id] = el.Key;
                    return new InventoryElementDO()
                    {
                        id = id,
                        icon = el.Key.name,
                        quantity = el.Value
                    };
                }).ToList()
            );
        }
    }
}