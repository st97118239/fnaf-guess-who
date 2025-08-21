using UnityEngine;
using UnityEngine.EventSystems;

public class ListPolaroid : MonoBehaviour, IPointerClickHandler
{
    public Character character;
    public int index;

    [SerializeField] private Polaroid polaroid;

    private ListPanel listPanel;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            LMB();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
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

    }

    private void RMB()
    {
        if (!listPanel.isInfoPanelShown)
        {
            listPanel.infoPanel.polaroidSlot = this;
            listPanel.ShowInfoPanel(character);
        }
    }
}
