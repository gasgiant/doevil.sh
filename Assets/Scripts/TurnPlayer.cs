using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPlayer : MonoBehaviour
{
    public static TurnPlayer Instance;

    public Agent focusAgent;
    [SerializeField]
    PredictionView predictionView = null;
    public CommandUiManager CommandUi;
    [SerializeField]
    UiManager uiManager = null;

    Override[,] overridesOnTiles;

    List<Vector3> predictedPositions = new List<Vector3>();

    Coroutine playRoutine;

    private void Awake()
    {
        Instance = this;
        overridesOnTiles = new Override[Grid.Size, Grid.Size];
        CommandUi = FindObjectOfType<CommandUiManager>();
    }

    private void Start()
    {
        CommandUi.DisplayCommands(focusAgent.commands);
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
        uiManager.ResetToDefaults();
        focusAgent.ResetToInitials();
        CommandUi.SetTurn(0);
    }

    public void AddOverride(Override overr, bool onTile, int turnNumber, Vector2Int index)
    {
        if (onTile)
            overridesOnTiles[index.x, index.y] = overr;
        else
            focusAgent.overridesOnTurns[turnNumber] = overr;
        Play(true);
    }

    public void RemoveOverride(bool onTile, int turnNumber, Vector2Int index)
    {
        if (onTile)
            overridesOnTiles[index.x, index.y] = null;
        else
            focusAgent.overridesOnTurns[turnNumber] = null;
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
        if (!prediction) uiManager.SwitchRun(false);
        playRoutine = StartCoroutine(PlayCoroutine(prediction));
    }

    private IEnumerator PlayCoroutine(bool prediction)
    {
        Blocker blocker = new Blocker();
        CommandResult result = new CommandResult();
        bool isFinished = false;

        if (prediction)
        {
            predictedPositions.Clear();
            predictedPositions.Add(Grid.IndexToPosition(focusAgent.InitialIndex));
        }

        while (true)
        {
            Command command = focusAgent.GetCommand();
            if (overridesOnTiles[focusAgent.Index.x, focusAgent.Index.y] != null)
                command = overridesOnTiles[focusAgent.Index.x, focusAgent.Index.y].GetResult(command);
            Override iverrideOnTurn = focusAgent.GetOverride();
            if (iverrideOnTurn != null)
                command = iverrideOnTurn.GetResult(command);

            focusAgent.ExecuteCommand(blocker, result, prediction, command);

            while (blocker.IsBuisy)
            {
                yield return null;
            }

            focusAgent.IncrementTurn();

            if (result.type == CommandResultType.Death)
            {
                if (!prediction)
                {
                    uiManager.ShowLoseSceen();
                }
                isFinished = true;
            }
            if (result.type == CommandResultType.Goal)
            {
                if (!prediction)
                {
                    uiManager.ShowWinSceen();
                }
                isFinished = true;
            }

            if (prediction)
            {
                predictedPositions.Add(Grid.IndexToPosition(focusAgent.Index));
            }
            else
            {
                if (!isFinished)
                {
                    CommandUi.SetTurn(focusAgent.currentTurn);
                    yield return new WaitForSeconds(0.5f);
                }
            }

            
            isFinished = isFinished || focusAgent.GetCommand() == null;

            if (isFinished) break;
        }

        if (prediction)
        {
            predictionView.SetPredictionData(predictedPositions);
        }

        playRoutine = null;
    }
}

public class Blocker
{
    public int count;
    public bool IsBuisy => count > 0;
}
