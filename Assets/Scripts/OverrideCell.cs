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
        TurnPlayer.Instance.AddOverride(overr, isOnTile, turnNumber, index);
        GetComponent<Collider>().enabled = false;
    }

    public void RemoveOverride()
    {
        TurnPlayer.Instance.RemoveOverride(isOnTile, turnNumber, index);
        GetComponent<Collider>().enabled = true;
    }
}
