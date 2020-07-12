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
    GameObject middle = null;
    [SerializeField]
    SpriteRenderer edge = null;

    Color edgeNormalColor;

    [SerializeField]
    GameObject skipVisuals = null;
    [SerializeField]
    GameObject invertVisuals = null;
    [SerializeField]
    GameObject repeatVisuals = null;

    bool initialized;
    

    private void OnEnable()
    {
        if (!initialized)
        {
            initialized = true;
            edgeNormalColor = edge.color;
            initialPosition = transform.position;
            Validate();
        }
    }

    public void SetAlpha(bool b)
    {
        Color c = new Color(0.3f, 0.3f, 0.3f, 1);
        skipVisuals.GetComponent<SpriteRenderer>().color = b ? Color.white : c;
        invertVisuals.GetComponent<SpriteRenderer>().color = b ? Color.white : c;
        repeatVisuals.GetComponent<SpriteRenderer>().color = b ? Color.white : c;
    }

    public void SetInteractable(bool b)
    {
        GetComponent<Collider>().enabled = b;
        SetAlpha(b);
        //edge.color = b ? edgeNormalColor : new Color(1, 1, 1, 0.6f);
    }

    public void BindToCell(OverrideCell cell)
    {
        cellBinded = cell;
        cell.AddOverride(this);
        LeanTween.cancel(tweenId);
        tweenId = LeanTween.move(gameObject, cell.transform.position, 0.3f).setEaseOutCubic().id;
        //if (cell.isOnTile)
        //    middle.SetActive(false);
    }

    public void UnbindFromCell()
    {
        if (cellBinded != null)
        {
            cellBinded.RemoveOverride();
            cellBinded = null;
        }
        middle.SetActive(true);
    }

    public void ResetToInitals()
    {
        //edge.color = edgeNormalColor;
        SetAlpha(true);
        LeanTween.cancel(tweenId);
        tweenId = LeanTween.move(gameObject, initialPosition, 0.3f).setEaseOutCubic().id;
        GetComponent<Collider>().enabled = true;
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
