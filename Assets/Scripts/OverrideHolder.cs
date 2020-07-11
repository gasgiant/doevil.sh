using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideHolder : MonoBehaviour
{
    public Override overr;

    OverrideCell cellBinded;
    Vector3 initialPosition;
    int tweenId;

    [SerializeField]
    GameObject skipVisuals;
    [SerializeField]
    GameObject invertVisuals;
    [SerializeField]
    GameObject repeatVisuals;
    

    private void OnEnable()
    {
        initialPosition = transform.position;
        Validate();
    }

    public void BindToCell(OverrideCell cell)
    {
        cellBinded = cell;
        cell.AddOverride(overr);
        LeanTween.cancel(tweenId);
        tweenId = LeanTween.move(gameObject, cell.transform.position, 0.3f).setEaseOutCubic().id;
    }

    public void UnbindFromCell()
    {
        if (cellBinded != null)
        {
            cellBinded.RemoveOverride();
            cellBinded = null;
        }
    }

    public void ResetToInitals()
    {
        
        LeanTween.cancel(tweenId);
        tweenId = LeanTween.move(gameObject, initialPosition, 0.3f).setEaseOutCubic().id;
    }

    public void Validate()
    {
        switch (overr.type)
        {
            case OverrideType.None:
                skipVisuals.SetActive(false);
                invertVisuals.SetActive(false);
                repeatVisuals.SetActive(false);
                break;
            case OverrideType.Skip:
                skipVisuals.SetActive(true);
                invertVisuals.SetActive(false);
                repeatVisuals.SetActive(false);
                break;
            case OverrideType.Invert:
                skipVisuals.SetActive(false);
                invertVisuals.SetActive(true);
                repeatVisuals.SetActive(false);
                break;
            case OverrideType.Repeat:
                skipVisuals.SetActive(false);
                invertVisuals.SetActive(false);
                repeatVisuals.SetActive(true);
                break;
        }
    }
}
