using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Ingredient", menuName = "Kitchen/Ingredient")]
public class IngredientData : ScriptableObject
{
    public string IngredientName;
    public Sprite IngredientIcon;
    public GameObject prefabASpawnear; 
    public GameObject prefabSlice; 
}
