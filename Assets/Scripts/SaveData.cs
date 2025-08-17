using System.Collections.Generic;

[System.Serializable]
public class ListData
{
    public string name;
    public List<string> characters = new();
}

[System.Serializable]
public class SaveData
{
    public List<ListData> lists = new();
}

