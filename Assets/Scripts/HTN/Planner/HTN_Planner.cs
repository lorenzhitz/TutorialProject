using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPC_Properties))]
public class HTN_Planner : MonoBehaviour
{
    private NPC_Properties _npcProperties;

    private List<HTN_Task> _plan = new List<HTN_Task>();
    private HTN_Task _currentTask;

    private TimeOfDayManager _timeOfDay;

    /// <summary>
    /// Simple runtime HTN planner that executes tasks from a linear plan list. The planner
    /// invokes the current task each frame and reacts to the task's HTN_State. This implementation
    /// is intentionally simple: there is no backtracking and no automatic method-selection.
    /// </summary>
    /// <remarks>
    /// Current behavior:
    /// - Success: remove the completed task and move to the next one.
    /// - Running: keep executing the current task in subsequent frames.
    /// - Failed: log a warning and remove the failing task.
    /// </remarks>



    private void Awake()
    {
        _npcProperties = GetComponent<NPC_Properties>();

        // initialize timetable and time manager
        _timeOfDay = TimeOfDayManager.Instance;

        // Initialen Plan aus dem Timetable des NPCs laden (falls vorhanden)
        var timetable = _npcProperties?.GetTimetableInstance();
        if (timetable != null)
        {
            int slot = _timeOfDay.GetCurrentSlotIndex();
            var tasks = timetable.GetTasksForSlot(slot);
            _plan = new List<HTN_Task>(tasks ?? new List<HTN_Task>());
        }

        // subscribe to slot changes to update plan
        if (_timeOfDay != null)
            _timeOfDay.OnSlotChanged += OnSlotChanged;
    }

    private void OnDestroy()
    {
        if (_timeOfDay != null)
            _timeOfDay.OnSlotChanged -= OnSlotChanged;
    }

    private void OnSlotChanged(int slot)
    {
        var timetable = _npcProperties?.GetTimetableInstance();
        if (timetable == null)
            return;

        var tasks = timetable.GetTasksForSlot(slot);
        _plan = new List<HTN_Task>(tasks ?? new List<HTN_Task>());
        _currentTask = null;
    }


    private void Update()
    {
        ExecuteCurrentTask();
    }


    private void ExecuteCurrentTask()
    {
        // Kein aktueller Task -> n�chsten holen
        if (_currentTask == null)
        {
            if (_plan.Count == 0)
            {
                return;
            }

            _currentTask = _plan[0];

        }


        HTN_State state = _currentTask.ExecuteTask(_npcProperties);


        switch (state)
        {
            case HTN_State.Running:
                break;


            case HTN_State.Success:
                _plan.RemoveAt(0);
                _currentTask = null;
                break;

            case HTN_State.Failed:
                Debug.LogWarning($"{name}: HTN task '" + _currentTask.name + "' failed. Removing from plan.");
                // Remove Failed Task and continue with the next Task
                _plan.RemoveAt(0);
                _currentTask = null;
                break;

        }
    }



}
