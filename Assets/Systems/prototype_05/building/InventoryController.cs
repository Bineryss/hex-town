using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Core;
using Systems.Prototype_04;
using Systems.Prototype_05.Score;
using Systems.Prototype_05.UI;
using UnityEngine;

namespace Systems.Prototype_05.Building
{
    public class InventoryController : SerializedMonoBehaviour
    {
        [OdinSerialize] private Dictionary<WorldTile, int> initialInventory;
        [SerializeField] private int initialPacks;
        [SerializeField] private List<ProductionPack> packs = new();

        private readonly InventoryDS inventoryDS = InventoryDS.Instance;

        public void Initialize()
        {
            EventBus<BuildingPlaced>.Event += RemoveBuilding;
            EventBus<PackUnlockThresholdReached>.Event += AddPack;
            EventBus<PackOpened>.Event += HandlePackOpen;

            OverrideInventory();
        }

        private void AddPack(PackUnlockThresholdReached data)
        {
            inventoryDS.packsLeft++;
            EventBus<BuildingInventoryChanged>.Raise();
        }

        private void RemoveBuilding(BuildingPlaced data)
        {
            inventoryDS.BuildingInventory[data.type]--;
            if (inventoryDS.BuildingInventory[data.type] == 0)
            {
                inventoryDS.BuildingInventory.Remove(data.type);
            }
            EventBus<BuildingInventoryChanged>.Raise();
        }

        private void HandlePackOpen(PackOpened data)
        {
            ProductionPack selected = packs.First(el => el.Id == data.PackId);
            if (selected == null)
            {
                Debug.Log($"Couldnt find pack with id: {data.PackId}");
                return;
            }

            foreach (BuildingRarity element in selected.buildings)
            {
                int amount = UnityEngine.Random.Range(element.min, element.max);
                if (amount > 0)
                {
                    if (inventoryDS.BuildingInventory.TryGetValue(element.building, out int quantity))
                    {
                        inventoryDS.BuildingInventory[element.building] = quantity + amount;
                    }
                    else
                    {
                        inventoryDS.BuildingInventory[element.building] = amount;
                    }
                }
            }
            inventoryDS.packsLeft--;
            EventBus<BuildingInventoryChanged>.Raise();
        }

        [Button("Sync inventory")]
        public void OverrideInventory()
        {
            inventoryDS.buildingInventory.Clear();
            foreach (var kvp in initialInventory)
            {
                inventoryDS.buildingInventory.Add(kvp.Key, kvp.Value);
            }
            inventoryDS.packsLeft = initialPacks;
            EventBus<BuildingInventoryChanged>.Raise();
        }
    }

    public struct BuildingInventoryChanged : IEvent { };
    public struct PackOpened : IEvent
    {
        public Guid PackId;
    }
}