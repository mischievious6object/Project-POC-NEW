using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public static T Load<T>(string fileName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string json = string.Empty;
        if (File.Exists(filePath))
        {
            json = File.ReadAllText(filePath);
        }
        return JsonUtility.FromJson<T>(json);
    }

    public static void Save<T>(string fileName, T data)
    {
        string filePath = Path.Combine(Application.persistentDataPath, fileName);
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
    }

}
