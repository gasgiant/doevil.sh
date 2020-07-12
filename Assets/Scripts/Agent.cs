using CameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public BounceShake.Params shakeParams;

    [SerializeField]
    CommandUiManager commandUiManagerPrefab;
    CommandUiManager CommandUi;
    public Vector2Int Index;
    public Vector2Int InitialIndex;
    public int Direction;
    public int InitialDirection;

    public bool unhackable;
    public int loops;
    public List<Command> commands = null;

    public GameObject selection;
    [HideInInspector]
    public Override[] overridesOnTurns;

    public int currentTurn = 0;
    public int currentLoop = 0;

    public bool IsOnWin => Grid.Instance.TileAt(Index).type == TileType.Goal;
    public bool IsDead;

    int tweenId;

    float moveTime = 0.7f;

    private void Awake()
    {
        overridesOnTurns = new Override[commands.Count];
        InitialIndex = Grid.PositionToIndex(transform.position);
        InitialDirection = Direction;
        if (unhackable)
            GetComponent<Collider>().enabled = false;

    }

    private void Start()
    {
        CommandUi = Instantiate(commandUiManagerPrefab);
        CommandUi.agent = this;
        CommandUi.DisplayCommands();
        CommandUi.gameObject.SetActive(false);
        ResetToInitials();
    }

    public void MakeFocuse(bool b)
    {
        CommandUi.gameObject.SetActive(b);
        selection.SetActive(b);
    }

    public void SetInteractable(bool b)
    {
        if (!unhackable)
            GetComponent<Collider>().enabled = b;
    }

    public void ResetToInitials()
    {
        CommandUi.SetTurn(0);
        CommandUi.SetLoop(true);
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

            CommandUi.SetLoop(prediction);
        }

        if (!prediction)
        {
            CommandUi.SetTurn(currentTurn);
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
                    TryMoveAnimation(dir, 0.2f);
                    yield return new WaitForSeconds(0.2f);
                    yield return new WaitForSeconds(0.2f);
                }
                continue;
            }

            Agent otherAgent = OtherAgentOnTile(this, newIndex);
            if (otherAgent != null && !otherAgent.GetPushed(dir, prediction))
            {
                if (!prediction)
                {
                    TryMoveAnimation(dir, 0.2f);
                    yield return new WaitForSeconds(0.2f);
                    yield return new WaitForSeconds(0.2f);
                }
                continue;
            }

            Tile tile = Grid.Instance.TileAt(newIndex);

            Vector3 newPosition = Vector3.right * (dir.x + Index.x) + Vector3.up * (dir.y + Index.y);
            Index += dir;

            if (!prediction)
            {
                tweenId = LeanTween.move(gameObject, newPosition, moveTime).setEaseInOutCubic().id;
                yield return new WaitForSeconds(moveTime);
            }

            if (tile.type == TileType.Goal)
            {
                tile.SetIsTouched(true);
            }

            if (tile.type == TileType.Death)
            {
                Die(prediction);
                break;
            }
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
            tweenId = LeanTween.move(gameObject, newPosition, moveTime).setEaseInOutCubic().id;
        }

        return true;
    }

    void Die(bool predicition)
    {
        IsDead = true;
        if (!predicition)
            CameraShaker.Shake(new BounceShake(shakeParams));
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
                tweenId = LeanTween.rotate(gameObject, Direction * 90 * Vector3.forward, moveTime).setEaseInOutCubic().id;
                yield return new WaitForSeconds(moveTime);
            }
        }

        blocker.count--;
    }


    public void TryMoveAnimation(Vector2Int dir, float time)
    {
        Vector2 pos = transform.position;
        tweenId = LeanTween.move(gameObject, pos + (Vector2)dir * 0.1f, time * 0.5f)
            .setLoopPingPong(1).setEaseOutCubic().id;
    }    

    public Vector2Int DirectionRelative(Vector2Int dir, int rot)
    {
        Vector2 v = dir;
        v = Quaternion.AngleAxis(90 * rot, Vector3.forward) * v;
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
    }
}




