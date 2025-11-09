using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Systems.Prototype_04;
using Systems.Prototype_05.Score;
using Systems.Prototype_05.UI;
using UnityEngine;

namespace Systems.Prototype_05.Building
{
    public class BuildingController : SerializedMonoBehaviour
    {
        public List<WorldTile> placedBuildings;
        [OdinSerialize] public Dictionary<WorldTile, int> debugInventory;
        [SerializeField] private List<ProductionPack> packs = new();

        private readonly BuildingInventory inventoryRef = BuildingInventory.Instance;
        [SerializeField] private int remainingPackCount;

        [Button("Sync inventory")]
        public void SyncInventory()
        {
            inventoryRef.buildingInventory = debugInventory;
            EventBus<BuildingInventoryChanged>.Raise();
        }

        void OnEnable()
        {
            inventoryRef.buildingInventory = debugInventory;
            EventBus<BuildingPlaced>.Event += RemoveBuilding;
            EventBus<PackUnlockThresholdReached>.Event += AddPack;
            EventBus<PackOpened>.Event += HandlePackOpen;
        }

        void Start()
        {
            EventBus<BuildingInventoryChanged>.Raise();
        }



        private void AddPack(PackUnlockThresholdReached data)
        {
            inventoryRef.PacksLeft++;
            EventBus<BuildingInventoryChanged>.Raise();
        }

        private void RemoveBuilding(BuildingPlaced data)
        {
            inventoryRef.buildingInventory[data.type]--;
            if (inventoryRef.buildingInventory[data.type] == 0)
            {
                inventoryRef.buildingInventory.Remove(data.type);
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
                    if (inventoryRef.buildingInventory.TryGetValue(element.building, out int quantity))
                    {
                        inventoryRef.buildingInventory[element.building] = quantity + amount;
                    }
                    else
                    {
                        inventoryRef.buildingInventory[element.building] = amount;
                    }
                }
            }
            inventoryRef.PacksLeft--;
            EventBus<BuildingInventoryChanged>.Raise();
        }
    }

    public struct BuildingInventoryChanged : IEvent { };
    public struct PackOpened : IEvent
    {
        public Guid PackId;
    }
}