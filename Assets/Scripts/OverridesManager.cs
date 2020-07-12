using CameraShake;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverridesManager : MonoBehaviour
{
    public BounceShake.Params shakeParams;
    [SerializeField]
    Camera cam = null;
    [SerializeField]
    LayerMask holdersLayer = 0;
    [SerializeField]
    LayerMask cellsLayer = 0;

    OverrideHolder[] holders;
    OverrideCell[] cells;

    OverrideHolder holderInHand;
    Vector3 vel;
    

    private void Awake()
    {
        holders = FindObjectsOfType<OverrideHolder>();
        
    }

    private void Start()
    {
        cells = FindObjectsOfType<OverrideCell>();
    }

    void SetHintForAllCells(bool b)
    {
        foreach (var cell in cells)
        {
            cell.SetHintActive(b);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, holdersLayer))
            {
                holderInHand = hit.transform.gameObject.GetComponent<OverrideHolder>();
                if (holderInHand != null)
                {
                    SetHintForAllCells(true);
                    holderInHand.UnbindFromCell();
                }
            }
            
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (holderInHand != null)
            {
                SetHintForAllCells(false);
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                OverrideCell cell = null;
                if (Physics.Raycast(ray, out hit, 100, cellsLayer))
                {
                    cell = hit.transform.gameObject.GetComponent<OverrideCell>();
                }

                if (cell != null)
                {
                    holderInHand.BindToCell(cell);
                    //CameraShaker.Shake(new BounceShake(shakeParams));
                    holderInHand = null;
                }
                else
                {
                    holderInHand.ResetToInitals();
                    holderInHand = null;
                }
            }
        }

        if (holderInHand != null)
        {
            Vector3 pos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
            holderInHand.transform.position = Vector3.SmoothDamp(holderInHand.transform.position, pos, ref vel, 0.05f);
        }
    }

    public void SetInteractable(bool b)
    {
        foreach (var holder in holders)
        {
            holder.SetInteractable(b);
        }
    }
}
