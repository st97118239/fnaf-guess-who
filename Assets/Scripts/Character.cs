using UnityEngine;

[CreateAssetMenu(menuName = "Character")]
public class Character : ScriptableObject
{
    public CharacterName charName;
    public CharacterType type;
    public CharacterGame game;
    public Sprite sprite;
}