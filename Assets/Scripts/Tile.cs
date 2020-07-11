using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int Index;
    public TileType type = TileType.Free;
}

public enum TileType { Free, Wall }
