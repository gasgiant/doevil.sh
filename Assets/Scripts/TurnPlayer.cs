using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnPlayer : MonoBehaviour
{
    [SerializeField]
    Agent agent = null;
    [SerializeField]
    PredictionView predictionView = null;
    [SerializeField]
    CommandUiManager commandUi = null;

    [SerializeField]
    List<Command> commands = null;

    [SerializeField]
    List<Override> overrides = null;

    List<Vector3> predictedPositions = new List<Vector3>();

    private void Start()
    {
        commandUi.DisplayCommands(commands);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Play(3, true));
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Play());
        }
    }

    private IEnumerator Play(int loops = 3, bool prediction = false)
    {
        Blocker blocker = new Blocker();
        if (prediction)
        {
            predictedPositions.Clear();
            predictedPositions.Add(Grid.IndexToPosition(agent.InitialIndex));
        }

        for (int k = 0; k < loops; k++)
        {
            for (int i = 0; i < commands.Count; i++)
            {
                int index = i % commands.Count;

                Command command = commands[index];
                ExecuteCommand(blocker, prediction, command);

                while (blocker.IsBuisy)
                {
                    yield return null;
                }

                if (prediction)
                {
                    predictedPositions.Add(Grid.IndexToPosition(agent.Index));
                }
                else
                    yield return new WaitForSeconds(0.5f);
            }
        }

        if (prediction)
        {
            predictionView.SetPredictionData(predictedPositions);
            agent.ResetToInitials();
        }
    }

    public void ExecuteCommand(Blocker blocker, bool prediction, Command command)
    {
        switch (command.type)
        {
            case CommandType.Move:
                agent.StartCoroutine(agent.Move(blocker, prediction, command.dir, command.repeats));
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
