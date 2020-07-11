using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPlayer : MonoBehaviour
{
    public static TurnPlayer Instance;

    [SerializeField]
    Agent agent = null;
    [SerializeField]
    PredictionView predictionView = null;
    [SerializeField]
    CommandUiManager commandUi = null;

    [SerializeField]
    List<Command> commands = null;

    Override[] overridesOnTurns;
    Override[,] overridesOnTiles;

    List<Vector3> predictedPositions = new List<Vector3>();

    Coroutine playRoutine;

    private void Awake()
    {
        Instance = this;
        overridesOnTurns = new Override[commands.Count];
        overridesOnTiles = new Override[Grid.Size, Grid.Size];
    }

    private void Start()
    {
        commandUi.DisplayCommands(commands);
        Play(true);
    }

    public void Run()
    {
        Play();
    }

    public void ResetEverything()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }
        agent.ResetToInitials();
        commandUi.SetTurn(0);
    }

    public void AddOverride(Override overr, bool onTile, int turnNumber, Vector2Int index)
    {
        if (onTile)
            overridesOnTiles[index.x, index.y] = overr;
        else
            overridesOnTurns[turnNumber] = overr;
        Play(true);
    }

    public void RemoveOverride(bool onTile, int turnNumber, Vector2Int index)
    {
        if (onTile)
            overridesOnTiles[index.x, index.y] = null;
        else
            overridesOnTurns[turnNumber] = null;
        Play(true);
    }

    private void Play(bool prediction = false)
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }
        ResetEverything();
        playRoutine = StartCoroutine(PlayCoroutine(prediction));
    }

    private IEnumerator PlayCoroutine(bool prediction)
    {
        Blocker blocker = new Blocker();
        CommandResult result = new CommandResult();
        if (prediction)
        {
            predictedPositions.Clear();
            predictedPositions.Add(Grid.IndexToPosition(agent.InitialIndex));
        }

        for (int i = 0; i < commands.Count; i++)
        {
            int index = i % commands.Count;

            Command command = commands[index];
            if (overridesOnTiles[agent.Index.x, agent.Index.y] != null)
                command = overridesOnTiles[agent.Index.x, agent.Index.y].GetResult(command);
            if (overridesOnTurns[index] != null)
                command = overridesOnTurns[index].GetResult(command);

            ExecuteCommand(blocker, result, prediction, command);

            while (blocker.IsBuisy)
            {
                yield return null;
            }

            if (prediction)
            {
                predictedPositions.Add(Grid.IndexToPosition(agent.Index));
            }
            else
            {
                if (index < commands.Count - 1)
                    commandUi.SetTurn(index + 1);
                yield return new WaitForSeconds(0.5f);
            }

            if (result.type == CommandResultType.Death)
            {
                break;
            }

            if (result.type == CommandResultType.Goal)
            {
                break;
            }
        }

        if (prediction)
        {
            predictionView.SetPredictionData(predictedPositions);
        }

        playRoutine = null;
    }

    public void ExecuteCommand(Blocker blocker, CommandResult result, bool prediction, Command command)
    {
        result.type = CommandResultType.None;
        switch (command.type)
        {
            case CommandType.Move:
                agent.StartCoroutine(agent.Move(blocker, result, prediction, command.dir, command.repeats));
                break;
            default:
                break;
        }
    }
}

public class Blocker
{
    public int count;
    public bool IsBuisy => count > 0;
}
