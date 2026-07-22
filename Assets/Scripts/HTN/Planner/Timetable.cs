using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimetableSlot
{
    [HideInInspector]
    [SerializeField]
    private string _name;

    public string Name => _name;

    public List<HTN_Task> Tasks = new List<HTN_Task>();

    public TimetableSlot() { }

    public TimetableSlot(string name)
    {
        _name = name;
    }

    internal void SetName(string name)
    {
        _name = name;
    }
}

[CreateAssetMenu(menuName = "HTN/Timetable")]
public class Timetable : ScriptableObject
{
    // 48 half-hour slots for a 24-hour day
    [SerializeField]
    private List<TimetableSlot> _slots = new List<TimetableSlot>();

    public List<TimetableSlot> Slots => _slots;

    private void OnEnable()
    {
        EnsureSlots();
    }

    private void OnValidate()
    {
        EnsureSlots();
    }

    private void EnsureSlots()
    {
        if (_slots == null)
            _slots = new List<TimetableSlot>();

        // ensure exactly 48 slots exist
        while (_slots.Count < 48)
        {
            _slots.Add(new TimetableSlot(SlotName(_slots.Count)));
        }

        while (_slots.Count > 48)
        {
            _slots.RemoveAt(_slots.Count - 1);
        }

        // enforce deterministic slot names (not editable in Inspector)
        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i].SetName(SlotName(i));
        }
    }

    private string SlotName(int index)
    {
        int hour = index / 2;
        int minute = (index % 2) == 0 ? 0 : 30;
        return string.Format("{0:00}:{1:00}", hour, minute);
    }

    /// <summary>
    /// Get tasks assigned to the given half-hour slot index (0..47).
    /// </summary>
    public List<HTN_Task> GetTasksForSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _slots.Count)
            return new List<HTN_Task>();

        return _slots[slotIndex].Tasks;
    }
}
