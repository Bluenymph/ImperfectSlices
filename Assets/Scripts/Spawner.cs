using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] private float timeLoopSpawn = 2f;
    [SerializeField] private float speedSlide = 0.5f;
    [SerializeField] private float destroyIngredientThreshold = -8f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool canSpawn = true;
    [SerializeField] private Transform parent;
    [SerializeField] private GameObject speedParticles;
    
    private List<GameObject> _objectPool = new List<GameObject>();
    private List<GameObject> _activeIngredients = new List<GameObject>();
    private int _poolIndex = 0;
    
    public delegate void LevelEnded();
    public static event LevelEnded OnLevelEnded;

    private void OnEnable()
    {
        GameManager.OnAddScoreMultiplier += ApplyScoreMultiplier;
    }

    private void OnDisable()
    {
        GameManager.OnAddScoreMultiplier -= ApplyScoreMultiplier;
    }

    public void StartSpawnerSettings()
    {
        _objectPool.Clear();
        _activeIngredients.Clear();
        _poolIndex = 0;
        timeLoopSpawn = 2f;
        speedSlide = 0.5f;
        canSpawn = true;
        
        if(GameManager.Instance == null) return;
        RecipeData recipeData = GameManager.Instance.currentRecipeData;
        
        //Ingredientes positivos
        for (int i = 0; i < recipeData.positiveIngredients.Count * 2 * GameManager.Instance.difficulty; i++)
        {
            int indiceAleatorio = Random.Range(0, recipeData.positiveIngredients.Count);
            GameObject instance = Instantiate(recipeData.positiveIngredients[indiceAleatorio].prefabASpawnear, spawnPoint.position - Vector3.down*6, spawnPoint.rotation);
            instance.transform.SetParent(parent);
            instance.SetActive(false);
            _objectPool.Add(instance);
        }
        
        //Ingredientes negativos
        for (int i = 0; i < recipeData.negativeIngredients.Count * 5; i++)
        {
            int indiceAleatorio = Random.Range(0, recipeData.negativeIngredients.Count);
            GameObject instance = Instantiate(recipeData.negativeIngredients[indiceAleatorio].prefabASpawnear, spawnPoint.position - Vector3.down*6, spawnPoint.rotation);
            if(!instance.CompareTag("CuttingBoard"))GameManager.ApplyTagRecursive(instance, "Negative");
            instance.transform.SetParent(parent);
            instance.SetActive(false);
            
            _objectPool.Add(instance);
        }
        
        GameManager.ShuffleList(_objectPool);
        StartSpawning();
    }
    
    public void StartSpawning()
    {
        StartCoroutine(SpawnIngredient());
    }

    IEnumerator SpawnIngredient()
    {
        while (canSpawn) 
        {
            if (_activeIngredients.Count >= _objectPool.Count) break;
            
            _objectPool[_poolIndex].SetActive(true);
            _objectPool[_poolIndex].transform.position = spawnPoint.position;
            _activeIngredients.Add(_objectPool[_poolIndex]);
            _poolIndex++;
            yield return new WaitForSeconds(timeLoopSpawn);
        }
        Invoke(nameof(InvokeLevelEnded), 4f);
        yield return null;
    }
    
    private void Update()
    {
        
        if (_activeIngredients.Count > 0)
        {
            for (int i = _activeIngredients.Count - 1; i >= 0; i--)
            {
                GameObject ingredient = _activeIngredients[i];

                if (!ingredient)
                {
                    _activeIngredients.RemoveAt(i);
                    continue;
                }
                
                ingredient.gameObject.transform.Translate(Vector3.back * (speedSlide * Time.deltaTime));
                if (ingredient.gameObject.transform.position.z < destroyIngredientThreshold)
                {
                    ingredient.SetActive(false);
                }
            }
        }
    }

    private void InvokeLevelEnded()
    {
        OnLevelEnded?.Invoke();
    }
    
    private void ApplyScoreMultiplier(int scoreMultiplier)
    {
        switch (scoreMultiplier)
        {
            case 1:
                timeLoopSpawn = 2f;
                speedSlide = 0.5f;
                speedParticles.SetActive(false);
                break;
            case 2:
                timeLoopSpawn = 1.7f;
                speedSlide = 0.7f;
                speedParticles.SetActive(false);
                break;
            case 3:
                timeLoopSpawn = 1.7f;
                speedSlide = 0.7f;
                speedParticles.SetActive(true);
                break;
            case 4:
                timeLoopSpawn = 1.2f;
                speedSlide = 1f;
                speedParticles.SetActive(true);
                break;
            case 5:
                timeLoopSpawn = 1.2f;
                speedSlide = 1f;
                speedParticles.SetActive(true);
                break;
        }
    }
}
