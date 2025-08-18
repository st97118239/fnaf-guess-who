using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameManager gameManager;
    public CharacterList characterList;
    public GameObject polaroidGrid;
    public CharacterSidebar characterSidebar;
    public InfoPanel infoPanel;
    public GameObject charSlotPrefab;
    public GameObject emptySlotPrefab;
    public List<Transform> emptySlots;
    public List<CharSlot> charSlots;
    public List<CharSlot> crossedOff;
    public Character chosenCharacter;
    public Animator animator;
    public bool isInfoPanelShown;
    public Vector3 polaroidSpawnPos;

    private bool hasLoaded;
    private int slotAmount;

    public void StartGame()
    {
        if (!hasLoaded)
        {
            slotAmount = characterList.characters.Count;

            charSlots = new List<CharSlot>(slotAmount);
            emptySlots = new List<Transform>(slotAmount);

            Invoke(nameof(SpawnEmptySlots), 1f);

            hasLoaded = true;

            gameManager.hasStarted = true;
        }

        animator.SetTrigger("GameOpen");
    }

    private void SpawnEmptySlots()
    {
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
        gameManager.player.chosenCharacter = chosenCharacter.directory;
    }

    public void UpdateSidebar()
    {
        characterSidebar.ReloadSidebarStats(crossedOff.Count, charSlots.Count);
    }

    public void ShowInfoPanel(Character character)
    {
        infoPanel.Show(character);
    }
}
