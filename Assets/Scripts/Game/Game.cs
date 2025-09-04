using System;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Game : MonoBehaviour
{
    public GameManager gameManager;
    public MainPanel mainPanel;
    public AudioManager audioManager;
    public Player player;
    public Character[] playerCharArray;
    public Character[] opponentCharArray;
    public GameObject polaroidGrid;
    public CharacterSidebar characterSidebar;
    public InfoPanel infoPanel;
    public WinPanel winPanel;
    public GameObject charSlotPrefab;
    public GameObject emptySlotPrefab;
    public List<EmptySlot> emptySlots;
    public List<CharSlot> charSlots;
    public List<CharSlot> possibleSlots;
    public List<CharSlot> crossedOff;
    public Character chosenCharacter;
    public Character accusedCharacter;
    public Animator animator;
    public bool isInfoPanelShown;
    public Vector3 polaroidSpawnPos;

    [SerializeField] private int emptySlotAmount = 96;
    [SerializeField] private Vector2 rndVoicelineTimerBase;
    [SerializeField] private Subtitles subtitles;

    private int slotAmount;
    private float rndVoicelineTimer;

    private static readonly int GameOpen = Animator.StringToHash("GameOpen");
    private static readonly int GameClose = Animator.StringToHash("GameClose");

    private void Start()
    {
        rndVoicelineTimer = Random.Range(rndVoicelineTimerBase.x, rndVoicelineTimerBase.y);
    }

    private void Update()
    {
        if (!gameManager.hasStarted || !chosenCharacter || gameManager.hasFinished) 
            return;

        if (rndVoicelineTimer > 0)
            rndVoicelineTimer -= Time.deltaTime;
        else
        {
            rndVoicelineTimer = Random.Range(rndVoicelineTimerBase.x, rndVoicelineTimerBase.y);

            Character rndChar = possibleSlots[Random.Range(0, possibleSlots.Count - 1)].character;

            if (rndChar.voicelines)
                rndChar.voicelines.Play(audioManager.voicelines, subtitles);
        }
    }

    public void LoadGame()
    {
        foreach (EmptySlot t in emptySlots)
        {
            Destroy(t.gameObject);
        }

        emptySlots.Clear();

        characterSidebar.doneNote.Disable();
        
        switch (player.playerIdx)
        {
            case 1:
            {
                if (playerCharArray.Length == 0)
                {
                    playerCharArray = new Character[gameManager.player1List.Length];

                    for (int i = 0; i < gameManager.player1List.Length; i++)
                    {
                        playerCharArray[i] = Resources.Load<Character>(gameManager.player1List[i]);
                    }

                    Debug.Log("Added p1 to playerArray");
                }

                if (opponentCharArray.Length == 0)
                {
                    opponentCharArray = new Character[gameManager.player2List.Length];

                    for (int i = 0; i < gameManager.player2List.Length; i++)
                    {
                        opponentCharArray[i] = Resources.Load<Character>(gameManager.player2List[i]);
                    }

                    Debug.Log("Added p2 to opponentArray");
                }

                break;
            }
            case 2:
            {
                if (playerCharArray.Length == 0)
                {
                    playerCharArray = new Character[gameManager.player2List.Length];

                    for (int i = 0; i < gameManager.player2List.Length; i++)
                    {
                        playerCharArray[i] = Resources.Load<Character>(gameManager.player2List[i]);
                    }

                    Debug.Log("Added p2 to playerArray");
                }

                if (opponentCharArray.Length == 0)
                {
                    opponentCharArray = new Character[gameManager.player1List.Length];

                    for (int i = 0; i < gameManager.player1List.Length; i++)
                    {
                        opponentCharArray[i] = Resources.Load<Character>(gameManager.player1List[i]);
                    }

                    Debug.Log("Added p1 to opponentArray");
                }

                break;
            }
        }

        if (gameManager.player1List.Length == 0)
        {
            Debug.Log("PLayer1List is empty.");
        }
        else if (gameManager.player2List.Length == 0)
        {
            Debug.Log("Player2List is empty.");
        }

        if (playerCharArray.Length > 0 && opponentCharArray.Length > 0)
            StartGame();
        else
            gameManager.DisconnectAll();
    }

    private void StartGame()
    {
        Invoke(nameof(SpawnEmptySlots), 1f);

        infoPanel.chooseType = 1;

        mainPanel.currentPanel = Panels.Game;
        animator.SetTrigger(GameOpen);

        characterSidebar.turnNote.ChangeText("Pick suspect");

        if (mainPanel.isReady)
            mainPanel.isReady = false;

        mainPanel.isInGame = true;
    }

    private void SpawnEmptySlots()
    {
        emptySlots = new List<EmptySlot>(emptySlotAmount);

        for (int i = 0; i < emptySlotAmount; i++)
        {
            EmptySlot slotObj = Instantiate(emptySlotPrefab, polaroidGrid.transform).GetComponent<EmptySlot>();
            emptySlots.Add(slotObj);
            slotObj.index = i;
        }

        Invoke(nameof(SpawnPolaroids), Time.deltaTime);
    }

    private void SpawnPolaroids()
    {
        if (gameManager.round == 0 && !chosenCharacter)
        {
            slotAmount = playerCharArray.Length;
            charSlots = new List<CharSlot>(slotAmount);

            for (int i = 0; i < slotAmount; i++)
            {
                CharSlot slotChar = Instantiate(charSlotPrefab, emptySlots[i].transform.position, Quaternion.identity, emptySlots[i].transform).GetComponent<CharSlot>();
                charSlots.Add(slotChar);
                slotChar.Load(playerCharArray[i], this, false);
            }

            Debug.Log("Spawned player polaroids");
        }
        else
        {
            slotAmount = opponentCharArray.Length;
            charSlots = new List<CharSlot>(slotAmount);
            possibleSlots = new List<CharSlot>(slotAmount);

            for (int i = 0; i < slotAmount; i++)
            {
                CharSlot slotChar = Instantiate(charSlotPrefab, emptySlots[i].transform.position, Quaternion.identity, emptySlots[i].transform).GetComponent<CharSlot>();
                charSlots.Add(slotChar);
                possibleSlots.Add(slotChar);
                slotChar.Load(opponentCharArray[i], this, true);
            }

            Debug.Log("Spawned opponent polaroids");
        }

        Invoke(nameof(PlayEmptyFade), 0.3f);
        UpdateSidebar();
    }

    private void PlayReverseFadeAnim()
    {
        PlayFadeAnim(true, true, false);
    }

    private void PlayFadeAnim(bool shouldReverse, bool shouldDestroyChildren, bool shouldDestroySelf)
    {
        foreach (EmptySlot t in emptySlots)
        {
            t.Play(shouldReverse, shouldDestroyChildren, shouldDestroySelf);
        }
    }

    private void PlayEmptyFade()
    {
        PlayFadeAnim(false, false, false);
    }

    public void ChooseCharacter(Character givenChar)
    {
        characterSidebar.turnNote.ChangeText("Opponent is picking");
        chosenCharacter = givenChar;
        characterSidebar.SetCharacter(givenChar);
        player.ChooseCharacter(givenChar);
        Invoke(nameof(PlayChosenCharacterAudio), 0.1f);

        Invoke(nameof(ChangePolaroids), 0.5f);
    }

    private void PlayChosenCharacterAudio()
    {
        if (chosenCharacter.voicelines)
            chosenCharacter.voicelines.Play(audioManager.voicelines, subtitles);
    }

    public void HasAccused(Character accusedChar)
    {
        accusedCharacter = accusedChar;
        Invoke(nameof(PlayAccusedCharacterAudio), 0.1f);

        foreach (CharSlot t in charSlots)
        {
            t.CanLMB(false);
        }
    }

    private void PlayAccusedCharacterAudio()
    {
        if (accusedCharacter.voicelines)
            accusedCharacter.voicelines.Play(audioManager.voicelines, subtitles);
    }

    public void ChangePolaroids()
    {
        PlayReverseFadeAnim();

        Invoke(nameof(SpawnPolaroids), 1);
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
        audioManager.soundEffects.PlayOneShot(audioManager.bellSFX);
        if (possibleSlots.Count <= 2 && gameManager.round != 1)
            characterSidebar.ChangeTurn(gameManager.turn, true);
        else
            characterSidebar.ChangeTurn(gameManager.turn, hasToAccuse);

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
        if (mainPanel.isReady)
            mainPanel.isReady = false;

        if (player)
        {
            gameManager.player = null;
            gameManager.opponent = null;

            if (player.isHost)
                NetworkManager.singleton.StopClient();
        }

        mainPanel.Load();

        Invoke(nameof(ResetGame), 1);
    }

    public void ResetGame()
    {
        player = null;
        charSlots.Clear();
        possibleSlots.Clear();
        crossedOff.Clear();
        playerCharArray = Array.Empty<Character>();
        opponentCharArray = Array.Empty<Character>();
        chosenCharacter = null;
        accusedCharacter = null;
        slotAmount = 0;
        gameManager.ResetGame();

        foreach (EmptySlot t in emptySlots)
        {
            Destroy(t.gameObject);
        }

        emptySlots.Clear();

        characterSidebar.ResetGame();
        infoPanel.ResetGame();
    }

    public void DetermineResult(bool p1Won, bool p2Won, string opponentCharDirectory, string playerSuspectDirectory, string playerCharDirectory, string opponentSuspectedDirectory)
    {
        PlayerPrefs.SetInt("Games", PlayerPrefs.GetInt("Games") + 1);

        switch (player.playerIdx)
        {
            case 1:
            {
                switch (p1Won)
                {
                    case true when !p2Won:
                        winPanel.result = "You accused the correct suspect! And your opponent did not.";
                        break;
                    case true when p2Won:
                        winPanel.result = "You accused the correct suspect! But your opponent accused your suspect.";
                        break;
                    case false when p2Won:
                        winPanel.result = "You did not accuse the correct suspect. But your opponent accused your suspect.";
                        break;
                    case false when !p2Won:
                        winPanel.result = "You did not accuse the correct suspect. And neither did your opponent.";
                        break;
                }

                if (p1Won)
                    PlayerPrefs.SetInt("Wins", PlayerPrefs.GetInt("Wins") + 1);

                mainPanel.saveManager.WinResults(p1Won, p2Won);
                break;
            }
            case 2:
            {
                switch (p2Won)
                {
                    case true when !p1Won:
                        winPanel.result = "You accused the correct suspect! And your opponent did not.";
                        break;
                    case true when p1Won:
                        winPanel.result = "You accused the correct suspect! But your opponent accused your suspect.";
                        break;
                    case false when p1Won:
                        winPanel.result = "You did not accuse the correct suspect. But your opponent accused your suspect.";
                        break;
                    case false when !p1Won:
                        winPanel.result = "You did not accuse the correct suspect. And neither did your opponent.";
                        break;
                }

                if (p2Won)
                    PlayerPrefs.SetInt("Wins", PlayerPrefs.GetInt("Wins") + 1);

                mainPanel.saveManager.WinResults(p2Won, p1Won);
                break;
            }
        }

        Character opponentChar = Resources.Load<Character>(opponentCharDirectory);
        Character playerSuspect = Resources.Load<Character>(playerSuspectDirectory);
        Character playerChar = Resources.Load<Character>(playerCharDirectory);
        Character opponentSuspected = Resources.Load<Character>(opponentSuspectedDirectory);

        string playerSuspectName = string.Empty;
        string playerCharName = string.Empty;
        string opponentSuspectedName = string.Empty;

        if (playerSuspect)
            playerSuspectName = playerSuspect.characterName;
        else
            Debug.LogWarning("Could not find " + playerSuspectDirectory);
        if (playerChar)
            playerCharName = playerChar.characterName;
        else
            Debug.LogWarning("Could not find " + playerCharDirectory);
        if (opponentSuspected)
            opponentSuspectedName = opponentSuspected.characterName;
        else
            Debug.LogWarning("Could not find " + opponentSuspectedDirectory);

        winPanel.Show(opponentChar, playerSuspectName, playerCharName, opponentSuspectedName);
    }
}
