using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public Vector2Int Index;

    public Vector2Int InitialIndex;

    int tweenId;

    private void Awake()
    {
        InitialIndex = Grid.PositionToIndex(transform.position);
        ResetToInitials();
    }

    public void ResetToInitials()
    {
        LeanTween.cancel(tweenId);
        Index = InitialIndex;
        transform.position = Vector3.right * Index.x + Vector3.up * Index.y;
    }

    public IEnumerator Move(Blocker blocker, CommandResult result, bool prediction, Vector2Int dir, int repeats)
    {
        blocker.count++;
        for (int i = 0; i < repeats; i++)
        {
            Vector2Int newIndex = Index + dir;
            
            if (Grid.IsOutOfBoundaries(newIndex) || Grid.Instance.TileAt(newIndex).type == TileType.Wall)
            {
                if (!prediction)
                {
                    TryMoveAnimation(dir, 0.3f);
                    yield return new WaitForSeconds(0.3f);
                    yield return new WaitForSeconds(0.3f);
                }
                continue;
            }

            Tile tile = Grid.Instance.TileAt(newIndex);

            Vector3 newPosition = Vector3.right * (dir.x + Index.x) + Vector3.up * (dir.y + Index.y);
            Index += dir;

            if (!prediction)
            {
                tweenId = LeanTween.move(gameObject, newPosition, 0.3f).setEaseOutCubic().id;
                yield return new WaitForSeconds(0.3f);
            }

            if (tile.type == TileType.Goal)
            {
                result.type = CommandResultType.Goal;
                break;
            }

            if (tile.type == TileType.Death)
            {
                result.type = CommandResultType.Death;
                break;
            }
        }

        blocker.count--;
    }

    public void TryMoveAnimation(Vector2Int dir, float time)
    {
        Vector2 pos = transform.position;
        tweenId = LeanTween.move(gameObject, pos + (Vector2)dir * 0.2f, time * 0.5f)
            .setLoopPingPong(1).setEaseOutCubic().id;
    }    
}

public enum CommandResultType { None, Goal, Death }

public class CommandResult
{
    public CommandResultType type;
}



