using TMPro;
using UnityEngine;

public class DevManager : MonoBehaviour
{
    public bool unlockAllCharacters;
    public bool isUnlocked;

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
        if (Input.GetKey(KeyCode.Backslash) && !devClipboard.isShown)
        {
            if (holdTimer > 0)
                holdTimer -= Time.deltaTime;
            else
            {
                devClipboard.OpenClipboard();
                holdTimer = holdTimerBase;
            }
        }
        else if (Input.GetKeyUp(KeyCode.Backslash))
            holdTimer = holdTimerBase;
    }

    public void CheckPassword()
    {
        if (passworldField.text == devPassword)
        {
            devClipboard.NextPage();
            isUnlocked = true;
            holdTimerBase = 0.05f;
            holdTimer = holdTimerBase;
        }
    }
}
