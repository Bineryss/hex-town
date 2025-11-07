using System.Collections.Generic;
using UnityEngine;

namespace Systems.Prototype_04
{
    [CreateAssetMenu(fileName = "BuildingPack", menuName = "Scriptable Objects/BuildingPack")]
    public class BuildingPack : ScriptableObject
    {
        public string packName;
        public List<BuildingRarity> buildings;
    }

    [System.Serializable]
    public struct BuildingRarity
    {
        public WorldTile building;
        public int min;
        public int max;
    }
}
