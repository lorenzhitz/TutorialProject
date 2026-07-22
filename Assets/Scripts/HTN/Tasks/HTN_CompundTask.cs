using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "HTN/Tasks/CompoundTask")]
public class HTN_CompundTask : HTN_Task
{
    [SerializeField] private List<HTN_Task> _subTasks;

    /// <summary>
    /// Compound task that sequentially executes a list of subtasks. Progress for this
    /// compound task is tracked per-NPC inside NPC_Properties to keep the ScriptableObject
    /// asset stateless.
    /// </summary>
    /// <remarks>
    /// Behavior notes:
    /// - If the subtask list is empty the compound task immediately returns Success.
    /// - The current progress index is retrieved from NPC_Properties. If the index exceeds
    ///   the current subtask list (for example because the asset was edited), the index is reset
    ///   and the compound task completes.
    /// - If any subtask returns Failed, the compound task resets its progress for this NPC and
    ///   propagates Failed so the planner can decide how to react.
    /// - Do not store runtime state in this ScriptableObject; multiple NPCs share the same asset.
    /// </remarks>
    public override HTN_State ExecuteTask(NPC_Properties nPC_Properties)
    {
        if (_subTasks == null || _subTasks.Count == 0)
            return HTN_State.Success;

        int index = nPC_Properties.GetCompoundTaskIndex(this);

        // clamp index in case the task list changed
        if (index >= _subTasks.Count)
        {
            nPC_Properties.ResetCompoundTask(this);
            return HTN_State.Success;
        }

        var currentTask = _subTasks[index].ExecuteTask(nPC_Properties);

        if (currentTask == HTN_State.Failed)
        {
            // Reset progress for this compound task and propagate failure
            nPC_Properties.ResetCompoundTask(this);
            return HTN_State.Failed;
        }

        if (currentTask == HTN_State.Success)
        {
            nPC_Properties.AdvanceCompoundTask(this);

            // if we just finished the last subtask, compound task is done
            if (nPC_Properties.GetCompoundTaskIndex(this) >= _subTasks.Count)
            {
                nPC_Properties.ResetCompoundTask(this);
                return HTN_State.Success;
            }
        }

        return HTN_State.Running;
    }
}
