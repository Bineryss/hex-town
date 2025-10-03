using Systems.Grid;
using UnityEngine;

public interface ITileFactory
{
    INode CreateTile(HexCoordinate cellPosition, Vector3 worldPosition);

    void PregenerateTiles(int size) { }
}