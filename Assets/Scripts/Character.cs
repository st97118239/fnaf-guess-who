using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character")]
public class Character : ScriptableObject
{
    public string characterName;
    public string pronouns;
    public string yearMade;
    public string yearBorn;
    public string familyMembers;
    public string owner;
    public string occupation;
    public string employment;
    public string affiliation;
    public string aliases;
    public string type;
    public string model;
    public string height;
    public string weight;
    public string color;
    public string eyeColor;
    public string firstAppearance;
    public List<Sprite> polaroidSprite;
    public List<Sprite> fullBodySprite;
    public List<AudioClip> voicelines;

    public string directory;
}