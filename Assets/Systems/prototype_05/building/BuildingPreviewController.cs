using System;
using System.Collections.Generic;
using Systems.Core;
using Systems.Prototype_04;
using Systems.Prototype_05.Player;
using Systems.Prototype_05.UI;
using UnityEngine;

namespace Systems.Prototype_05.Building
{
    public class BuildingPreviewController : MonoBehaviour, IPreviewController
    {
        [SerializeField] private WorldNode previewNode;
        [SerializeField] private HexGridGenerator generator;
        [SerializeField] private BuildingUIController buildingUIController;
        public Guid Mode => mode;

        public bool Active //TODO extract into exit and enter methods
        {
            get => active; set
            {
                active = value;
                if (!value)
                {

                    previewNode.gameObject.SetActive(false);
                    if (prevNode != null)
                    {
                        prevNode.gameObject.SetActive(true);
                    }
                    EventBus<ScorePreviewRequested>.Raise(new ScorePreviewRequested());
                }
            }
        }
        private bool active;
        private Guid mode;

        private WorldTile selectedBuilding;
        private WorldNode prevNode;
        private readonly InventoryDS buildingInventory = InventoryDS.Instance;

        public void Initialize(Guid mode)
        {
            this.mode = mode;
            previewNode.gameObject.SetActive(false);

            EventBus<InventoryElementSelected>.Event += HandleBuildingSelection;
            EventBus<TileSelectionChanged>.Event += HandleMouseInteraction;
        }

        private void HandleMouseInteraction(TileSelectionChanged data)
        {
            if (!active) return;
            WorldNode node = data.node;
            bool clicked = data.clicked;

            if (node == null)
            {
                Debug.Log("node null");
                EventBus<ScorePreviewRequested>.Raise(new ScorePreviewRequested());
                prevNode?.gameObject.SetActive(true);
                previewNode.gameObject.SetActive(false);
                return;
            }

            if (selectedBuilding == null) return;
            if (!buildingInventory.BuildingInventory.ContainsKey(selectedBuilding))
            {
                Debug.Log($"no building of type: {selectedBuilding.name} in inventory");
                selectedBuilding = null;
                return;
            }
            if (!node.worldTile.isBuildable)
            {
                EventBus<ScorePreviewRequested>.Raise(new ScorePreviewRequested()
                {
                    Tooltips = new List<ScorePreview>()
                    {
                        new()
                        {
                            Position = node.Position,
                            Icon = "XX",
                            Score = 0
                        }
                    }
                });
                prevNode?.gameObject.SetActive(true);
                previewNode.gameObject.SetActive(false);
                List<WorldNode> prevPossibleSubTiles = buildingUIController.GetListOfPossibleSubTiles(prevNode, selectedBuilding);
                foreach (var subTile in prevPossibleSubTiles)
                {
                    subTile.Deselect();
                }
                prevNode = node;
                return;
            }
            previewNode.Position = node.Position;
            EventBus<ScorePreviewRequested>.Raise(new ScorePreviewRequested()
            {
                Tooltips = new List<ScorePreview>()
                    {
                        new()
                        {
                            Position = node.Position,
                            Icon = previewNode.ResourceType.ToString(),
                            Score = selectedBuilding.resourceAmount
                        }
                    }
            });
            if (prevNode != null && !prevNode.Position.Equals(node.Position))
            {
                prevNode.gameObject.SetActive(true);
                List<WorldNode> prevPossibleSubTiles = buildingUIController.GetListOfPossibleSubTiles(prevNode, selectedBuilding);
                foreach (var subTile in prevPossibleSubTiles)
                {
                    subTile.Deselect();
                }
            }

            previewNode.gameObject.SetActive(true);
            node.gameObject.SetActive(false);
            List<WorldNode> possibleSubTiles = buildingUIController.GetListOfPossibleSubTiles(node, selectedBuilding);
            foreach (var subTile in possibleSubTiles)
            {
                subTile.Select();
            }
            previewNode.gameObject.transform.position = node.transform.position;

            if (!clicked)
            {
                prevNode = node;
                return;
            }
            if (buildingUIController.TryPlaceBuilding(node.Position, selectedBuilding))
            {
                node.gameObject.SetActive(true);
            }
            previewNode.gameObject.SetActive(false);
            prevNode = node;
            if (!buildingInventory.BuildingInventory.ContainsKey(selectedBuilding))
            {
                Debug.Log($"no building of type: {selectedBuilding.name} in inventory left");
                selectedBuilding = null;
                EventBus<PreviewDeactivationRequested>.Raise(new PreviewDeactivationRequested()
                {
                    Mode = mode
                });
                return;
            }
            EventBus<ScorePreviewRequested>.Raise(new ScorePreviewRequested());
        }
        private void HandleBuildingSelection(InventoryElementSelected data)
        {
            if (data.tile == null) return;
            if (data.tile.Equals(selectedBuilding))
            {
                EventBus<PreviewDeactivationRequested>.Raise(new PreviewDeactivationRequested()
                {
                    Mode = mode,
                });
                selectedBuilding = null;
                return;
            }

            selectedBuilding = data.tile;
            previewNode.Initialize(selectedBuilding, new AxialCoordinate(0, 0));
            previewNode.gameObject.SetActive(false);
            Debug.Log($"Selected {data.tile.name}");
            EventBus<PreviewActivationRequested>.Raise(new PreviewActivationRequested()
            {
                Mode = mode,
                Force = true,
            });
        }

    }
}