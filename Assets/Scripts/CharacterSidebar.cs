using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSidebar : MonoBehaviour
{
    public Image slotImage;
    public Image characterImage;
    public TMP_Text characterNameText;
    public TMP_Text suspectsLeftText;

    private Character character;
    private int suspectsLeft;

    private void Start()
    {
        slotImage.color = new Color(255, 255, 255, 255);
        characterImage.sprite = null;
        characterImage.color = new Color(255, 255, 255, 0);
        characterNameText.text = null;
    }

    public void SetCharacter(Character givenCharacter)
    {
        character = givenCharacter;

        slotImage.color = new Color(255, 255, 255, 255);
        characterImage.sprite = character.sprite;
        characterImage.color = new Color(255, 255, 255, 255);
        characterNameText.text = character.name;
    }

    public void ReloadSidebarStats(int crossedOffCount, int totalAmountSuspects)
    {
        suspectsLeft = totalAmountSuspects - crossedOffCount;
        suspectsLeftText.text = "Suspects remaining: " + suspectsLeft;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
