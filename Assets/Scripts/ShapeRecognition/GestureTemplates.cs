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

    public void Save()
    {
        string path = Application.persistentDataPath + $"/{_currJsonPath}.json";
        string potion = JsonUtility.ToJson(this);
        File.WriteAllText(path, potion);
    }

    private void Load()
    {
        string path = Application.persistentDataPath + $"/{_currJsonPath}.json";
        if (File.Exists(path))
        {
            GestureTemplates data = JsonUtility.FromJson<GestureTemplates>(File.ReadAllText(path));
            RawTemplates.Clear();
            RawTemplates.AddRange(data.RawTemplates);
            ProceedTemplates.Clear();
            ProceedTemplates.AddRange(data.ProceedTemplates);
        }
    }
}