using System;
using UnityEngine;

/// <summary>
/// Manages in-game time of day. A full in-game day is mapped to <see cref="DayLengthSeconds"/>
/// real seconds. Time is reported in in-game seconds (0..86400).
/// </summary>
public class TimeOfDayManager : MonoBehaviour
{
    private static TimeOfDayManager _instance;
    public static TimeOfDayManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TimeOfDayManager>();
                if (_instance == null)
                {
                    var go = new GameObject("TimeOfDayManager");
                    _instance = go.AddComponent<TimeOfDayManager>();
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// Real seconds for a full in-game day. Default = 12 minutes = 720 seconds.
    /// </summary>
    public float DayLengthSeconds = 720f;

    // in-game seconds per real second (scale factor)
    private float _scale = 1f;

    // current in-game time in seconds (0 .. 86400)
    private double _currentInGameSeconds = 0.0;

    // last slot index to detect changes
    private int _lastSlot = -1;

    /// <summary>
    /// Fired when the current half-hour slot index (0..47) changes.
    /// Parameter is the new slot index.
    /// </summary>
    public event Action<int> OnSlotChanged;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;

        // scale maps real seconds to 86400 in-game seconds
        _scale = 86400f / Mathf.Max(1f, DayLengthSeconds);

        // initialize from current time
        _currentInGameSeconds = 0.0;
        _lastSlot = GetCurrentSlotIndex();
    }

    private void Update()
    {
        // advance in-game time
        _currentInGameSeconds += Time.deltaTime * _scale;
        // wrap day
        if (_currentInGameSeconds >= 86400.0)
            _currentInGameSeconds %= 86400.0;

        int slot = GetCurrentSlotIndex();
        if (slot != _lastSlot)
        {
            _lastSlot = slot;
            OnSlotChanged?.Invoke(slot);
        }
    }

    /// <summary>
    /// Get current in-game time in seconds (0..86400).
    /// </summary>
    public double GetCurrentInGameSeconds() => _currentInGameSeconds;

    /// <summary>
    /// Returns formatted time string (HH:MM) representing current in-game time.
    /// </summary>
    public string GetTimeString()
    {
        int totalSeconds = (int)_currentInGameSeconds;
        int hours = (totalSeconds / 3600) % 24;
        int minutes = (totalSeconds / 60) % 60;
        return string.Format("{0:00}:{1:00}", hours, minutes);
    }

    /// <summary>
    /// Returns the current half-hour slot index (0..47).
    /// </summary>
    public int GetCurrentSlotIndex()
    {
        int totalSeconds = (int)_currentInGameSeconds;
        int hours = (totalSeconds / 3600) % 24;
        int minutes = (totalSeconds / 60) % 60;
        int slot = hours * 2 + (minutes >= 30 ? 1 : 0);
        return slot;
    }
}
