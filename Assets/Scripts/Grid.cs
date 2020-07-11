using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Grid : MonoBehaviour
{
    const int GRID_SIZE = 10;
    public static Grid Instance;
    public static int Size => GRID_SIZE;


    public Tile TileAt(Vector2Int index) => Tiles[index.x, index.y];

    [SerializeField]
    GameObject tilePrefab = null;

    [SerializeField]
    Tile[] tilesArray;

    Tile[,] Tiles;

    private void Awake()
    {
        Instance = this;
        InitializeTiles();
    }

    private void InitializeTiles()
    {
        Tiles = new Tile[GRID_SIZE, GRID_SIZE];
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                Tiles[i, j] = tilesArray[LinearIndex(i, j)];
                Tiles[i, j].Index = new Vector2Int(i, j);
            }
        }
    }

    private int LinearIndex(int i, int j) => i * GRID_SIZE + j;

    public static bool IsOutOfBoundaries(Vector2Int index)
    {
        return index.x >= GRID_SIZE || index.x < 0 || index.y >= GRID_SIZE || index.y < 0;
    }

    public static Vector2Int PositionToIndex(Vector2 position) => new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));

    public static Vector2 IndexToPosition(Vector2Int index) => index;


#if UNITY_EDITOR
    [ContextMenu("Spawn Tiles")]
    public void PlaceTiles()
    {
        if (tilesArray != null)
        {
            foreach (var tile in tilesArray)
            {
                if (tile != null)
                    DestroyImmediate(tile.gameObject);
            }
        }

        tilesArray = new Tile[GRID_SIZE * GRID_SIZE];

        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                GameObject tile = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
                tile.transform.position = new Vector2(i, j);
                tile.transform.SetParent(transform);
                tilesArray[LinearIndex(i, j)] = tile.GetComponent<Tile>();
            }
        }
    }
#endif
}


