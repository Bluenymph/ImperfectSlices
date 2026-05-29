using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllRecipes", menuName = "Kitchen/AllRecipes")]
public class AllRecipesData : ScriptableObject
{
    public List<RecipeData> recipes = new List<RecipeData>();
}

