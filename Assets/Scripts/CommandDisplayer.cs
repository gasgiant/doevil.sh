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
    [SerializeField]
    List<GameObject> rotations = null;


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
                ShowRotate(command);
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

    void ShowRotate(Command command)
    {
        repeats.gameObject.SetActive(true);
        rotations[0].SetActive(command.dir.x > 0);
        rotations[1].SetActive(command.dir.x < 0);
    }
}
