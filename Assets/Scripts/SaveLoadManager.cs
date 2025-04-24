using System.IO;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    [SerializeField] private ColorSettings colorSettings;
    [SerializeField] private KillStatsSO killStats;

    private string _colorPath;
    private string _killsPath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _colorPath = Path.Combine(Application.persistentDataPath, "colorSettings.json");
        _killsPath = Path.Combine(Application.persistentDataPath, "killStats.json");

        LoadAll();
    }

    private void OnApplicationQuit() => SaveAll();

    public void SaveAll()
    {
        SaveColor();
        SaveKills();
    }

    public void LoadAll()
    {
        LoadColor();
        LoadKills();
    }

    [System.Serializable]
    private class ColorSettingsData
    {
        public float r, g, b, a;
    }

    private void SaveColor()
    {
        if (colorSettings == null) return;

        var data = new ColorSettingsData
        {
            r = colorSettings.currentColor.r,
            g = colorSettings.currentColor.g,
            b = colorSettings.currentColor.b,
            a = colorSettings.currentColor.a,
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_colorPath, json);
    }

    private void LoadColor()
    {
        if (colorSettings == null) return;
        if (!File.Exists(_colorPath)) return;

        string json = File.ReadAllText(_colorPath);
        var data = JsonUtility.FromJson<ColorSettingsData>(json);
        colorSettings.currentColor = new Color(data.r, data.g, data.b, data.a);
    }

    [System.Serializable]
    private class KillStatsData
    {
        public int selectedSlot;
        public int[] kills;
    }

    private void SaveKills()
    {
        if (killStats == null) return;

        var data = new KillStatsData
        {
            selectedSlot = killStats.selectedSlot,
            kills = new int[killStats.slots.Length]
        };

        for (int i = 0; i < killStats.slots.Length; i++)
            data.kills[i] = killStats.slots[i].kills;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(_killsPath, json);
    }

    private void LoadKills()
    {
        if (killStats == null) return;
        if (!File.Exists(_killsPath)) return;

        string json = File.ReadAllText(_killsPath);
        var data = JsonUtility.FromJson<KillStatsData>(json);

        killStats.selectedSlot = Mathf.Clamp(data.selectedSlot, 0, killStats.slots.Length - 1);

        int len = Mathf.Min(data.kills.Length, killStats.slots.Length);
        for (int i = 0; i < len; i++)
            killStats.slots[i].kills = data.kills[i];
    }
}
