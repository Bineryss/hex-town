using System.Collections.Generic;
using Systems.Prototype_04;
using Systems.Prototype_04.Grid;
using Systems.Prototype_05.Player;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Systems.Prototype_05.Building;
using Systems.Prototype_05.Transport;
using Unity.VisualScripting;

namespace Systems.Prototype_05.UI
{
    public class PreviewManager : MonoBehaviour
    {
        [SerializeField] private WorldNode previewNode;
        [SerializeField] private HexGridGenerator generator;
        [SerializeField] private BuildingUIController buildingUIController;
        [SerializeField] private Camera mainCamera;

        public VisualElement Root => root;
        private VisualElement root;
        private DataIndicator directIndicator;

        private readonly List<DataIndicator> indicators = new();
        private WorldTile selectedBuilding;
        private WorldNode prevNode;
        private InventoryDS buildingInventory = InventoryDS.Instance;
        [SerializeField] private SmoothLineRenderer routePreview;
        [SerializeField] private PathfindingController pathfindingController;
        [SerializeField] private float offsetHeight = 0.6f;
        [SerializeField] private TransportController transportController;

        [SerializeField, ReadOnly] private Mode mode = Mode.NONE;
        private HexCoordinate currentMouseCoord;

        private WorldNode origin;
        private WorldNode destination;

        public void Initialize()
        {
            root = new();
            root.style.position = Position.Absolute;
            root.style.top = 0;
            root.style.right = 0;
            root.style.left = 0;
            root.style.bottom = 0;
            directIndicator = new();
            directIndicator.Hide();
            root.Add(directIndicator);
            previewNode.gameObject.SetActive(false);

            EventBus<InventoryElementSelected>.Event += HandleBuildingSelection;
            EventBus<TileSelectionChanged>.Event += HandleMouseInteraction;
        }

        void Update()
        {
            if (mode == Mode.BUILD)
            {
                directIndicator.Place(WorldToScreen(generator.grid.CellToWorld(currentMouseCoord.ToOffset())));
            }
            if (mode == Mode.TRADE)
            {
                directIndicator.Place(WorldToScreen(generator.grid.CellToWorld(currentMouseCoord.ToOffset())));
            }
        }
        private void HandleMouseInteraction(TileSelectionChanged data)
        {
            currentMouseCoord = data.node.Position;
            if (selectedBuilding == null) //TODO add check for is building
            {
                mode = Mode.TRADE;
            }

            if (mode == Mode.TRADE)
            {
                HandleTradroutePreview(data.node, data.clicked);
            }
            if (mode == Mode.BUILD)
            {
                HandlePlacementPreview(data.node, data.clicked);
            }
        }
        private void HandlePlacementPreview(WorldNode node, bool clicked)
        {
            if (selectedBuilding == null) return;
            if (!buildingInventory.BuildingInventory.ContainsKey(selectedBuilding))
            {
                Debug.Log($"no building of type: {selectedBuilding.name} in inventory");
                selectedBuilding = null;
                return;
            }
            if (!node.worldTile.isBuildable)
            {
                directIndicator.Update(new DataIndicatorDO()
                {
                    points = 0,
                    icon = "XX"
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
            directIndicator.Show();
            directIndicator.Update(new DataIndicatorDO()
            {
                points = selectedBuilding.resourceAmount, // TODO set value after calculating possible score
                icon = selectedBuilding.resourceType.name.ToString()
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
            directIndicator.Hide();
        }
        private void HandleBuildingSelection(InventoryElementSelected data)
        {
            if (data.tile == null) return;
            if (data.tile.Equals(selectedBuilding))
            {
                mode = Mode.NONE;
                selectedBuilding = null;
                return;
            }

            selectedBuilding = data.tile;
            previewNode.Initialize(selectedBuilding, new HexCoordinate(0, 0));
            previewNode.gameObject.SetActive(false);
            mode = Mode.BUILD;
            Debug.Log($"Selected {data.tile.name}");
        }

        private void HandleTradroutePreview(WorldNode node, bool clicked)
        {
            if (origin == null && clicked)
            {
                origin = node;
                return;
            }

            if (origin == null) return;

            directIndicator.Update(new DataIndicatorDO()
            {
                points = Mathf.FloorToInt(origin.Production),
                icon = origin.ResourceType.ToString()
            });
            directIndicator.Show();
            if (node.Position.Equals(origin) && clicked)
            {
                origin = null;
            }
            else
            {
                destination = node;
                PreviewRoute(origin, node);
            }

            if (clicked && origin != null && destination != null)
            {
                HandleRouteCreation(origin, destination);
            }
        }

        private void HandleRouteCreation(WorldNode origin, WorldNode destination)
        {
            if (!transportController.TryCreateRoute(origin, destination, out TransportRoute route)) return;

            routePreview.RenderLine(route.path.ConvertAll(p => generator.grid.CellToWorld(p.ToOffset())).ConvertAll(v => new Vector3(v.x, offsetHeight, v.z)));
            routePreview.ShowLine();

        }

        private void PreviewRoute(WorldNode origin, WorldNode destination)
        {

            if (!transportController.Manager.CanCreateRoute(origin, destination, out string errorMessage, out List<HexCoordinate> path))
            {
                routePreview.ChangeColor(Color.red);
                Debug.Log($"Cannot commit route: {errorMessage}");
            }
            else
            {
                routePreview.ChangeColor(Color.green);
            }

            routePreview.RenderLine(path.ConvertAll(p => generator.grid.CellToWorld(p.ToOffset())).ConvertAll(v => new Vector3(v.x, offsetHeight, v.z)));
            routePreview.ShowLine();
        }
        private Vector2 WorldToScreen(Vector3 vector)
        {
            if (mainCamera == null) return new(0, 0);

            Vector3 screenPos = mainCamera.WorldToScreenPoint(vector);
            return new(screenPos.x, screenPos.y);
        }

        private enum Mode
        {
            NONE,
            BUILD,
            TRADE
        }
    }
}