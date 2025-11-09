using System;
using System.Collections.Generic;
using System.Data.Common;
using Systems.Prototype_04;
using UnityEngine;

namespace Systems.Prototype_05.Building
{

    [CreateAssetMenu(fileName = "Production Pack", menuName = "Scriptable Objects/Production Pack")]
    public class ProductionPack : ScriptableObject
    {
        public string PackName;
        public List<BuildingRarity> buildings;
        public Guid Id => id;

        private Guid id = Guid.NewGuid();
    }

    [System.Serializable]
    public struct BuildingRarity
    {
        public WorldTile building;
        public int min;
        public int max;
    }
}
