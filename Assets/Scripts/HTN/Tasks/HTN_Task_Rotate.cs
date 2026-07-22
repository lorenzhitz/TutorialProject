using UnityEngine;

[CreateAssetMenu(menuName = "HTN/Tasks/Rotate Towards")]
public class RotateTowardsTask : HTN_Task
{
    [SerializeField] private LocationType _targetLocation;
    [SerializeField] private float _rotationSpeed = 360f; 
    [SerializeField] private float _stoppingAngle = 1f; 

    public override HTN_State ExecuteTask(NPC_Properties npc)
    {
        Transform target = WorldState.Instance.GetLocation(_targetLocation);

        // If there is no target provided by the WorldState, the rotation cannot proceed.
        // Return Failed and let the planner decide on next steps (replan / remove / fallback).
        if (target == null)
        {
            Debug.LogWarning($"{npc.name}: RotateTowardsTask target '{_targetLocation}' not found.");
            return HTN_State.Failed;
        }

        var npcTransform = npc.transform;

        Vector3 direction = (target.position - npcTransform.position);
        direction.y = 0f; 

        if (direction.sqrMagnitude <= 0.0001f)
        {
            return HTN_State.Success;
        }

        Quaternion desired = Quaternion.LookRotation(direction.normalized);

        npcTransform.rotation = Quaternion.RotateTowards(
            npcTransform.rotation,
            desired,
            _rotationSpeed * Time.deltaTime
        );

        float angle = Quaternion.Angle(npcTransform.rotation, desired);

        if (angle <= _stoppingAngle)
            return HTN_State.Success;

        return HTN_State.Running;
    }
}
