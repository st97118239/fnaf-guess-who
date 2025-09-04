using UnityEngine;
using UnityEngine.EventSystems;

public class ListPolaroid : MonoBehaviour, IPointerClickHandler
{
    public Character character;
    public int index;
    public bool characterCanAdd;

    public ListPanel listPanel;

    [SerializeField] private Polaroid polaroid;
    [SerializeField] private bool canRMB = true;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!listPanel.openedList.builtIn)
                LMB();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (canRMB)
                RMB();
        }
    }

    public void Load(string givenCharacterDirectory, ListPanel givenPanel, int givenIndex)
    {
        character = Resources.Load<Character>(givenCharacterDirectory);
        listPanel = givenPanel;
        index = givenIndex;

        polaroid.Load(character);
    }

    private void LMB()
    {
        if (listPanel.menu == 1)
            listPanel.characterClipboard.OpenClipboard(character.directory, index);
    }

    private void RMB()
    {
        if (listPanel.isInfoPanelShown) 
            return;

        listPanel.infoPanel.polaroidSlot = this;
        listPanel.ShowInfoPanel(character);
    }
}
