using UnityEngine;

[CreateAssetMenu(fileName = "WorldTile", menuName = "Scriptable Objects/WorldTile")]
public class WorldTile : ScriptableObject
{
    public GameObject prefab;
    public ResourceType resourceType;
    public int resourceAmount;
    public int movementCost = 1;
    public bool isWalkable = true;
    public bool isBuildable = true;

    //TODO add cost to build, etc.

}

public enum ResourceType
{
    NONE,
    SHEEP,
    WOOD,
    WHEAT,
    BREAD,
}

