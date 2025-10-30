using System.Collections.Generic;
using System.Linq;
using Systems.Grid;
using Systems.Transport;
using UnityEngine;

namespace Systems.UI
{
    public class BuildingUIController : MonoBehaviour, IUIController
    {
        public IUIModeSegment UIModeSegment => buildingSelectionPanel;
        [SerializeField] private List<WorldTile> buildings;
        [SerializeField] private WorldNode previewNode;
        [SerializeField] private TransportController transportController;
        [SerializeField] private HexGridGenerator generator;

        private WorldTile selectedBuilding;
        private BuildingSelectionPanel buildingSelectionPanel;
        private readonly List<WorldNode> placedBuildings = new();
        public List<WorldNode> PlacedBuildings => placedBuildings;
        private WorldNode current;

        public void Initialize()
        {
            buildingSelectionPanel = new(buildings);
            buildingSelectionPanel.OnBuildingSelected += HandleBuildingSelection;
        }

        private void HandleBuildingSelection(WorldTile building)
        {
            if (building == null) return;
            selectedBuilding = building;
            previewNode.Initialize(selectedBuilding, new HexCoordinate(0, 0));
            previewNode.gameObject.SetActive(false);
        }

        public void HandleMouseInteraction(WorldNode node, WorldNode prevNode, bool isClick)
        {
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

            if (!isClick) return;
            transportController.Manager.RemoveAllRoutesForTile(node);
            node.name = $"{selectedBuilding.resourceType}-{node.Position}";
            node.InitializeWithSubTiles(selectedBuilding, node.Position, possibleSubTiles);
            foreach (var subTile in possibleSubTiles)
            {
                subTile.Deselect();
            }

            placedBuildings.Add(node);
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
            buildingSelectionPanel.ExitMode();
        }

        public void Activate()
        {
            buildingSelectionPanel.EnterMode();
        }
    }
}