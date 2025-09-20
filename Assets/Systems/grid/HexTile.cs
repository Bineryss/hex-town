using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HexTile", menuName = "Hex Tile")]
public class HexTile : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private List<HexTileVariant> variants;

    public List<HexTileVariant> Variants => variants;
}

[Serializable]
public class HexTileVariant
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private float probability;

    public GameObject Prefab => prefab;
    public float Probability => probability;
}
