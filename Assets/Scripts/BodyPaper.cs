using UnityEngine;
using UnityEngine.UI;

public class BodyPaper : MonoBehaviour
{
    public Image bodyImage;
    public Animator animator;
    public int index;

    public void Load(Sprite image, int idx)
    {
        index = idx;
        bodyImage.sprite = image;
    }
}
