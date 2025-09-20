using Systems.Grid;
using TMPro;
using UnityEngine;

public class DebugTileFactory : MonoBehaviour, ITileFactory
{
    [SerializeField] private GameObject debugTilePrefab;

    public Node CreateTile(HexCoordinate cellPosition, Vector3 worldPosition)
    {
        if (debugTilePrefab == null) return null;

        GameObject instance = Instantiate(debugTilePrefab, worldPosition, Quaternion.identity, transform);

        instance.name = $"node_{cellPosition.Q}:{cellPosition.R}:{cellPosition.S}";
        instance.transform.Find("Canvas/label").GetComponent<TMP_Text>().text =
            $"-{cellPosition.Distance(new(0, 0))}-\n{cellPosition.Q},{cellPosition.R},{cellPosition.S}";
        return new Node()
        {
            instance = instance,
            position = cellPosition
        }; ;
    }
}