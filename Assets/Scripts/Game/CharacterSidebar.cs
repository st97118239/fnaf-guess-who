using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSidebar : MonoBehaviour
{
    public Game gameScript;
    public CharSlot chosenSlot;

    public Polaroid slotPolaroid;
    public Note suspectsLeftNote;
    public Note turnNote;
    public Note doneNote;
    public Note leaveNote;

    [SerializeField] private Animator polaroidAnimator;


    private int suspectsLeft;

    private void Start()
    {
        slotPolaroid.polImage.color = Color.clear;
        slotPolaroid.characterImage.sprite = null;
        slotPolaroid.characterImage.color = Color.clear;
        slotPolaroid.characterText.text = null;
    }

    public void SetCharacter(Character givenCharacter)
    {
        chosenSlot.Load(givenCharacter, gameScript, false);
        slotPolaroid.polImage.color = Color.white;
        slotPolaroid.characterImage.color = Color.white;

        slotPolaroid.polImage.gameObject.SetActive(true);
        polaroidAnimator.SetTrigger("ChooseCharacter");
    }

    public void ReloadSidebarStats(int crossedOffCount, int totalAmountSuspects)
    {
        suspectsLeft = totalAmountSuspects - crossedOffCount;
        suspectsLeftNote.ChangeText("Suspects remaining: " + suspectsLeft);
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
            turnNote.ChangeText("Your turn");
            if (!hasToAccuse)
            {
                doneNote.Enable();
            }
        }
        else
        {
            turnNote.ChangeText("Opponent's turn");
            doneNote.Disable();
        }
    }

    public void ResetGame()
    {
        slotPolaroid.polImage.color = Color.clear;
        slotPolaroid.characterImage.sprite = null;
        slotPolaroid.characterImage.color = Color.clear;
        slotPolaroid.ChangeText(null);
        leaveNote.Disable();
        polaroidAnimator.SetTrigger("PolaroidRemove");

        suspectsLeft = 0;
    }
}
