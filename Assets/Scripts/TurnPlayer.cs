using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPlayer : MonoBehaviour
{
    public int nextLevel;
    public static TurnPlayer Instance;

    public Camera cam;
    public LayerMask agentsLayer;
    public Agent focusAgent;
    public List<Agent> agents;
    

    [SerializeField]
    PredictionView predictionView = null;
    
    [SerializeField]
    UiManager uiManager = null;

    OverridesManager overrideManager;

    Override[,] overridesOnTiles;

    List<Vector3> predictedPositions = new List<Vector3>();

    Coroutine playRoutine;

    List<Tile> goalTiles = new List<Tile>();

    private void Awake()
    {
        Instance = this;
        overridesOnTiles = new Override[Grid.Size, Grid.Size];
        overrideManager = FindObjectOfType<OverridesManager>();
    }

    private void Start()
    {
        foreach (var item in Grid.Instance.Tiles)
        {
            if (item.type == TileType.Goal)
                goalTiles.Add(item);
        }

        SetFocusOn(focusAgent);
        Play(true);
    }

    private void SetGoals(bool b)
    {
        foreach (var item in goalTiles)
        {
            item.SetIsTouched(b);
        }
    }

    private bool CheckGoals()
    {
        foreach (var item in goalTiles)
        {
            if (!item.IsTouched) return false;
        }
        return true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, agentsLayer))
            {
                Agent a = hit.transform.gameObject.GetComponent<Agent>();
                if (a != null)
                {
                    SetFocusOn(a);
                    Play(true);
                }
            }

        }
    }

    void SetFocusOn(Agent agent)
    {
        focusAgent = agent;
        foreach (var item in agents)
        {
            item.MakeFocuse(agent == item);
        }
    }

    public void ToNextLevel()
    {
        LevelManager.Instance.LoadLevel(nextLevel);
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

        foreach (var agent in agents)
        {
            agent.ResetToInitials();
        }
        uiManager.ResetToDefaults();
        
        overrideManager.SetInteractable(true);
        foreach (var agent in agents)
        {
            agent.SetInteractable(true);
        }

        SetGoals(false);
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
        bool isFinished = false;

        if (prediction)
        {
            predictedPositions.Clear();
            predictedPositions.Add(Grid.IndexToPosition(focusAgent.InitialIndex));
        }
        
        if (!prediction)
        {
            overrideManager.SetInteractable(false);
            foreach (var agent in agents)
            {
                agent.SetInteractable(false);
            }
        }

        while (true)
        {
            foreach (var agent in agents)
            {
                PlayAgentTurn(agent, blocker, prediction);
                while (blocker.IsBuisy)
                {
                    yield return null;
                }
                agent.IncrementTurn(prediction);
            }

            bool allHackableDead = true;
            foreach (var agent in agents)
            {
                if (!agent.unhackable && !agent.IsDead)
                {
                    allHackableDead = false;
                }
            }

            if (allHackableDead)
            {
                if (!prediction)
                {
                    yield return new WaitForSeconds(1.1f);
                    uiManager.ShowLoseSceen();
                }
                isFinished = true;
            }
            else
            {
                if (CheckGoals())
                {
                    if (!prediction)
                    {
                        uiManager.ShowWinScreen();
                    }
                    isFinished = true;
                }
            }

            if (prediction)
            {
                predictedPositions.Add(Grid.IndexToPosition(focusAgent.Index));
            }
            else
            {
                if (!isFinished)
                {
                    yield return new WaitForSeconds(0.3f);
                }
            }

            if (!isFinished)
            {
                bool b = true;
                foreach (var agent in agents)
                {
                    b = b && agent.GetCommand() == null;
                }
                isFinished = b;
            }

            if (isFinished) break;
        }

        if (prediction)
        {
            predictionView.SetPredictionData(predictedPositions);
            ResetEverything();
        }

        playRoutine = null;
    }

    void PlayAgentTurn(Agent agent, Blocker blocker, bool prediction)
    {
        Command command = agent.GetCommand();
        if (command != null && !agent.IsDead)
        {
            if (overridesOnTiles[agent.Index.x, agent.Index.y] != null)
                command = overridesOnTiles[agent.Index.x, agent.Index.y].GetResult(command);
            Override overrideOnTurn = agent.GetOverride();
            if (overrideOnTurn != null)
                command = overrideOnTurn.GetResult(command);
            agent.ExecuteCommand(blocker, prediction, command);
        }
    }
}

public class Blocker
{
    public int count;
    public bool IsBuisy => count > 0;
}
