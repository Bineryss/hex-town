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

    private VisualElement detailContainer;
    private Label routeOrigin;
    private Label routeDestination;
    private Label routeResource;
    private Label routeQuantity;
    private Label routeDelivered;
    private Label routeDistance;
    private Label routeEfficiency;
    private Button deleteButton;

    private ListView routeListView;
    private List<TransportRoute> routes;
    private TransportRoute selectedRoute;

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

        var header = new Label("Transport Route Details");
        header.style.fontSize = 16;
        header.style.unityFontStyleAndWeight = FontStyle.Bold;
        header.style.marginBottom = 10;
        header.style.color = Color.white;
        detailContainer.Add(header);

        routeOrigin = CreateDetailLabel("Origin: None");
        routeDestination = CreateDetailLabel("Destination: None");
        routeResource = CreateDetailLabel("Resource: N/A");
        routeQuantity = CreateDetailLabel("Quantity: N/A");
        routeDelivered = CreateDetailLabel("Delivered: N/A");
        routeDistance = CreateDetailLabel("Distance: N/A");
        routeEfficiency = CreateDetailLabel("Efficiency: N/A");

        deleteButton = new Button();
        deleteButton.text = "Delete Route";
        deleteButton.style.marginTop = 10;
        deleteButton.style.height = 30;
        deleteButton.style.backgroundColor = new Color(0.6f, 0.2f, 0.2f, 1f);
        deleteButton.style.color = Color.white;
        deleteButton.style.fontSize = 12;
        deleteButton.RegisterCallback<ClickEvent>(OnDeleteButtonClicked);
        deleteButton.SetEnabled(false);

        detailContainer.Add(deleteButton);
        Add(detailContainer);
    }

    private Label CreateDetailLabel(string text)
    {
        var label = new Label(text);
        label.style.fontSize = 12;
        label.style.marginBottom = 5;
        label.style.color = Color.white;
        detailContainer.Add(label);
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

    public void UpdateRoutes(List<TransportRoute> newRoutes)
    {
        routes = newRoutes;
        routeListView.itemsSource = routes;
        routeListView.Rebuild();
        
        // Clear selection if current route no longer exists
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