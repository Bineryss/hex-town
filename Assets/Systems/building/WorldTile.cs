using System.Collections.Generic;
using Systems.Building;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldTile", menuName = "Scriptable Objects/WorldTile")]
public class WorldTile : ScriptableObject
{
    [Header("Tile Properties")]
    public GameObject prefab;
    public int movementCost = 1;
    public bool isWalkable = true;
    public bool isBuildable = true;

    [Header("Production Properties")]
    public Resource resourceType;
    public int resourceAmount;
    public List<WorldTile> connectableTiles = new();

    [Header("Input Bonuses")]
    public List<ResourceBonus> inputBonuses = new();

    public List<ResourceType> TradeableResources => inputBonuses.ConvertAll(bonus => bonus.input.type);
}

[System.Serializable]
public class ResourceBonus
{
    public Resource input;
    public int bonusMultiplier;
    public int maxCapacity;
}



