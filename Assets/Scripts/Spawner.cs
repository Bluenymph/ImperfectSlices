using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mesas : MonoBehaviour
{
    [SerializeField] private float timeLoopSpawn = 2f;
    [SerializeField] private float speedSlide = 0.5f;
    [SerializeField] private float destroyIngredientThreshold = -8f;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool canSpawn = true;
    [SerializeField] private Transform parent;
    
    private List<GameObject> _objectPool = new List<GameObject>();
    private List<GameObject> _activeIngredients = new List<GameObject>();
    private int _poolIndex = 0;

    private void OnEnable()
    {
        GameManager.OnAddScoreMultiplier += ApplyScoreMultiplier;
    }

    private void OnDisable()
    {
        GameManager.OnAddScoreMultiplier -= ApplyScoreMultiplier;
    }

    private void Start()
    {
        if(GameManager.Instance == null) return;
        RecipeData recipeData = GameManager.Instance.currentRecipeData;
        
        //Ingredientes positivos
        for (int i = 0; i < recipeData.positiveIngredients.Count * 5 * GameManager.Instance.difficulty; i++)
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
            GameManager.ApplyTagRecursive(instance, "Negative");
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
            if (_activeIngredients.Count >= _objectPool.Count) yield break;
            
            _objectPool[_poolIndex].SetActive(true);
            _objectPool[_poolIndex].transform.position = spawnPoint.position;
            _activeIngredients.Add(_objectPool[_poolIndex]);
            _poolIndex++;
            yield return new WaitForSeconds(timeLoopSpawn);
        }
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

    private void ApplyScoreMultiplier(int scoreMultiplier)
    {
        switch (scoreMultiplier)
        {
            case 1:
                timeLoopSpawn = 2f;
                speedSlide = 0.5f;
                break;
            case 2:
                timeLoopSpawn = 1.7f;
                speedSlide = 0.7f;
                break;
            case 3:
                timeLoopSpawn = 1.7f;
                speedSlide = 0.7f;
                break;
            case 4:
                timeLoopSpawn = 1.2f;
                speedSlide = 1f;
                break;
            case 5:
                timeLoopSpawn = 1.2f;
                speedSlide = 1f;
                break;
        }
    }
}
