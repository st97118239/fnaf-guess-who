using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CharactersPanel : MonoBehaviour
{
    public ListPanel listPanel;
    public SettingsMenu settingsMenu;
    
    public int categoryIdx = -1;
    public float fadeAnimDelay = 0.7f;

    public CharacterCategory loadedCategory;
    public CharacterCategory newCharacters;
    public bool hasLoaded;

    [SerializeField] private CharacterList allCharacters;
    [SerializeField] private Categories categories;
    [SerializeField] private GameObject grid;

    [SerializeField] private GameObject categoryNotePrefab;
    [SerializeField] private GameObject emptySlotPrefab;
    [SerializeField] private GameObject characterPolaroidPrefab;

    [SerializeField] private Note backNote;
    [SerializeField] private Note categoryNote;
    [SerializeField] private int emptySlotAmount = 96;

    private List<CategoryNote> categoryNotes;
    private List<EmptySlot> emptySlots;
    private List<CharacterPolaroid> characterPolaroids;
    private int categoryAmount;
    private int characterAmount;

    private CategoryType categoryType;
    private List<CharacterCategory> category;

    private static readonly int ListCharOpen = Animator.StringToHash("ListCharOpen");

    private void Update()
    {
        if (listPanel.mainPanel.currentPanel != Panels.CharacterPanel) 
            return;

        if (listPanel.menu == 2 && !listPanel.isInfoPanelShown && Input.GetKeyDown(KeyCode.Escape))
            BackNote();
    }

    public void OpenPanel()
    {
        listPanel.mainPanel.currentPanel = Panels.CharacterPanel;

        listPanel.game.animator.SetTrigger(ListCharOpen);

        RefreshMenuVar();

        if (!hasLoaded)
            OpenFirst();
        else if (listPanel.menu == 2)
            RecheckPolaroids();
    }

    public void RefreshMenuVar()
    {
        if (loadedCategory)
            listPanel.menu = 2;
        else if (listPanel.hasListOpen)
            listPanel.menu = 1;
        else if (!listPanel.hasListOpen)
            listPanel.menu = 0;
    }

    public void ClosePanel()
    {
        listPanel.mainPanel.currentPanel = Panels.ListPanel;

        listPanel.menu = listPanel.hasListOpen switch
        {
            true => 1,
            false => 0
        };

        listPanel.fromCharacterPanel = true;
        listPanel.LoadPanel();
    }  
    
    private void OpenFirst()
    {
        hasLoaded = true;

        emptySlots = new List<EmptySlot>(emptySlotAmount);

        for (int i = 0; i < emptySlotAmount; i++)
        {
            GameObject slotObj = Instantiate(emptySlotPrefab, grid.transform);
            emptySlots.Add(slotObj.GetComponent<EmptySlot>());
            emptySlots[i].index = i;
        }

        if (categoryIdx != -1)
            categoryIdx = settingsMenu.settings.categoryIdx - 1;

        CategoryButton(1);
    }

    public void CategoryButton(int amountToAdd)
    {
        listPanel.mainPanel.audioManager.soundEffects.PlayOneShot(listPanel.mainPanel.audioManager.noteSFX);

        PlayFadeAnim(true, true, false);

        categoryIdx += amountToAdd;

        switch (categoryIdx)
        {
            case > 2:
                categoryIdx = 0;
                break;
            case < 0:
                categoryIdx = 2;
                break;
        }

        switch (categoryIdx)
        {
            case 0:
                categoryType = CategoryType.AnimatronicClass;
                categoryNote.ChangeText("Class");
                break;
            case 1:
                categoryType = CategoryType.FirstGame;
                categoryNote.ChangeText("Game");
                break;
            case 2:
                categoryType = CategoryType.SkinType;
                categoryNote.ChangeText("Skin");
                break;
        }

        settingsMenu.Save();

        Invoke(nameof(LoadCategories), fadeAnimDelay);
    }

    public void NewNote()
    {
        OpenCategoryFade(newCharacters);
    }

    public void LoadCategoriesFade()
    {
        backNote.Disable();

        PlayFadeAnim(true, true, false);

        Invoke(nameof(LoadCategories), fadeAnimDelay);
    }

    public void LoadCategories()
    {
        backNote.Disable();

        if (loadedCategory)
        {
            foreach (CharacterPolaroid t in characterPolaroids.Where(t => t))
            {
                Destroy(t.gameObject);
            }

            characterPolaroids.Clear();
            loadedCategory = null;
        }

        RefreshMenuVar();

        switch (categoryType)
        {
            default:
            case CategoryType.None:
                return;
            case CategoryType.FirstGame:
                loadedCategory = null;
                category = categories.gameCategories;
                break;
            case CategoryType.AnimatronicClass:
                loadedCategory = null;
                category = categories.classCategories;
                break;
            case CategoryType.SkinType:
                loadedCategory = null;
                category = categories.skinCategories;
                break;
        }

        categoryAmount = category.Count;

        categoryNotes = new List<CategoryNote>(categoryAmount);

        for (int i = 0; i < categoryAmount; i++)
        {
            GameObject slotObj = Instantiate(categoryNotePrefab, emptySlots[i].transform.position, Quaternion.identity, emptySlots[i].transform);
            categoryNotes.Add(slotObj.GetComponent<CategoryNote>());
            categoryNotes[i].LoadCategory(category[i], this);
        }

        categoryNote.Enable();

        PlayFadeAnim(false, false, false);
    }

    public void RefreshPolaroids()
    {
        PlayFadeAnim(true, true, false);

        Invoke(nameof(OpenCategory), fadeAnimDelay);
    }

    public void RecheckPolaroids()
    {
        foreach (CharacterPolaroid t in characterPolaroids)
        {
            t.CheckIfCanAdd();
        }
    }

    public void OpenCategoryFade(CharacterCategory categoryToOpen)
    {
        categoryNote.Disable();

        PlayFadeAnim(true, true, false);

        loadedCategory = categoryToOpen;
        Invoke(nameof(OpenCategory), fadeAnimDelay);
    }

    private void OpenCategory()
    {
        foreach (CategoryNote t in categoryNotes.Where(t => t))
        {
            Destroy(t.gameObject);
        }

        categoryNotes.Clear();

        characterAmount = loadedCategory.characters.Count;

        characterPolaroids = new List<CharacterPolaroid>(characterAmount);

        for (int i = 0; i < characterAmount; i++)
        {
            CharacterPolaroid charPol = Instantiate(characterPolaroidPrefab, emptySlots[i].transform.position, Quaternion.identity, emptySlots[i].transform).GetComponent<CharacterPolaroid>();
            characterPolaroids.Add(charPol);
            charPol.Load(loadedCategory.characters[i], this, i);
        }

        RefreshMenuVar();

        backNote.Enable();

        PlayFadeAnim(false, false, false);
    }

    public void BackNote()
    {
        switch (listPanel.hasListOpen)
        {
            case true:
                listPanel.menu = 1;
                break;
            case false:
                listPanel.menu = 0;
                break;
        }

        LoadCategoriesFade();
    }

    private void PlayFadeAnim(bool shouldReverse, bool shouldDestroyChildren, bool shouldDestroySelf)
    {
        foreach (EmptySlot t in emptySlots)
        {
            t.Play(shouldReverse, shouldDestroyChildren, shouldDestroySelf);
        }
    }

    public void CreateListAllChars()
    {
#if UNITY_EDITOR
        CharacterList list = ScriptableObject.CreateInstance<CharacterList>();

        list.characters = new();

        foreach (CharacterCategory t in categories.classCategories)
        {
            foreach (Character t1 in t.characters)
            {
                int isDuplicate = list.characters.FindIndex(l => l.directory == t1.directory);

                if (isDuplicate == -1)
                    list.characters.Add(t1);
                else
                    Debug.Log("Skipped " + t1.characterName);
            }
        }

        foreach (CharacterCategory t in categories.gameCategories)
        {
            if (t.firstGame == FirstGame.SD)
                continue;

            foreach (Character t1 in t.characters)
            {
                int isDuplicate = list.characters.FindIndex(l => l.directory == t1.directory);

                if (isDuplicate == -1)
                    list.characters.Add(t1);
                else
                    Debug.Log("Skipped " + t1.characterName);
            }
        }

        foreach (CharacterCategory t in categories.skinCategories)
        {
            foreach (Character t1 in t.characters)
            {
                int isDuplicate = list.characters.FindIndex(l => l.directory == t1.directory);

                if (isDuplicate == -1)
                    list.characters.Add(t1);
                else
                    Debug.Log("Skipped " + t1.characterName);
            }
        }

        list.listName = "All Characters";
        list.version = PlayerPrefs.GetInt("Version");

        AssetDatabase.CreateAsset(list, "Assets/ScriptableObjects/CharacterLists/All Characters.asset");
#endif
    }
}
