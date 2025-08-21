using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSidebar : MonoBehaviour
{
    public Game gameScript;
    public Image slotImage;
    public Image characterImage;
    public CharSlot chosenSlot;
    public TMP_Text characterNameText;
    public TMP_Text suspectsLeftText;
    public TMP_Text turnText;

    public Note doneNote;
    public Note leaveNote;

    private Character character;
    private Animator polaroidAnimator;
    private int suspectsLeft;

    private Color transparent = new(255, 255, 255, 0);
    private Color opaque = new(255, 255, 255, 255);

    private void Start()
    {
        polaroidAnimator = slotImage.GetComponent<Animator>();

        slotImage.color = opaque;
        characterImage.sprite = null;
        characterImage.color = transparent;
        characterNameText.text = null;
    }

    public void SetCharacter(Character givenCharacter)
    {
        character = givenCharacter;

        chosenSlot.Load(givenCharacter, gameScript);
        slotImage.color = opaque;
        characterImage.color = opaque;

        slotImage.gameObject.SetActive(true);
        polaroidAnimator.SetTrigger("ChooseCharacter");
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

    public void ChangeTurn(int turn, bool hasToAccuse)
    {
        if (gameScript.infoPanel.chooseType != 2)
            gameScript.infoPanel.chooseType = 2;

        if (turn == gameScript.player.playerIdx)
        {
            turnText.text = "Your turn";
            if (!hasToAccuse)
            {
                doneNote.Enable();
            }
        }
        else
        {
            turnText.text = "Opponent's turn";
            doneNote.Disable();
        }
    }

    public void ResetGame()
    {
        slotImage.color = new Color(255, 255, 255, 255);
        characterImage.sprite = null;
        characterImage.color = new Color(255, 255, 255, 0);
        characterNameText.text = null;
        leaveNote.Disable();
        polaroidAnimator.SetTrigger("PolaroidRemove");

        character = null;
        suspectsLeft = 0;
    }
}
