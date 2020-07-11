using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommandDisplayer : MonoBehaviour
{
    [SerializeField]
    TextMeshPro number = null;

    [SerializeField]
    TextMeshPro repeats = null;

    [SerializeField]
    List<GameObject> moves = null;

    OverrideCell cell;

    private void OnEnable()
    {
        cell = GetComponentInChildren<OverrideCell>();
    }

    public void Show(int num, Command command)
    {
        cell.turnNumber = num;
        number.text = num.ToString();
        repeats.text = command.repeats.ToString();

        switch (command.type)
        {
            case CommandType.Move:
                ShowMove(command);
                break;
            case CommandType.Rotate:
                break;
            default:
                ShowEmpty();
                break;
        }
    }

    void ShowEmpty()
    {
        repeats.gameObject.SetActive(false);
        foreach (var move in moves)
        {
            move.SetActive(false);
        }
    }

    void ShowMove(Command command)
    {
        repeats.gameObject.SetActive(true);
        moves[0].SetActive(command.dir == Vector2Int.right);
        moves[1].SetActive(command.dir == Vector2Int.left);
        moves[2].SetActive(command.dir == Vector2Int.up);
        moves[3].SetActive(command.dir == Vector2Int.down);
    }


}
