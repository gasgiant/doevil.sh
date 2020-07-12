using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public TextMeshPro boot;

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

    private void Start()
    {
        if (boot != null)
        {
            StartCoroutine(Booter(boot, boot.text));
        }
    }

    private IEnumerator Booter(TextMeshPro tmp, string text)
    {
        for (int i = 0; i <= text.Length; i++)
        {
            tmp.text = text.Substring(0, i);
            yield return new WaitForSeconds(0.01f);
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
