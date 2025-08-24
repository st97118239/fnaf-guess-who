using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Category")]
public class CharacterCategory : ScriptableObject
{
    public string categoryName;
    public CategoryType type;
    public FirstGame firstGame;
    public AnimatronicClass animatronicClass;
    public SkinType skinType;
    public List<Character> characters;
}

public enum CategoryType
{
    None,
    FirstGame,
    AnimatronicClass,
    SkinType
}

public enum AnimatronicClass
{
    None,
    Classic,
    Withered,
    Toy,
    Shadow,
    Phantom,
    Springlock,
    Nightmare,
    Halloween,
    Funtime,
    Rockstar,
    Mediocre,
    Hardmore,
    Glamrock,
    Daycare,
    Endoskeleton,
    Amalgamation,
    ControlModule,
    Vendor,
    Cupcake,
    STAFF,
    Kiosk,
    WelcomeShow,
    PuppetCrew,
    MascotSuits,
    ElectronicToys,
    AI,
    UnclassifiedAnim,
    UnclassifiedBots,
    FNAFWorld,
}

public enum SkinType
{
    None,
    Holiday,
    Chocolate,
    Arcade,
    Wasteland,
    July4th,
    Heatwave,
    Circus,
    Forest,
    Winter,
    Valentine,
    Aztec,
    ScaryTales,
    WickedTides,
    Screampunk
}

public enum FirstGame
{
    None,
    FNAF1,
    FNAF2,
    FNAF3,
    FNAF4,
    WORLD,
    SL,
    PS,
    UCN,
    HW,
    SD,
    SB,
    SBR,
    HW2,
    SOTM
}