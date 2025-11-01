using System.Collections.Generic;
using Systems.Building;
using Systems.Transport;
using UnityEngine;

namespace Systems.UI
{
    public class InspectUIController : MonoBehaviour, IUIController
    {
        public IUIModeSegment UIModeSegment => inspectPanel;
        [SerializeField] private TransportController transportController;

        private InspectPanel inspectPanel;

        public void Initialize()
        {
            inspectPanel = new();
        }

        public void HandleMouseInteraction(WorldNode node, WorldNode prevNode, bool isClick)
        {
            if (!isClick) return;
            if (node == null)
            {
                inspectPanel.UpdateTileInfo(new TileInformation()
                {
                    TileName = "NAN"
                });
            }
            else
            {
                Dictionary<ResourceType, int> incomingResources = transportController.Manager.GetIncomingResourcesFor(node.incomingRoutes);
                List<BonusInformation> bonusInfos = new();

                foreach (ResourceBonus bonus in node.InputBonuses)
                {
                    incomingResources.TryGetValue(bonus.input.type, out int amount);
                    bonusInfos.Add(new BonusInformation
                    {
                        ResourceType = bonus.input.type,
                        BonusMultiplier = bonus.bonusMultiplier,
                        MaxCapacity = bonus.maxCapacity,
                        CurrentInputAmount = amount
                    });
                }

                inspectPanel.UpdateTileInfo(new TileInformation
                {
                    TileName = node.worldTile.name,
                    ProductionType = node.ResourceType,
                    ProductionRate = node.Production,
                    AvailableResources = node.GetAvailableProduction(),
                    BonusInformations = bonusInfos,
                    CumulatedBonus = node.CumulatedBonus * 100,
                    SubTiles = node.ConnectedNodes.ConvertAll(subTile => new SubTile
                    {
                        Position = subTile.Position,
                        Type = subTile.ResourceType,
                        Production = subTile.Production
                    })
                });
            }
        }

        public void Exit()
        {
            inspectPanel.ExitMode();
        }

        public void Activate()
        {
            inspectPanel.EnterMode();
        }
    }
}