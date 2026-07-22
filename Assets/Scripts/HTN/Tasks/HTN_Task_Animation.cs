using UnityEngine;

[CreateAssetMenu(menuName = "HTN/Tasks/Play Animation")]
public class HTN_Task_Animation : HTN_Task
{
    [SerializeField] private string _stateName;
    [SerializeField] private float _transitionTime;

    /// <summary>
    /// Play an animation state on the NPC's Animator. This task is typically a stateless
    /// command: it sets the desired animation on the Animator and immediately returns Success.
    /// The animation itself remains active on the Animator until another task changes it.
    /// </summary>
    /// <remarks>
    /// Use Running only if the task must wait for the animation to finish. Current design
    /// intentionally treats animation tasks as immediate commands so movement or other tasks
    /// can run next while the animation continues.
    /// </remarks>
    public override HTN_State ExecuteTask(NPC_Properties npc_Properties)
    {
        var animController = npc_Properties.AnimatorController;

        if (animController == null)
        {
            Debug.LogWarning($"{npc_Properties.name}: AnimatorController is null for HTN_Task_Animation ({_stateName}).");
            return HTN_State.Failed;
        }

        var hash = Animator.StringToHash($"Base Layer.{_stateName}");

        if (animController.HasState(0, hash))
        {
            // CrossFade the state and return Success immediately. The Animator keeps playing
            // the state until another task or system changes it.
            animController.CrossFade(hash, _transitionTime);
            return HTN_State.Success;
        }

        Debug.LogWarning($"{npc_Properties.name}: Animator state '{_stateName}' not found on controller.");
        return HTN_State.Failed;
    }
}
