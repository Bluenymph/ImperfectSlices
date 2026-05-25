using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0;
    public int scoreMultiplier = 1;
    public RecipeData currentRecipeData;
    public float difficulty = 2;
    
    public delegate void AddScoreMultiplier(int multiplier);
    public static event AddScoreMultiplier OnAddScoreMultiplier;

    private int _previousMultiplier = 1;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;  
        Application.targetFrameRate = 60;
    }

    public void AddScore(int number)
    {
        score += Mathf.FloorToInt(number * (scoreMultiplier/2f));
        CalculateScoreMultiplier(currentRecipeData.maxScore);
    }

    public void RemoveScore(int number)
    {
        score -= Mathf.FloorToInt(number * (scoreMultiplier/2f));
        CalculateScoreMultiplier(currentRecipeData.maxScore);
    }

    private void CalculateScoreMultiplier(int totalScore)
    {
        scoreMultiplier = Mathf.FloorToInt(score / (totalScore / 5f)) + 1;
        scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 5);

        if (scoreMultiplier != _previousMultiplier)
        {
            OnAddScoreMultiplier?.Invoke(scoreMultiplier);
            _previousMultiplier = scoreMultiplier;
        }
    }

    #region Utilities
    public static void ShuffleList<T>(List<T> lista)
    {
        int n = lista.Count;
        while (n > 1)
        {
            n--;
            int rng = Random.Range(0, n + 1);
            
            (lista[rng], lista[n]) = (lista[n], lista[rng]);
        }
    }
    
    public static void ApplyTagRecursive(GameObject parent, string newTag)
    {
        parent.tag = newTag;
    
        foreach (Transform hijo in parent.GetComponentsInChildren<Transform>(true))
        {
            hijo.gameObject.tag = newTag;
        }
    }
    #endregion
}
