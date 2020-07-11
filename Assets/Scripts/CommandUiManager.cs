using System.Collections.Generic;
using UnityEngine;

public class CommandUiManager : MonoBehaviour
{
    [SerializeField]
    CommandDisplayer commandDisplayerPrefab;
    [SerializeField]
    float commandsSpacing = 1;

    public void DisplayCommands(List<Command> commands)
    {
        int count = commands.Count;
        for (int i = 0; i < count; i++)
        {
            CommandDisplayer diplayer = Instantiate(commandDisplayerPrefab);
            diplayer.transform.SetParent(transform);
            diplayer.transform.localPosition = Vector3.right * commandsSpacing * (i - count / 2);
            diplayer.Show(i, commands[i]);
        }
    }
}
