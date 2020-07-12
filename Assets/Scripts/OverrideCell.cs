using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideCell : MonoBehaviour
{
    public bool isOnTile;
    public int turnNumber;
    public Vector2Int index;
    Collider col;
    float nextTimeToCheck;
    OverrideHolder myHolder;

    private void OnEnable()
    {
        col = GetComponent<Collider>();
    }

    public void AddOverride(OverrideHolder holder)
    {
        TurnPlayer.Instance.AddOverride(holder.overr, isOnTile, turnNumber, index);
        col.enabled = false;
        holder.transform.SetParent(transform, true);
        myHolder = holder;
    }

    public void RemoveOverride()
    {
        if (myHolder != null) myHolder.transform.SetParent(null);
        TurnPlayer.Instance.RemoveOverride(isOnTile, turnNumber, index);
        col.enabled = true;
    }

    private void Update()
    {
        if (nextTimeToCheck < Time.time && isOnTile)
        {
            nextTimeToCheck = Time.time + 0.3f;
            bool b = true;
            foreach (var agent in TurnPlayer.Instance.agents)
            {
                if (agent.Index == index)
                {
                    b = false;
                    break;
                }
            }
            col.enabled = b;
        }   
    }
}
