using UnityEngine;
using TMPro;
public class KillCounterUI : MonoBehaviour
{
    [Header("GUI")]
    [SerializeField] private TextMeshProUGUI textField;

    [Header("Data")]
    [Tooltip("Имя KillStatsSO.asset в папке Resources")]
    [SerializeField] private string killStatsPath = "KillStatsSO";

    private KillStatsSO stats;

    private void Awake()
    {
        stats = Resources.Load<KillStatsSO>(killStatsPath);
        UpdateKillText();
    }

    public void UpdateKillText()
    {
        if (textField == null || stats == null) return;

        int kills = stats.GetKills(stats.selectedSlot);
        textField.text = $"Текущее кол-во убийств\n{kills}";
    }
}