using UnityEngine;

[CreateAssetMenu(fileName = "KillStatsSO", menuName = "Scriptable Objects/KillStatsSO")]
public class KillStatsSO : ScriptableObject
{
    [System.Serializable]
    public class SaveSlot
    {
        public int kills;
    }

    [Tooltip("Три слота сейвов")]
    public SaveSlot[] slots = new SaveSlot[3]  // 0,1,2
    {
        new SaveSlot(), new SaveSlot(), new SaveSlot()
    };

    [Tooltip("Какой слот сейчас активен (0-2)")]
    public int selectedSlot = 0;

    /// <summary>Увеличивает kills в активном слоте.</summary>
    public void AddKill()
    {
        if (selectedSlot >= 0 && selectedSlot < slots.Length)
            slots[selectedSlot].kills++;
    }

    public int GetKills(int slot) => slots[slot].kills;
}