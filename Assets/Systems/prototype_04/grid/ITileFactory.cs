using Systems.Core;
using UnityEngine;

public interface ITileFactory
{
    void SetParent(Transform parent) {}
    INode CreateTile(AxialCoordinate cellPosition, Vector3 worldPosition);

    void PregenerateTiles(int size) { }
}