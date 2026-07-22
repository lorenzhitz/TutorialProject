using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPC_Properties : MonoBehaviour
{
    [SerializeField] private float _walkingSpeed;
    private Animator _animController;

    // Timetable für diesen NPC. Kann als geteiltes Asset oder als Laufzeit-Kopie verwendet werden.
    [SerializeField] private Timetable _timetable;
    [SerializeField] private bool _instantiateTimetableAtRuntime = false;
    private Timetable _runtimeTimetable;

    // tracks progress for compound tasks without mutating the ScriptableObject
    // Key: the CompoundTask asset instance, Value: current subtask index for this NPC
    private Dictionary<HTN_CompundTask, int> _compoundTaskIndices = new();

    private void Awake()
    {
        if (_animController == null)
            _animController = GetComponent<Animator>();
    }

    /// <summary>
    /// NPC walking speed (units per second). Kept as a serialized field so it can be
    /// tuned per NPC in the editor.
    /// </summary>
    public float WalkingSpeed { get => _walkingSpeed; private set => _walkingSpeed = value; }

    /// <summary>
    /// Reference to the NPC's Animator component used by animation tasks.
    /// </summary>
    public Animator AnimatorController { get => _animController; private set => _animController = value; }

    /// <summary>
    /// Get the current subtask index for a given compound task for this NPC instance.
    /// If no entry exists yet the index is initialized to 0.
    /// </summary>
    public int GetCompoundTaskIndex(HTN_CompundTask task)
    {
        if (!_compoundTaskIndices.TryGetValue(task, out var idx))
        {
            idx = 0;
            _compoundTaskIndices[task] = idx;
        }

        return idx;
    }

    /// <summary>
    /// Advance the stored subtask index for the given compound task.
    /// </summary>
    public void AdvanceCompoundTask(HTN_CompundTask task)
    {
        if (_compoundTaskIndices.ContainsKey(task))
            _compoundTaskIndices[task]++;
        else
            _compoundTaskIndices[task] = 1;
    }

    /// <summary>
    /// Reset the stored subtask index for the given compound task to 0.
    /// </summary>
    public void ResetCompoundTask(HTN_CompundTask task)
    {
        _compoundTaskIndices[task] = 0;
    }

    /// <summary>
    /// Return the Timetable instance this NPC should use. If runtime instantiation
    /// is enabled a copy of the assigned asset is created on first access.
    /// </summary>
    public Timetable GetTimetableInstance()
    {
        if (!_instantiateTimetableAtRuntime)
            return _timetable;

        if (_runtimeTimetable == null && _timetable != null)
            _runtimeTimetable = Instantiate(_timetable);

        return _runtimeTimetable ?? _timetable;
    }
}
