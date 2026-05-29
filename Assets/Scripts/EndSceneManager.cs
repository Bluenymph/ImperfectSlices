using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class EndSceneManager : MonoBehaviour
{
    [SerializeField] private Animator animatorCanvas;
    [SerializeField] private Animator falseKnifeAnimator;
    
    [Header("End Panel")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI endText;
    
    [Header("Slices")]
    [SerializeField] private Transform slicesParent;
    
    private static readonly int Direction = Animator.StringToHash("direction");
    private const string CHANGE_ANIMATION = "ChangeScene";
    private const string SWING_ANIMATION = "FalseKnifeSwing";
    
    private List<GameObject> _slicesPrefab = new List<GameObject>();
    private List<GameObject> slices = new List<GameObject>();


    private void Start()
    {
        PlayChangeSceneAnimation(false);

        List<IngredientData> ingredients = GameManager.Instance.currentRecipeData.positiveIngredients;
        
        for (int i =0; i < ingredients.Count; i++)
        {
            _slicesPrefab.Add(ingredients[i].prefabSlice);
        }
        
        for (int i = 1; i < 10; i++)
        {
            Transform position = slicesParent.GetChild(i).gameObject.transform;
            int rng = Random.Range(0, _slicesPrefab.Count);
            GameObject slice = Instantiate(_slicesPrefab[rng], position.position, position.rotation);
            Rigidbody sliceRb = slice.GetComponentInChildren<Rigidbody>();
            sliceRb.constraints = RigidbodyConstraints.FreezeAll;
            slice.transform.SetParent(position);
            slices.Add(slice);
        }
        Invoke(nameof(LetSlicesFall),1f);
    }

    public void PlayChangeSceneAnimation(bool reverse)
    {
        if (!animatorCanvas) return;
        
        if (reverse == false)
        {
            animatorCanvas.SetFloat(Direction, 1f);
            animatorCanvas.Play(CHANGE_ANIMATION,-1,0);
        }
        else
        {
            Debug.Log("Esto tendria que salir");
            animatorCanvas.SetFloat(Direction, -1f);
            animatorCanvas.Play(CHANGE_ANIMATION,-1,1f);
        }
    }

    private void LetSlicesFall()
    {
        for (int i = 0; i < slices.Count; i++)
        {
            Rigidbody sliceRb = slices[i].GetComponentInChildren<Rigidbody>();
            sliceRb.constraints = RigidbodyConstraints.None;
        }
        falseKnifeAnimator.Play(SWING_ANIMATION);
        Invoke(nameof(ShowEndPanel),0.5f);
    }

    private void ShowEndPanel()
    {
        endPanel.SetActive(true);
        endText.SetText("¡Receta " + GameManager.Instance.currentRecipeData.dishName + " terminada!");
    }

    public void NextLevelPressed()
    {
        SceneManager.LoadScene("GameLevel");
        GameManager.Instance.CallDeferredSetSettings();
    }
}
