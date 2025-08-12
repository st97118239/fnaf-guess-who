using UnityEngine;

[CreateAssetMenu(menuName = "Character")]
public class Character : ScriptableObject
{
    public CharacterName charName;
    public CharacterGame game;
    public Sprite sprite;
}