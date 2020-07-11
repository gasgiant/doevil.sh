using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideCell : MonoBehaviour
{
    public bool isOnTile;
    public int turnNumber;
    public Vector2Int index;

    public void AddOverride(Override overr)
    {
        TurnPlayer.Instance.WriteOverride(overr, isOnTile, turnNumber, index);
    }

    public void RemoveOverride()
    {
        TurnPlayer.Instance.RemoveOverride(isOnTile, turnNumber, index);
    }
}
