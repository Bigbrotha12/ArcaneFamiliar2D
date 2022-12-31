using System.Collections.Generic;

[System.Serializable]
public class UserData
{
    public string name;
    public int level;
    public int playTime;
    public int money;
    public string[] items;
    public string[] recipes;
    public string[] spells;
    public Dictionary<string, string> locations;
    public int[] progress; 
}