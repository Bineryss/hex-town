using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Grid;
using Systems.Transport;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.UI
{

    public class GameUIController : SerializedMonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private PlayerGridSelector playerGridSelector;
        [SerializeField] private List<ModeElement> modes = new();

        [SerializeField] private BuildingUIController buildingUIController; //TODO extract with resource overview

        [Header("Debug Info")]
        [SerializeField, ReadOnly] private WorldNode prevSelectedNode;
        [SerializeField] private UIState currentState = UIState.INSPECTING;

        private VisualElement Root => uiDocument.rootVisualElement;
        private ResourceOverview resourceOverview;
        private ModeSelectionPanel modeSelectionPanel;
        [OdinSerialize] private readonly Dictionary<UIState, IUIController> uiControllers = new();

        void Update()
        {
            resourceOverview.UpdateResources(buildingUIController.PlacedBuildings.GroupBy(b => b.ResourceType).Select(g => new ResourceInfo
            {
                ResourceType = g.Key.ToString(),
                Quantity = g.Sum(b => b.GetAvailableProduction())
            }).ToList());
        }

        void Start()
        {
            if (uiDocument == null && TryGetComponent(out UIDocument uiDoc))
            {
                uiDocument = uiDoc;
            }
        }

        void OnEnable()
        {
            foreach (IUIController controller in uiControllers.Values)
            {
                controller.Initialize();
            }

            modeSelectionPanel = new(modes);
            resourceOverview = new();

            playerGridSelector.OnChange += HandleMouseChange;
            modeSelectionPanel.OnModeSelected += HandleModeSelection;

            BuildUI();
        }

        private void HandleMouseChange(WorldNode node, bool isClick)
        {
            if (node == null) return;
            if (currentState == UIState.EXPLORING) return;
            if (prevSelectedNode != null)
            {
                prevSelectedNode.Deselect();
            }
            node.Select();
            uiControllers[currentState].HandleMouseInteraction(node, prevSelectedNode, isClick);
            prevSelectedNode = node;
        }

        private void HandleModeSelection(UIState state)
        {
            if (state == currentState) return;

            if (prevSelectedNode != null)
            {
                prevSelectedNode.Deselect();
                prevSelectedNode = null;
            }

            foreach (IUIController controller in uiControllers.Values)
            {
                controller.Exit();
            }


            if (uiControllers.TryGetValue(state, out var selectedMode))
            {
                selectedMode.Activate();
            }
            currentState = state;
        }

        private void BuildUI()
        {
            Root.Clear();
            Root.Add(resourceOverview);

            VisualElement menuContainer = new();
            menuContainer.style.maxWidth = 300;
            menuContainer.style.flexGrow = 1;
            menuContainer.style.flexDirection = FlexDirection.Column;
            foreach (IUIController segment in uiControllers.Values)
            {
                segment.Exit();
                menuContainer.Add(segment.UIModeSegment as VisualElement);
            }

            Root.Add(menuContainer);

            Root.Add(modeSelectionPanel);
        }
    }
}