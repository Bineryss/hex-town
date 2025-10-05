using System.Collections.Generic;
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
    public ResourceType resourceType;
    public int resourceAmount;

    [Header("Input Bonuses")]
    public List<ResourceBonus> inputBonuses = new();

    public List<ResourceType> TradeableResources => inputBonuses.ConvertAll(bonus => bonus.input);
}

[System.Serializable]
public class ResourceBonus
{
    public ResourceType input;
    public float bonusMultiplier;
}

public enum ResourceType
{
    NONE,
    SHEEP,
    WOOD,
    WHEAT,
    BREAD,
}

