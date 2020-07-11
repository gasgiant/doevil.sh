using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public Vector2Int Index;

    public Vector2Int InitialIndex;

    private void Awake()
    {
        InitialIndex = Grid.PositionToIndex(transform.position);
        ResetToInitials();
    }

    public void ResetToInitials()
    {
        Index = InitialIndex;
        transform.position = Vector3.right * Index.x + Vector3.up * Index.y;
    }

    public IEnumerator Move(Blocker blocker, bool prediction, Vector2Int dir, int steps)
    {
        blocker.count++;
        for (int i = 0; i < steps; i++)
        {
            Vector2Int newIndex = Index + dir;
            if (Grid.IsOutOfBoundaries(newIndex)) break;
            Tile tile = Grid.Instance.TileAt(newIndex);
            if (tile.type == TileType.Wall) break;
            Index += dir;
            Vector3 newPosition = transform.position + Vector3.right * dir.x + Vector3.up * dir.y;

            if (!prediction)
            {
                LeanTween.move(gameObject, newPosition, 0.3f).setEaseOutCubic();
                yield return new WaitForSeconds(0.3f);
            }
        }

        blocker.count--;
    }
}



