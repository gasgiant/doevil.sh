using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Grid : MonoBehaviour
{
    const int GRID_SIZE = 10;
    public static int GridSize => GRID_SIZE;
    public Tile[,] Tiles;

    public GameObject tilePrefab;
    public float tileSize = 1;

    [SerializeField]
    Tile[] tilesArray;

    private void Start()
    {
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


