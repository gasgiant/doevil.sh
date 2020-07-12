using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    [SerializeField]
    CommandUiManager commandUiManagerPrefab;
    CommandUiManager CommandUi;
    public Vector2Int Index;
    public Vector2Int InitialIndex;
    public int Direction;
    public int InitialDirection;

    public int loops = 2;
    public List<Command> commands = null;

    [HideInInspector]
    public Override[] overridesOnTurns;

    public int currentTurn;
    public int currentLoop;

    public bool IsOnWin => Grid.Instance.TileAt(Index).type == TileType.Goal;
    public bool IsDead;

    int tweenId;

    private void Awake()
    {
        overridesOnTurns = new Override[commands.Count];
        InitialIndex = Grid.PositionToIndex(transform.position);
        InitialDirection = Direction;
        
    }

    private void Start()
    {
        CommandUi = Instantiate(commandUiManagerPrefab);
        CommandUi.agent = this;
        CommandUi.DisplayCommands();
        ResetToInitials();
    }

    public void ResetToInitials()
    {
        CommandUi.SetTurn(0);
        LeanTween.cancel(tweenId);
        IsDead = false;
        currentTurn = 0;
        currentLoop = 0;
        Index = InitialIndex;
        Direction = InitialDirection;
        transform.position = Vector3.right * Index.x + Vector3.up * Index.y;
        transform.rotation = Quaternion.AngleAxis(90 * Direction, Vector3.forward);
    }

    public void IncrementTurn(bool prediction)
    {
        currentTurn += 1;
        if (currentTurn >= commands.Count)
        {
            currentLoop++;
            currentTurn = 0;
        }

        if (!prediction) CommandUi.SetTurn(currentTurn);
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

    public void ExecuteCommand(Blocker blocker, bool prediction, Command command)
    {
        switch (command.type)
        {
            case CommandType.Move:
                StartCoroutine(Move(blocker, prediction, command.dir, command.repeats));
                break;
            case CommandType.Rotate:
                StartCoroutine(Roatate(blocker, prediction, command.dir, command.repeats));
                break;
            default:
                break;
        }
        
    }

    public IEnumerator Move(Blocker blocker, bool prediction, Vector2Int dir, int repeats)
    {
        blocker.count++;
        dir = DirectionRelative(dir, Direction);
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

            Agent otherAgent = OtherAgentOnTile(this, newIndex);
            if (otherAgent != null && !otherAgent.GetPushed(dir, prediction))
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

            if (tile.type == TileType.Goal) break;

            if (tile.type == TileType.Death) Die(prediction);
        }

        blocker.count--;
    }

    public bool GetPushed(Vector2Int dir, bool prediction)
    {
        Vector2Int newIndex = Index + dir;
        if (Grid.IsOutOfBoundaries(newIndex) || Grid.Instance.TileAt(newIndex).type == TileType.Wall)
            return false;

        Agent otherAgent = OtherAgentOnTile(this, newIndex);

        if (otherAgent != null && !otherAgent.GetPushed(dir, prediction))
        {
            return false;
        }      

        Vector3 newPosition = Vector3.right * (dir.x + Index.x) + Vector3.up * (dir.y + Index.y);
        Index += dir;
        if (Grid.Instance.TileAt(Index).type == TileType.Death)
            Die(prediction);

        if (!prediction)
        {
            tweenId = LeanTween.move(gameObject, newPosition, 0.3f).setEaseOutCubic().id;
        }

        return true;
    }

    void Die(bool predicition)
    {
        IsDead = true;
    }

    Agent OtherAgentOnTile(Agent agent, Vector2Int index)
    {
        Agent res = null;
        foreach (var item in TurnPlayer.Instance.agents)
        {
            if (item.Index == index && item != agent)
                res = item;
        }
        return res;
    }

    public IEnumerator Roatate(Blocker blocker, bool prediction, Vector2Int dir, int repeats)
    {
        blocker.count++;
        for (int i = 0; i < repeats; i++)
        {
            Direction = (Direction + dir.x) % 4;
            if (!prediction)
            {
                tweenId = LeanTween.rotate(gameObject, Direction * 90 * Vector3.forward, 0.3f).setEaseOutCubic().id;
                yield return new WaitForSeconds(0.3f);
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

    public Vector2Int DirectionRelative(Vector2Int dir, int rot)
    {
        Vector2 v = dir;
        v = Quaternion.AngleAxis(90 * rot, Vector3.forward) * v;
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    }
}




