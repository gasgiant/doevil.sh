using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommandUiManager : MonoBehaviour
{
    public Agent agent;

    [SerializeField]
    CommandDisplayer commandDisplayerPrefab = null;
    [SerializeField]
    float commandsSpacing = 1;
    [SerializeField]
    Transform turnCoursor = null;
    public float turnCoursorSpacing = 1;

    [SerializeField]
    Transform loopsCounter = null;
    [SerializeField]
    Transform loopsLine = null;
    [SerializeField]
    TextMeshPro loopsText = null;

    List<Transform> displayers = new List<Transform>();

    public void DisplayCommands()
    {
        List<Command> commands = agent.commands;
        displayers.Clear();
        int count = commands.Count;
        for (int i = 0; i < count; i++)
        {
            CommandDisplayer diplayer = Instantiate(commandDisplayerPrefab);
            diplayer.transform.SetParent(transform);
            diplayer.transform.localPosition = Vector3.right * commandsSpacing * (i - count / 2);
            diplayer.Show(i, commands[i]);
            displayers.Add(diplayer.transform);
        }
        turnCoursor.position = displayers[0].position + Vector3.up * turnCoursorSpacing;
        loopsCounter.position = transform.position + Vector3.up * turnCoursorSpacing + Vector3.left * 0.5f * ((count + 1) % 2) * commandsSpacing;
        Vector3 v = loopsLine.localScale;
        v.x = commandsSpacing * count;
        loopsLine.localScale = v;

        if (agent.loops > 1)
        {
            loopsText.text = "LOOPS " + agent.loops;
        }
        else
        {
            loopsText.text = "";
        }
    }

    public void SetLoop(bool prediction)
    {
        if (prediction)
        {
            if (agent.loops > 1)
            {
                loopsText.text = "LOOPS " + agent.loops;
            }
            else
            {
                loopsText.text = "";
            }
        }
        else
        {
            if (agent.loops > 1)
            {
                loopsText.text = "LOOPS " + (agent.loops - agent.currentLoop);
            }
            else
            {
                loopsText.text = "";
            }
        }
        
    }

    public void SetTurn(int i)
    {
        LeanTween.move(turnCoursor.gameObject, displayers[i].position + Vector3.up * turnCoursorSpacing, 0.3f)
            .setEaseInOutCubic();
    }
}
