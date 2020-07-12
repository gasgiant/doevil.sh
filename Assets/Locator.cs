using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locator : MonoBehaviour
{
    public void LoadLevel(int n)
    {
        LevelManager.Instance.LoadLevel(n);
    }
}
