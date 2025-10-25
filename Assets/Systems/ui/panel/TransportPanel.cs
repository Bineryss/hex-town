namespace Systems.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Systems.Transport;

    public class TransportRoutePanel : VisualElement, IUIModeSegment
    {
        public event Action<TransportRoute> OnRouteSelected;
        public event Action<Guid> OnRouteDeleted;
        public event Action<WorldNode, WorldNode> OnCreateRouteConfirmed;

        private VisualElement detailContainer;
        private VisualElement detailViewContainer;
        private VisualElement creationViewContainer;

        // Detail view elements
        private Label routeOrigin;
        private Label routeDestination;
        private Label routeResource;
        private Label routeQuantity;
        private Label routeDelivered;
        private Label routeDistance;
        private Label routeEfficiency;
        private Button deleteButton;
        private Button createNewButton;

        // Creation view elements
        private Label originNodeLabel;
        private Label destinationNodeLabel;
        private Button confirmButton;
        private Button cancelButton;

        private ListView routeListView;
        private List<TransportRoute> routes;
        private TransportRoute selectedRoute;

        private WorldNode selectedOrigin;
        public WorldNode SelectedOrigin => selectedOrigin;
        private WorldNode selectedDestination;
        public WorldNode SelectedDestination => selectedDestination;
        private bool isInCreationMode = false;

        public TransportRoutePanel() : this(new List<TransportRoute>())
        {
        }

        public TransportRoutePanel(List<TransportRoute> availableRoutes)
        {
            routes = availableRoutes;

            CreateDetailZone();
            CreateListZone();

            SetDefaultDetailState();
        }

        private void CreateDetailZone()
        {
            detailContainer = new VisualElement();
            detailContainer.style.paddingTop = 10;
            detailContainer.style.paddingBottom = 10;
            detailContainer.style.paddingLeft = 15;
            detailContainer.style.paddingRight = 15;
            detailContainer.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);

            CreateDetailView();
            CreateCreationView();

            Add(detailContainer);
        }

        private void CreateDetailView()
        {
            detailViewContainer = new VisualElement();

            var header = new Label("Transport Route Details");
            header.style.fontSize = 16;
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.marginBottom = 10;
            header.style.color = Color.white;
            detailViewContainer.Add(header);

            routeOrigin = CreateDetailLabel("Origin: None");
            routeDestination = CreateDetailLabel("Destination: None");
            routeResource = CreateDetailLabel("Resource: N/A");
            routeQuantity = CreateDetailLabel("Quantity: N/A");
            routeDelivered = CreateDetailLabel("Delivered: N/A");
            routeDistance = CreateDetailLabel("Distance: N/A");
            routeEfficiency = CreateDetailLabel("Efficiency: N/A");

            var buttonRow = new VisualElement();
            buttonRow.style.flexDirection = FlexDirection.Row;
            buttonRow.style.marginTop = 10;

            deleteButton = new Button();
            deleteButton.text = "Delete Route";
            deleteButton.style.height = 30;
            deleteButton.style.flexGrow = 1;
            deleteButton.style.marginRight = 5;
            deleteButton.style.backgroundColor = new Color(0.6f, 0.2f, 0.2f, 1f);
            deleteButton.style.color = Color.white;
            deleteButton.style.fontSize = 12;
            deleteButton.RegisterCallback<ClickEvent>(OnDeleteButtonClicked);
            deleteButton.SetEnabled(false);

            createNewButton = new Button();
            createNewButton.text = "Create New Route";
            createNewButton.style.height = 30;
            createNewButton.style.flexGrow = 1;
            createNewButton.style.backgroundColor = new Color(0.2f, 0.5f, 0.2f, 1f);
            createNewButton.style.color = Color.white;
            createNewButton.style.fontSize = 12;
            createNewButton.RegisterCallback<ClickEvent>(OnCreateNewButtonClicked);

            buttonRow.Add(deleteButton);
            buttonRow.Add(createNewButton);

            detailViewContainer.Add(buttonRow);
            detailContainer.Add(detailViewContainer);
        }

        private void CreateCreationView()
        {
            creationViewContainer = new VisualElement();
            creationViewContainer.style.display = DisplayStyle.None;

            var header = new Label("Create Transport Route");
            header.style.fontSize = 16;
            header.style.unityFontStyleAndWeight = FontStyle.Bold;
            header.style.marginBottom = 10;
            header.style.color = Color.white;
            creationViewContainer.Add(header);

            var nodeSelectionRow = new VisualElement();
            nodeSelectionRow.style.flexDirection = FlexDirection.Row;
            nodeSelectionRow.style.marginBottom = 15;

            var originContainer = new VisualElement();
            originContainer.style.flexGrow = 1;
            originContainer.style.marginRight = 10;

            var originHeader = new Label("Origin Node");
            originHeader.style.fontSize = 12;
            originHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            originHeader.style.color = Color.white;
            originHeader.style.marginBottom = 5;

            originNodeLabel = new Label("Not Selected");
            originNodeLabel.style.fontSize = 12;
            originNodeLabel.style.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            originNodeLabel.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            originNodeLabel.style.paddingTop = 8;
            originNodeLabel.style.paddingBottom = 8;
            originNodeLabel.style.paddingLeft = 10;
            originNodeLabel.style.paddingRight = 10;
            originNodeLabel.style.unityTextAlign = TextAnchor.MiddleCenter;

            originContainer.Add(originHeader);
            originContainer.Add(originNodeLabel);

            var destinationContainer = new VisualElement();
            destinationContainer.style.flexGrow = 1;

            var destinationHeader = new Label("Destination Node");
            destinationHeader.style.fontSize = 12;
            destinationHeader.style.unityFontStyleAndWeight = FontStyle.Bold;
            destinationHeader.style.color = Color.white;
            destinationHeader.style.marginBottom = 5;

            destinationNodeLabel = new Label("Not Selected");
            destinationNodeLabel.style.fontSize = 12;
            destinationNodeLabel.style.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            destinationNodeLabel.style.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);
            destinationNodeLabel.style.paddingTop = 8;
            destinationNodeLabel.style.paddingBottom = 8;
            destinationNodeLabel.style.paddingLeft = 10;
            destinationNodeLabel.style.paddingRight = 10;
            destinationNodeLabel.style.unityTextAlign = TextAnchor.MiddleCenter;

            destinationContainer.Add(destinationHeader);
            destinationContainer.Add(destinationNodeLabel);

            nodeSelectionRow.Add(originContainer);
            nodeSelectionRow.Add(destinationContainer);

            creationViewContainer.Add(nodeSelectionRow);

            var actionButtonRow = new VisualElement();
            actionButtonRow.style.flexDirection = FlexDirection.Row;
            actionButtonRow.style.marginTop = 10;

            confirmButton = new Button();
            confirmButton.text = "Confirm Route";
            confirmButton.style.height = 30;
            confirmButton.style.flexGrow = 1;
            confirmButton.style.marginRight = 5;
            confirmButton.style.backgroundColor = new Color(0.2f, 0.5f, 0.2f, 1f);
            confirmButton.style.color = Color.white;
            confirmButton.style.fontSize = 12;
            confirmButton.RegisterCallback<ClickEvent>(OnConfirmButtonClicked);
            confirmButton.SetEnabled(false);

            cancelButton = new Button();
            cancelButton.text = "Cancel";
            cancelButton.style.height = 30;
            cancelButton.style.flexGrow = 1;
            cancelButton.style.backgroundColor = new Color(0.4f, 0.4f, 0.4f, 1f);
            cancelButton.style.color = Color.white;
            cancelButton.style.fontSize = 12;
            cancelButton.RegisterCallback<ClickEvent>(OnCancelButtonClicked);

            actionButtonRow.Add(confirmButton);
            actionButtonRow.Add(cancelButton);

            creationViewContainer.Add(actionButtonRow);
            detailContainer.Add(creationViewContainer);
        }

        private Label CreateDetailLabel(string text)
        {
            var label = new Label(text);
            label.style.fontSize = 12;
            label.style.marginBottom = 5;
            label.style.color = Color.white;
            return label;
        }

        private void CreateListZone()
        {
            var listContainer = new VisualElement();
            listContainer.style.flexGrow = 1;
            listContainer.style.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 1f);
            listContainer.style.paddingTop = 10;
            listContainer.style.paddingBottom = 10;

            var headerRow = CreateHeaderRow();
            listContainer.Add(headerRow);

            routeListView = new ListView
            {
                itemsSource = routes,
                fixedItemHeight = 30,
                makeItem = MakeRouteListItem,
                bindItem = BindRouteListItem,
                selectionType = SelectionType.Single,
                focusable = false
            };

            routeListView.selectionChanged += OnListSelectionChanged;
            routeListView.style.flexGrow = 1;
            routeListView.style.marginTop = 5;

            listContainer.Add(routeListView);
            Add(listContainer);
        }

        private VisualElement CreateHeaderRow()
        {
            var headerRow = new VisualElement();
            headerRow.style.flexDirection = FlexDirection.Row;
            headerRow.style.paddingLeft = 15;
            headerRow.style.paddingRight = 15;
            headerRow.style.paddingBottom = 5;
            headerRow.style.borderBottomWidth = 1;
            headerRow.style.borderBottomColor = new Color(0.3f, 0.3f, 0.3f, 1f);

            var resourceHeader = CreateHeaderLabel("Resource");
            resourceHeader.style.flexGrow = 1;

            var amountHeader = CreateHeaderLabel("Amount");
            amountHeader.style.flexGrow = 1;

            headerRow.Add(resourceHeader);
            headerRow.Add(amountHeader);

            return headerRow;
        }

        private Label CreateHeaderLabel(string text)
        {
            var label = new Label(text);
            label.style.fontSize = 12;
            label.style.color = Color.white;
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            return label;
        }

        private VisualElement MakeRouteListItem()
        {
            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.paddingLeft = 15;
            row.style.paddingRight = 15;
            row.style.paddingTop = 5;
            row.style.paddingBottom = 5;

            var resourceLabel = new Label();
            resourceLabel.style.fontSize = 12;
            resourceLabel.style.color = Color.white;
            resourceLabel.style.flexGrow = 1;

            var amountLabel = new Label();
            amountLabel.style.fontSize = 12;
            amountLabel.style.color = Color.white;
            amountLabel.style.flexGrow = 1;

            row.Add(resourceLabel);
            row.Add(amountLabel);

            return row;
        }

        private void BindRouteListItem(VisualElement element, int index)
        {
            if (index < 0 || index >= routes.Count)
                return;

            var route = routes[index];
            var labels = element.Query<Label>().ToList();

            if (labels.Count >= 2)
            {
                labels[0].text = route.resourceType.ToString();
                labels[1].text = route.GetDeliveredAmount().ToString();
            }
        }

        private void OnListSelectionChanged(IEnumerable<object> selectedItems)
        {
            if (isInCreationMode)
                return;

            foreach (var item in selectedItems)
            {
                if (item is TransportRoute route)
                {
                    selectedRoute = route;
                    UpdateDetailView(route);
                    OnRouteSelected?.Invoke(route);
                    break;
                }
            }
        }

        private void UpdateDetailView(TransportRoute route)
        {
            routeOrigin.text = $"Origin: {route.origin.Position}";
            routeDestination.text = $"Destination: {route.destination.Position}";
            routeResource.text = $"Resource: {route.resourceType}";
            routeQuantity.text = $"Quantity: {route.quantity}";
            routeDelivered.text = $"Delivered: {route.GetDeliveredAmount()}";
            routeDistance.text = $"Distance: {route.path.Count}";

            float efficiency = route.GetDeliveredAmount() / (float)route.quantity * 100f;
            routeEfficiency.text = $"Efficiency: {efficiency:F1}%";

            deleteButton.SetEnabled(true);
        }

        private void SetDefaultDetailState()
        {
            routeOrigin.text = "Origin: None";
            routeDestination.text = "Destination: None";
            routeResource.text = "Resource: N/A";
            routeQuantity.text = "Quantity: N/A";
            routeDelivered.text = "Delivered: N/A";
            routeDistance.text = "Distance: N/A";
            routeEfficiency.text = "Efficiency: N/A";
            deleteButton.SetEnabled(false);
        }

        private void OnDeleteButtonClicked(ClickEvent evt)
        {
            if (selectedRoute != null)
            {
                OnRouteDeleted?.Invoke(selectedRoute.Id);
                selectedRoute = null;
                SetDefaultDetailState();
            }
        }

        private void OnCreateNewButtonClicked(ClickEvent evt)
        {
            EnterCreationMode();
        }

        private void OnConfirmButtonClicked(ClickEvent evt)
        {
            if (selectedOrigin != null && selectedDestination != null)
            {
                OnCreateRouteConfirmed?.Invoke(selectedOrigin, selectedDestination);
                ExitCreationMode();
            }
        }

        public void ConfirmRouteCreation()
        {
            OnCreateRouteConfirmed?.Invoke(selectedOrigin, selectedDestination);
            ExitCreationMode();
        }

        private void OnCancelButtonClicked(ClickEvent evt)
        {
            ExitCreationMode();
        }

        private void EnterCreationMode()
        {
            isInCreationMode = true;
            detailViewContainer.style.display = DisplayStyle.None;
            creationViewContainer.style.display = DisplayStyle.Flex;

            selectedOrigin = null;
            selectedDestination = null;
            originNodeLabel.text = "Not Selected";
            destinationNodeLabel.text = "Not Selected";
            confirmButton.SetEnabled(false);

            routeListView.ClearSelection();
        }

        private void ExitCreationMode()
        {
            isInCreationMode = false;
            creationViewContainer.style.display = DisplayStyle.None;
            detailViewContainer.style.display = DisplayStyle.Flex;

            selectedOrigin = null;
            selectedDestination = null;
        }

        public void SetOriginNode(WorldNode node)
        {
            if (node == null)
            {
                originNodeLabel.text = "Not Selected";
            }
            else
            {
                EnterCreationMode();
                originNodeLabel.text = $"{node.Position}\n{node.ResourceType}\n{node.Production}";
            }
            selectedOrigin = node;
            UpdateConfirmButtonState();
        }

        public void SetDestinationNode(WorldNode node)
        {
            selectedDestination = node;
            if (node == null)
            {
                destinationNodeLabel.text = "Not Selected";
            }
            else
            {
                destinationNodeLabel.text = $"{node.Position}\n{node.ResourceType}\n{node.Production}";
            }
            UpdateConfirmButtonState();
        }

        private void UpdateConfirmButtonState()
        {
            confirmButton.SetEnabled(selectedOrigin != null && selectedDestination != null);
        }

        public bool IsInCreationMode()
        {
            return isInCreationMode;
        }

        public void UpdateRoutes(List<TransportRoute> newRoutes)
        {
            routes = newRoutes;
            routeListView.itemsSource = routes;
            routeListView.Rebuild();

            if (selectedRoute != null && !routes.Contains(selectedRoute))
            {
                selectedRoute = null;
                SetDefaultDetailState();
                routeListView.ClearSelection();
            }
        }

        public TransportRoute GetSelectedRoute()
        {
            return selectedRoute;
        }

        public void EnterMode()
        {
            style.display = DisplayStyle.Flex;
        }

        public void ExitMode()
        {
            style.display = DisplayStyle.None;
        }
    }

}