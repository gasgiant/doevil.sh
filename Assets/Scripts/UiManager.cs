using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UiManager : MonoBehaviour
{
    [SerializeField]
    Button runButton = null;
    [SerializeField]
    Button resetButton = null;

    [SerializeField]
    GameObject loseScreen = null;
    [SerializeField]
    GameObject winScreen = null;


    public void SwitchRun(bool run)
    {
        runButton.interactable = run;
    }

    public void ShowLoseSceen()
    {
        loseScreen.SetActive(true);
    }

    public void ShowWinScreen()
    {
        winScreen.SetActive(true);
    }

    public void ResetToDefaults()
    {
        loseScreen.SetActive(false);
        winScreen.SetActive(false);
        SwitchRun(true);
    }
}
