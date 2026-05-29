using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Recipe", menuName = "Kitchen/Recipe")]
public class RecipeData : ScriptableObject
{
    public string dishName;
    public List<IngredientData> positiveIngredients;
    public List<IngredientData> negativeIngredients;
    public int maxScore = 0;
}
