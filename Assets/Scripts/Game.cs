using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
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

    private int slotAmount;

    private void Start()
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
