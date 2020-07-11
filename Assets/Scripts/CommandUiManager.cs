using System.Collections.Generic;
using UnityEngine;

public class CommandUiManager : MonoBehaviour
{
    [SerializeField]
    CommandDisplayer commandDisplayerPrefab = null;
    [SerializeField]
    float commandsSpacing = 1;

    List<Transform> displayers = new List<Transform>();

    public void DisplayCommands(List<Command> commands)
    {
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
    }
}
