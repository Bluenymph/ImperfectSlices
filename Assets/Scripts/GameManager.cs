using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public int score = 0;
    public int scoreMultiplier = 1;
    public float difficulty = 2;

    [Header("References")] 
    [SerializeField] private AllRecipesData allRecipesData;
    public RecipeData currentRecipeData;
    public HUD hud;
    private Spawner spawner;
    
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
        SetSettings();
    }

    private void SetSettings()
    {
        QualitySettings.vSyncCount = 0;  
        Application.targetFrameRate = 60;

        scoreMultiplier = 1;
        score =  0;
        
        int rng =  Random.Range(0, allRecipesData.recipes.Count);
        currentRecipeData = allRecipesData.recipes[rng];
        
        spawner = FindAnyObjectByType<Spawner>();
        hud = FindAnyObjectByType<HUD>();
        
        InitLevel();
    }

    public void CallDeferredSetSettings()
    {
        Invoke(nameof(SetSettings), 1f);
    }

    public void AddScore(int number)
    {
        score += Mathf.FloorToInt(number * (scoreMultiplier/2f));
        CalculateScoreMultiplier(currentRecipeData.maxScore);
    }

    public void RemoveScore(int number)
    {
        score -=  Mathf.FloorToInt(number * (scoreMultiplier*5));
        if(score < 0) score = 0;
        CalculateScoreMultiplier(currentRecipeData.maxScore);
    }

    public void InitLevel()
    {
        hud.UpdateHUD(currentRecipeData);
        spawner.StartSpawnerSettings();
    }

    private void CalculateScoreMultiplier(int totalScore)
    {
        scoreMultiplier = GetMultiplier(score, totalScore);

        if (scoreMultiplier != _previousMultiplier)
        {
            OnAddScoreMultiplier?.Invoke(scoreMultiplier);
            _previousMultiplier = scoreMultiplier;
        }
        hud.UpdateMultiplierText(scoreMultiplier.ToString());
    }

    public void CalculateSliderMultiplier()
    {
        int maxValue = currentRecipeData.maxScore;
        int currentScore = score;
        float value = (float)currentScore / maxValue;
        
        hud.UpdateMultiplierSlider(value);
    }
    
    private int GetMultiplier(float currentScore, float totalScore)
    {
        float[] thresholds = new float[] { 0.066f, 0.20f, 0.466f, 1.0f };

        for (int i = 0; i < thresholds.Length; i++)
        {
            if (currentScore < totalScore * thresholds[i])
            {
                return i + 1;
            }
        }

        return 5;
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
