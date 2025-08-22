using System.Collections.Generic;
using UnityEngine;

public class CharactersPanel : MonoBehaviour
{
    public ListPanel listPanel;

    [SerializeField] private Categories categories;
    [SerializeField] private GameObject grid;

    [SerializeField] private GameObject categoryNotePrefab;
    [SerializeField] private GameObject emptySlotPrefab;
    [SerializeField] private GameObject characterPolaroidPrefab;

    [SerializeField] private Note backNote;
    [SerializeField] private int emptySlotAmount = 96;
    [SerializeField] private CategoryType defaultCategory = CategoryType.AnimatronicClass;

    private List<CategoryNote> categoryNotes;
    private List<EmptySlot> emptySlots;
    private List<CharacterPolaroid> characterPolaroids;
    private int categoryAmount;
    private int characterAmount;

    private CategoryType categoryType;
    private List<CharacterCategory> category;
    private CharacterCategory loadedCategory;

    private bool hasLoaded;

    public void OpenPanel()
    {
        listPanel.game.animator.SetTrigger("ListCharOpen");

        RefreshMenuVar();

        if (!hasLoaded)
            OpenFirst();
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
        if (listPanel.hasListOpen)
            listPanel.menu = 1;
        else if (!listPanel.hasListOpen)
            listPanel.menu = 0;

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

        categoryType = defaultCategory;
        Invoke(nameof(LoadCategories), 1.2f);
    }

    public void LoadCategoriesFade()
    {
        backNote.Disable();

        PlayFadeAnim(true, true, false);

        characterPolaroids.Clear();
        loadedCategory = null;

        Invoke(nameof(LoadCategories), 0.7f);
    }

    public void LoadCategories()
    {
        backNote.Disable();

        if (loadedCategory)
        {
            for (int i = 0; i < characterPolaroids.Count; i++)
            {
                Destroy(characterPolaroids[i].gameObject);
            }

            characterPolaroids.Clear();
            loadedCategory = null;
        }

        RefreshMenuVar();

        if (categoryType == CategoryType.None)
            return;
        else if (categoryType == CategoryType.FirstGame)
        {
            loadedCategory = null;
            category = categories.gameCategories;
        }
        else if (categoryType == CategoryType.AnimatronicClass)
        {
            loadedCategory = null;
            category = categories.classCategories;
        }
        else if (categoryType == CategoryType.SkinType)
        {
            loadedCategory = null;
            category = categories.skinCategories;
        }

        categoryAmount = category.Count;

        categoryNotes = new List<CategoryNote>(categoryAmount);

        for (int i = 0; i < categoryAmount; i++)
        {
            GameObject slotObj = Instantiate(categoryNotePrefab, emptySlots[i].transform.position, Quaternion.identity, emptySlots[i].transform);
            categoryNotes.Add(slotObj.GetComponent<CategoryNote>());
            categoryNotes[i].LoadCategory(category[i], this);
        }

        PlayFadeAnim(false, false, false);
    }

    public void OpenCategoryFade(CharacterCategory categoryToOpen)
    {
        PlayFadeAnim(true, true, false);

        categoryNotes.Clear();

        loadedCategory = categoryToOpen;
        Invoke(nameof(OpenCategory), 0.7f);
    }

    private void OpenCategory()
    {
        characterAmount = loadedCategory.characters.Count;

        characterPolaroids = new List<CharacterPolaroid>(characterAmount);

        for (int i = 0; i < characterAmount; i++)
        {
            GameObject slotObj = Instantiate(characterPolaroidPrefab, emptySlots[i].transform.position, Quaternion.identity, emptySlots[i].transform);
            characterPolaroids.Add(slotObj.GetComponent<CharacterPolaroid>());
            characterPolaroids[i].Load(loadedCategory.characters[i], this, i);
        }

        RefreshMenuVar();

        backNote.Enable();

        PlayFadeAnim(false, false, false);
    }

    public void BackNote()
    {
        if (listPanel.hasListOpen)
            listPanel.menu = 1;
        else if (!listPanel.hasListOpen)
            listPanel.menu = 0;

        LoadCategoriesFade();
    }

    private void PlayFadeAnim(bool shouldReverse, bool shouldDestroyChildren, bool shouldDestroySelf)
    {
        for (int i = 0; i < emptySlots.Count; i++)
        {
            emptySlots[i].Play(shouldReverse, shouldDestroyChildren, shouldDestroySelf);
        }
    }
}
