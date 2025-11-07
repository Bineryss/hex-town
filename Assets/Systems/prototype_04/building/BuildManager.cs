using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Systems.Prototype_04
{
    public class BuildManager : SerializedMonoBehaviour
    {
        [SerializeField] private HexGrid hexGrid;
        [OdinSerialize] private Dictionary<char, WorldTile> buildables;

        public void HandleKeyPressed(char ctx, WorldNode selected)
        {
            if (selected == null) return;
            if (!buildables.ContainsKey(ctx)) return;
            if (!selected.worldTile.isBuildable) return;

            if (buildables.TryGetValue(ctx, out WorldTile tile))
            {
                selected.name = $"{tile.resourceType}-{selected.Position}";
                selected.Initialize(tile, selected.Position);
                selected.Deselect();
            }
        }
    }
}