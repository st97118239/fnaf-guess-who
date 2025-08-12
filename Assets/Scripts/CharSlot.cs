using System;
using UnityEngine;
using UnityEngine.UI;

public class CharSlot : MonoBehaviour
{
    public Image slotImage;
    public Character character;
    public bool isChosen;
    public bool isHidden;

    private void Start()
    {
        slotImage.sprite = Resources.Load<Sprite>("FNAF1/" + character.ToString());
    }
}
