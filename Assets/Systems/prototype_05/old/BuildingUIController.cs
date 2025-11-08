using System;
using System.Collections.Generic;
using System.Linq;
using Systems.Prototype_04;
using Systems.Prototype_04.Grid;
using Systems.Prototype_05.Building;
using Systems.Prototype_05.Player;
using Systems.Prototype_05.Transport;
using UnityEngine;

namespace Systems.Prototype_05.UI
{
    public struct InventoryElementSelected : IEvent
    {
        public WorldTile tile;
    }
    public struct BuildingPlaced : IEvent
    {
        public WorldTile type;
    }

    public class BuildingUIController : MonoBehaviour
    {
        [SerializeField] private WorldNode previewNode;
        [SerializeField] private TransportController transportController;
        [SerializeField] private HexGridGenerator generator;

        private WorldTile selectedBuilding;
        [SerializeField] private List<WorldNode> placedBuildings = new();
        public List<WorldNode> PlacedBuildings => placedBuildings;
        private WorldNode current;
        private WorldNode prevNode;

        private BuildingInventory buildingInventory = BuildingInventory.Instance;

        public void Initialize()
        {
            EventBus<InventoryElementSelected>.Event += HandleBuildingSelection;
            EventBus<TileSelectionChanged>.Event += HandleMouseInteraction;
        }

        private void HandleBuildingSelection(InventoryElementSelected data)
        {
            if (data.tile == null) return;
            selectedBuilding = data.tile;
            previewNode.Initialize(selectedBuilding, new HexCoordinate(0, 0));
            previewNode.gameObject.SetActive(false);
            Debug.Log($"Selected {data.tile.name}");
        }

        public void HandleMouseInteraction(TileSelectionChanged data)
        {
            WorldNode node = data.node;
            bool isClick = data.clicked;

            if (selectedBuilding == null) return;
            if (!node.worldTile.isBuildable)
            {
                prevNode?.gameObject.SetActive(true);
                previewNode.gameObject.SetActive(false);
                List<WorldNode> prevPossibleSubTiles = GetListOfPossibleSubTiles(prevNode);
                foreach (var subTile in prevPossibleSubTiles)
                {
                    subTile.Deselect();
                }
                prevNode = node;
                return;
            }

            if (prevNode != null && !prevNode.Position.Equals(node.Position))
            {
                prevNode.gameObject.SetActive(true);
                List<WorldNode> prevPossibleSubTiles = GetListOfPossibleSubTiles(prevNode);
                foreach (var subTile in prevPossibleSubTiles)
                {
                    subTile.Deselect();
                }
            }

            previewNode.gameObject.SetActive(true);
            node.gameObject.SetActive(false);
            current = node;
            List<WorldNode> possibleSubTiles = GetListOfPossibleSubTiles(node);
            foreach (var subTile in possibleSubTiles)
            {
                subTile.Select();
            }
            previewNode.gameObject.transform.position = node.transform.position;

            if (!isClick)
            {
                prevNode = node;
                return;
            }
            transportController.Manager.RemoveAllRoutesForTile(node);
            node.name = $"{selectedBuilding.resourceType}-{node.Position}";
            node.InitializeWithSubTiles(selectedBuilding, node.Position, possibleSubTiles);
            node.ConfirmPlacement();
            foreach (var subTile in possibleSubTiles)
            {
                subTile.Deselect();
            }
            EventBus<BuildingPlaced>.Raise(new BuildingPlaced()
            {
                type = selectedBuilding
            });
            if (!buildingInventory.buildingInventory.ContainsKey(selectedBuilding))
            {
                previewNode.gameObject.SetActive(false);
                node.gameObject.SetActive(true);
                current = null;
                selectedBuilding = null;
            }
            placedBuildings.Add(node);
            prevNode = node;
        }

        private List<WorldNode> GetListOfPossibleSubTiles(WorldNode node)
        {
            List<WorldNode> possibleSubTiles = new();
            foreach (WorldNode neighbor in node.Neighbors(generator.nodes).Cast<WorldNode>())
            {
                if (selectedBuilding.connectableTiles.Contains(neighbor.worldTile) && !neighbor.isSubTile)
                {
                    possibleSubTiles.Add(neighbor);
                }
            }
            return possibleSubTiles;
        }

        public void Exit()
        {
            previewNode.gameObject.SetActive(false);
            current?.gameObject.SetActive(true);
            current = null;
        }
    }
}