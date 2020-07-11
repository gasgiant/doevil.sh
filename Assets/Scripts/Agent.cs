﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int Index;
    [HideInInspector]
    public Vector2Int InitialIndex;

    public int loops = 2;
    public List<Command> commands = null;

    [HideInInspector]
    public Override[] overridesOnTurns;

    public int currentTurn;
    public int currentLoop;

    int tweenId;

    private void Awake()
    {
        overridesOnTurns = new Override[commands.Count];
        InitialIndex = Grid.PositionToIndex(transform.position);
        ResetToInitials();
    }

    public void ResetToInitials()
    {
        LeanTween.cancel(tweenId);
        currentTurn = 0;
        currentLoop = 0;
        Index = InitialIndex;
        transform.position = Vector3.right * Index.x + Vector3.up * Index.y;
    }

    public void IncrementTurn()
    {
        currentTurn += 1;
        if (currentTurn >= commands.Count)
        {
            currentLoop++;
            currentTurn = 0;
        }
    }

    public Command GetCommand()
    {
        if (currentLoop >= loops) return null;
        return commands[currentTurn];
    }

    public Override GetOverride()
    {
        if (currentLoop >= loops) return null;
        return overridesOnTurns[currentTurn];
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



