﻿using UnityEngine;

public class Tile : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int Index;
    public TileType type = TileType.Free;

    public bool IsTouched => isTouched;

    bool isTouched;

    OverrideCell cell;

    [SerializeField]
    GameObject freeVisuals = null;
    [SerializeField]
    GameObject wallVisuals = null;
    [SerializeField]
    GameObject deathVisuals = null;
    [SerializeField]
    GameObject goalVisuals = null;

    private void Start()
    {
        cell = GetComponentInChildren<OverrideCell>();
        cell.isOnTile = true;
        cell.index = Index;
    }

    public void SetIsTouched(bool b)
    {
        goalVisuals.SetActive(!b);
        isTouched = b;
    }

    public void Validate()
    {
        switch (type)
        {
            case TileType.Free:
                freeVisuals.SetActive(true);
                wallVisuals.SetActive(false);
                goalVisuals.SetActive(false);
                deathVisuals.SetActive(false);
                break;
            case TileType.Wall:
                freeVisuals.SetActive(false);
                wallVisuals.SetActive(true);
                goalVisuals.SetActive(false);
                deathVisuals.SetActive(false);
                break;
            case TileType.Goal:
                freeVisuals.SetActive(false);
                wallVisuals.SetActive(false);
                goalVisuals.SetActive(true);
                deathVisuals.SetActive(false);
                break;
            case TileType.Death:
                freeVisuals.SetActive(false);
                wallVisuals.SetActive(false);
                goalVisuals.SetActive(false);
                deathVisuals.SetActive(true);
                break;
        }
    }
}

public enum TileType { Free, Wall, Goal, Death }
