using UnityEngine;
using System.IO;

public static class SaveManager
{

    public static readonly string SaveFilePath = Application.persistentDataPath + "/Saves/";
    public static readonly string TestSaveFilePath = Application.persistentDataPath + "/TestSaves/";
    
    public static void Init()
    {
        if (!Directory.Exists(SaveFilePath))
        {
            Directory.CreateDirectory(SaveFilePath);
        }
        if (!Directory.Exists(TestSaveFilePath))
        {
            Directory.CreateDirectory(TestSaveFilePath);
        }
    }

    public static void SaveGame(string data)
    {
        Debug.Log($"Saving game to {Path.Combine(SaveFilePath, "savegame.json")}");
        if (Application.isEditor)
        {
            Debug.Log("Running in Editor, saving to TestSaves folder.");
            File.WriteAllText(Path.Combine(TestSaveFilePath, "savegame.json"), data);
        }
        else
        {
            File.WriteAllText(Path.Combine(SaveFilePath, "savegame.json"), data);
        }
    }

    public static SaveObject LoadGame()
    {
        string filePath;
        if (Application.isEditor)
        {
            Debug.Log($"Loading game from {Path.Combine(TestSaveFilePath, "savegame.json")}");
            filePath = Path.Combine(TestSaveFilePath, "savegame.json");
        }
        else
        {
            Debug.Log($"Loading game from {Path.Combine(SaveFilePath, "savegame.json")}");
            filePath = Path.Combine(SaveFilePath, "savegame.json");
        }
        if (File.Exists(filePath))
        {
            string data = File.ReadAllText(filePath);
            return JsonUtility.FromJson<SaveObject>(data);
        }
        return null;
    }
}
