using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexTile", menuName = "Hex Tile")]
public class HexTile : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private List<GameObject> variants;
}
