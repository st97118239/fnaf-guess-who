using UnityEngine;
using UnityEngine.EventSystems;

public class ListPolaroid : MonoBehaviour, IPointerClickHandler
{
    public Character character;

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

    public void Load(string givenCharacterDirectory, ListPanel givenPanel)
    {
        character = Resources.Load<Character>(givenCharacterDirectory);
        listPanel = givenPanel;

        polaroid.Load(character);
    }

    private void LMB()
    {

    }

    private void RMB()
    {
        if (!listPanel.isInfoPanelShown)
        {
            listPanel.infoPanel.charSlot = this;
            listPanel.ShowInfoPanel(character);
        }
    }
}
