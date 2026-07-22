using UnityEngine;
using TMPro;

/// <summary>
/// Simple UI updater that shows the current time of day from TimeOfDayManager.
/// Attach to a UI Text object and optionally set a prefix.
/// </summary>
public class TimeOfDayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private string prefix = "Zeit: ";

    private string _lastText = string.Empty;

    private void Reset()
    {
        // Try to auto-assign when added to a TextMeshProUGUI GameObject
        timeText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (timeText == null)
            Debug.LogWarning("TimeOfDayUI: TextMeshProUGUI-Komponente nicht zugewiesen.");

        UpdateTextImmediate();
    }

    private void Update()
    {
        if (TimeOfDayManager.Instance == null || timeText == null)
            return;

        string current = prefix + TimeOfDayManager.Instance.GetTimeString();
        if (current != _lastText)
        {
            _lastText = current;
            timeText.text = current;
        }
    }

    private void UpdateTextImmediate()
    {
        if (TimeOfDayManager.Instance == null || timeText == null)
            return;
        _lastText = prefix + TimeOfDayManager.Instance.GetTimeString();
        timeText.text = _lastText;
    }
}
