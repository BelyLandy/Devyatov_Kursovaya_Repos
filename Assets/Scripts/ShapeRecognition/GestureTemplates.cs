using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class GestureTemplates
{
    private string _currJsonPath = "GestureTemplates";
    
    private static GestureTemplates instance;

    public static GestureTemplates Get()
    {
        if (instance == null)
        {
            instance = new GestureTemplates();
            instance.Load();
        }

        return instance;
    }


    public List<ShapeRecognizerManager.GestureTemplate> RawTemplates = new List<ShapeRecognizerManager.GestureTemplate>();
    public List<ShapeRecognizerManager.GestureTemplate> ProceedTemplates = new List<ShapeRecognizerManager.GestureTemplate>();

    public List<ShapeRecognizerManager.GestureTemplate> GetTemplates()
    {
        return ProceedTemplates;
    }

    public void RemoveAtIndex(int indexToRemove)
    {
        ProceedTemplates.RemoveAt(indexToRemove);
        RawTemplates.RemoveAt(indexToRemove);
    }

    public ShapeRecognizerManager.GestureTemplate[] GetRawTemplatesByName(string name)
    {
        return RawTemplates.Where(template => template.Name == name).ToArray();
    }

    /*public void Save()
    {
        string path = Application.persistentDataPath + $"/{_currJsonPath}.json";
        string potion = JsonUtility.ToJson(this);
        File.WriteAllText(path, potion);
    }*/

    private void Load()
    {
        TextAsset jsonAsset = Resources.Load<TextAsset>(_currJsonPath);
        if (jsonAsset == null)
        {
            Debug.LogError($"[GestureTemplates] Не найден файл {_currJsonPath}.json в Resources!");
            return;
        }
        
        GestureTemplates data = JsonUtility.FromJson<GestureTemplates>(jsonAsset.text);
        if (data == null)
        {
            Debug.LogError("[GestureTemplates] Не удалось распарсить JSON из GestureTemplates.json");
            return;
        }
        
        RawTemplates.Clear();
        RawTemplates.AddRange(data.RawTemplates);

        ProceedTemplates.Clear();
        ProceedTemplates.AddRange(data.ProceedTemplates);

        Debug.Log($"[GestureTemplates] Загружено {RawTemplates.Count} шаблонов из Resources/{_currJsonPath}.json");
    }
}