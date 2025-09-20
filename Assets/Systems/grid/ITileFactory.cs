using Systems.Grid;
using UnityEngine;

public interface ITileFactory
{
    Node CreateTile(HexCoordinate cellPosition, Vector3 worldPosition);
}