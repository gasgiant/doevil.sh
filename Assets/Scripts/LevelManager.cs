using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    int currentLevel;

    private void Awake()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadLevel(int n)
    {
        currentLevel = n;
        SceneManager.LoadScene(n);
    }

    public void ToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
