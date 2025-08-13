using UnityEngine;

[CreateAssetMenu(menuName = "Character")]
public class Character : ScriptableObject
{
    public string characterName;
    public string year;
    public string affiliation;
    public string firstAppearance;
    public string type;
    public string model;
    public string color;
    public string eyeColor;
    public string pronouns;
    public string aliases;
    public Sprite sprite;
    public Sprite fullBodySprite;
}