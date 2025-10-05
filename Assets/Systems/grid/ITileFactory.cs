using Systems.Grid;
using UnityEngine;

public interface ITileFactory
{
    void SetParent(Transform parent) {}
    INode CreateTile(HexCoordinate cellPosition, Vector3 worldPosition);

    void PregenerateTiles(int size) { }
}