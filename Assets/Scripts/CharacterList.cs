using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character List")]
public class CharacterList : ScriptableObject
{
    public string listName;
    public int version;
    public List<Character> characters;
}
