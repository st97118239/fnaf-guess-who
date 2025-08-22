using UnityEngine;

public class CharactersPanel : MonoBehaviour
{
    public ListPanel listPanel;

    public void OpenPanel()
    {
        listPanel.game.animator.SetTrigger("ListCharOpen");
    }

    public void ClosePanel()
    {
        listPanel.fromCharacterPanel = true;
        listPanel.LoadPanel();
    }    
}
