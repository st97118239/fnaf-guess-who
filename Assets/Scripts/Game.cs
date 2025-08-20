using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameManager gameManager;
    public MainPanel mainPanel;
    public Player player;
    public CharacterList characterList;
    public GameObject polaroidGrid;
    public CharacterSidebar characterSidebar;
    public InfoPanel infoPanel;
    public WinPanel winPanel;
    public GameObject charSlotPrefab;
    public GameObject emptySlotPrefab;
    public List<Transform> emptySlots;
    public List<CharSlot> charSlots;
    public List<CharSlot> crossedOff;
    public Character chosenCharacter;
    public Animator animator;
    public bool isInfoPanelShown;
    public Vector3 polaroidSpawnPos;

    private int slotAmount;

    public void StartGame()
    {
        Invoke(nameof(SpawnEmptySlots), 1f);

        infoPanel.chooseType = 1;

        animator.SetTrigger("GameOpen");
    }

    private void SpawnEmptySlots()
    {
        slotAmount = characterList.characters.Count;

        charSlots = new List<CharSlot>(slotAmount);
        emptySlots = new List<Transform>(slotAmount);

        for (int i = 0; i < slotAmount; i++)
        {
            GameObject slotObj = Instantiate(emptySlotPrefab, polaroidGrid.transform);
            emptySlots.Add(slotObj.transform);
            slotObj.GetComponent<EmptySlot>().index = i;
        }

        Invoke(nameof(SpawnPolaroids), Time.deltaTime);
    }

    private void SpawnPolaroids()
    {
        for (int i = 0; i < slotAmount; i++)
        {
            GameObject slotObj = Instantiate(charSlotPrefab, emptySlots[i].position, Quaternion.identity, emptySlots[i]);
            charSlots.Add(slotObj.GetComponent<CharSlot>());
            charSlots[i].Load(characterList.characters[i], this);
        }

        UpdateSidebar();
    }

    public void ChooseCharacter(Character givenChar)
    {
        chosenCharacter = givenChar;
        characterSidebar.SetCharacter(chosenCharacter);
        player.ChooseCharacter(givenChar);
    }

    public void UpdateSidebar()
    {
        characterSidebar.ReloadSidebarStats(crossedOff.Count, charSlots.Count);
    }

    public void ShowInfoPanel(Character character)
    {
        infoPanel.Show(character);
    }

    public void StartRound(bool hasToAccuse)
    {
        
    }

    public void Done()
    {
        player.FinishedTurn();
    }

    public void StopGame()
    {
        characterSidebar.leaveNote.Enable();
    }

    public void Leave()
    {
        animator.SetTrigger("GameClose");
        player.Disconnect();
        Invoke(nameof(ResetGame), 1);
    }

    public void ResetGame()
    {
        gameManager = null;
        player = null;
        charSlots.Clear();
        crossedOff.Clear();
        chosenCharacter = null;
        slotAmount = 0;

        for (int i = 0; i < emptySlots.Count; i++)
        {
            Destroy(emptySlots[i].gameObject);
        }

        emptySlots.Clear();

        characterSidebar.ResetGame();
        infoPanel.ResetGame();
    }

    public void DetermineResult(bool p1Won, bool p2Won, string opponentCharDirectory, string playerSuspect, string playerCharDirectory, string opponentSuspected)
    {
        if (player.playerIdx == 1)
        {
            if (p1Won && !p2Won)
                winPanel.result = "You accused the correct suspect! And your opponent did not.";
            else if (p1Won && p2Won)
                winPanel.result = "You accused the correct suspect! But your opponent accused your suspect.";
            else if (!p1Won && p2Won)
                winPanel.result = "You did not accuse the correct suspect. But your opponent accused your suspect.";
            else if (!p1Won && !p2Won)
                winPanel.result = "You did not accuse the correct suspect. And neither did your opponent.";
        }
        else if (player.playerIdx == 2)
        {
            if (p2Won && !p1Won)
                winPanel.result = "You accused the correct suspect! And your opponent did not.";
            else if (p2Won && p1Won)
                winPanel.result = "You accused the correct suspect! But your opponent accused your suspect.";
            else if (!p2Won && p1Won)
                winPanel.result = "You did not accuse the correct suspect. But your opponent accused your suspect.";
            else if (!p2Won && !p1Won)
                winPanel.result = "You did not accuse the correct suspect. And neither did your opponent.";
        }

        winPanel.Show(Resources.Load<Character>(opponentCharDirectory), Resources.Load<Character>(playerSuspect).characterName, Resources.Load<Character>(playerCharDirectory).characterName, Resources.Load<Character>(opponentSuspected).characterName);
    }
}
