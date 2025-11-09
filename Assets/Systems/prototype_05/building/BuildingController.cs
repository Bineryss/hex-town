using System.Collections.Generic;
using System.Linq;
using Systems.Prototype_04;
using Systems.Prototype_04.Grid;
using Systems.Prototype_05.Building;
using Systems.Prototype_05.Transport;
using UnityEngine;

namespace Systems.Prototype_05.UI
{
    public class BuildingUIController : MonoBehaviour
    {
        [SerializeField] private TransportController transportController;
        [SerializeField] private HexGridGenerator generator;

        [SerializeField] private List<WorldNode> placedBuildings = new();
        public List<WorldNode> PlacedBuildings => placedBuildings;

        private InventoryDS buildingInventory = InventoryDS.Instance;

        public bool TryPlaceBuilding(HexCoordinate position, WorldTile type)
        {
            if (!buildingInventory.BuildingInventory.ContainsKey(type))
            {
                Debug.Log($"no building of type: {type.name} in inventory");
                return false;
            }
            if (!generator.nodes.TryGetValue(position, out INode node) || node is not WorldNode)
            {
                Debug.Log($"couldn't find node for position {position}");
                return false;
            }
            WorldNode worldNode = node as WorldNode;
            if (worldNode == null)
            {
                Debug.Log($"node at position {position} is not a WorldNode!");
                return false;
            }

            List<WorldNode> possibleSubTiles = GetListOfPossibleSubTiles(worldNode, type);
            foreach (var subTile in possibleSubTiles)
            {
                subTile.Select();
            }

            transportController.Manager.RemoveAllRoutesForTile(worldNode);
            worldNode.name = $"{type.resourceType}-{worldNode.Position}";
            worldNode.InitializeWithSubTiles(type, worldNode.Position, possibleSubTiles);
            worldNode.ConfirmPlacement();
            foreach (var subTile in possibleSubTiles)
            {
                subTile.Deselect();
            }
            EventBus<BuildingPlaced>.Raise(new BuildingPlaced()
            {
                type = type
            });
            if (!buildingInventory.BuildingInventory.ContainsKey(type))
            {
                worldNode.gameObject.SetActive(true);
            }
            placedBuildings.Add(worldNode);
            return true;
        }

        public List<WorldNode> GetListOfPossibleSubTiles(WorldNode node, WorldTile type)
        {
            List<WorldNode> possibleSubTiles = new();
            foreach (WorldNode neighbor in node.Neighbors(generator.nodes).Cast<WorldNode>())
            {
                if (type.connectableTiles.Contains(neighbor.worldTile) && !neighbor.isSubTile)
                {
                    possibleSubTiles.Add(neighbor);
                }
            }
            return possibleSubTiles;
        }
    }

    public struct BuildingPlaced : IEvent
    {
        public WorldTile type;
    }
}