using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character List")]
public class CharacterList : ScriptableObject
{
    public string listName;
    public List<Character> characters;
}
