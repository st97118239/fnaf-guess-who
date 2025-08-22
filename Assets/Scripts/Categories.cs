using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Categories")]
public class Categories : ScriptableObject
{
    public List<CharacterCategory> gameCategories;
    public List<CharacterCategory> classCategories;
    public List<CharacterCategory> skinCategories;
}