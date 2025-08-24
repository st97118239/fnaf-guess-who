using TMPro;
using UnityEngine;

public class DevManager : MonoBehaviour
{
    public bool unlockAllCharacters;

    [SerializeField] private DevClipboard devClipboard;

    [SerializeField] private float holdTimerBase;
    [SerializeField] private string devPassword;

    [SerializeField] private TMP_InputField passworldField;

    private float holdTimer;

    private void Start()
    {
        holdTimer = holdTimerBase;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Backspace) && !devClipboard.isShown)
        {
            if (holdTimer > 0)
                holdTimer -= Time.deltaTime;
            else
            {
                devClipboard.OpenClipboard();
                holdTimer = holdTimerBase;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Backspace))
            holdTimer = holdTimerBase;
    }

    public void CheckPassword()
    {
        if (passworldField.text == devPassword)
            devClipboard.NextPage();
    }
}
