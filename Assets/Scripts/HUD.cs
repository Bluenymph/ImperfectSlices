using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private GameObject headerPanel;
    [SerializeField] private Image header;
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private List<Image> ingredientsImages;

    [Header("Multipler")] 
    [SerializeField] private GameObject multiplierPanel;
    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private Slider multiplierSlider;
    [SerializeField] private Image multiplierSliderFill;
    
    [Header("EndScreen")]
    [SerializeField] private GameObject endScreenPanel;
    [SerializeField] private TextMeshProUGUI endScreenText;
    

    private const string GREEN_COLOR = "#09FF00";
    private const string ORANGE_COLOR = "#EAFF00";
    private const string HEAVY_ORANGE_COLOR = "#FFB500";
    private const string ALMOST_RED_COLOR = "#FF6100";
    private const string RED_COLOR = "#FF0012";

    private void Awake()
    {
        Spawner.OnLevelEnded += EndLevelSequence;
    }

    private void OnDisable()
    {
        Spawner.OnLevelEnded -= EndLevelSequence;
    }

    public void UpdateHUD(RecipeData recipe)
    {
        headerText.text = "Receta:" + recipe.dishName;
        for (int i = 0; i < recipe.positiveIngredients.Count; i++)
        {
            ingredientsImages[i].sprite = recipe.positiveIngredients[i].IngredientIcon;
            ingredientsImages[i].gameObject.SetActive(true);
        }
    }

    public void UpdateMultiplierText(string newtext)
    {
        multiplierText.text = newtext;
        
        int number;
        if (int.TryParse(newtext, out number))
        {
            SetFillColor(number);
        }
    }
    
    public void UpdateInfoText(string newtext)
    {
        infoText.text = newtext;
        GameManager.Instance.CalculateSliderMultiplier();
    }

    public void UpdateMultiplierSlider(float newvalue)
    {
        if(newvalue > 1) return;
        multiplierSlider.value = newvalue/2f;
    }

    private void EndLevelSequence()
    {
        headerPanel.SetActive(false);
        multiplierPanel.SetActive(false);
        endScreenPanel.SetActive(true);

        endScreenText.text = "¡Felicidades! ¡Has terminado el nivel!\nPuntuación: " + GameManager.Instance.score;
    }

    public void EndScreenPressed()
    {
        SceneManager.LoadScene("CookScene");
    }

    public void SetFillColor(int multiplier)
    {
        Color newcolor = Color.green;
        
        if (multiplier == 1 && ColorUtility.TryParseHtmlString(GREEN_COLOR, out newcolor))
        {
            multiplierSliderFill.color = newcolor;
        }
        if (multiplier == 2 && ColorUtility.TryParseHtmlString(ORANGE_COLOR, out newcolor))
        {
            multiplierSliderFill.color = newcolor;
        }
        if (multiplier == 3 && ColorUtility.TryParseHtmlString(HEAVY_ORANGE_COLOR, out newcolor))
        {
            multiplierSliderFill.color = newcolor;
        }
        if (multiplier == 4 && ColorUtility.TryParseHtmlString(ALMOST_RED_COLOR, out newcolor))
        {
            multiplierSliderFill.color = newcolor;
        }
        if (multiplier == 5 && ColorUtility.TryParseHtmlString(RED_COLOR, out newcolor))
        {
            multiplierSliderFill.color = newcolor;
        }

    }
}
