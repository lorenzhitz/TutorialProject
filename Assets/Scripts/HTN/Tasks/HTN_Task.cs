using UnityEngine;
/// <summary>
/// Base class for all HTN tasks. Tasks are represented as ScriptableObjects so they
/// can be authored in the editor and shared between multiple NPC instances.
/// </summary>
/// <remarks>
/// ExecuteTask should avoid storing per-NPC runtime state inside the ScriptableObject.
///
/// The return value contract:
/// - Success: the task finished and any side-effects (e.g. setting an animation) are applied.
/// - Running: the task is still in progress and should be executed again in subsequent frames.
/// - Failed: the task cannot complete (missing precondition, null targets, etc.).
/// </remarks>
public abstract class HTN_Task : ScriptableObject
{
    /// <summary>
    /// Execute this task for the given NPC runtime context.
    /// </summary>
    /// <param name="npc_Properties">Runtime data and components for the NPC (Transform, Animator, etc.).</param>
    /// <returns>HTN_State indicating Success, Running or Failed.</returns>
    public abstract HTN_State ExecuteTask(NPC_Properties npc_Properties);

}
