using System;
using UnityEngine;

[CreateAssetMenu(menuName = "HTN/Tasks/Move Towards")]
public class MoveTowardsTask : HTN_Task
{
    [SerializeField] private LocationType targetLocation;
    [SerializeField] private float stoppingDistance = 0.2f;


    public override HTN_State ExecuteTask(NPC_Properties npc)
    {
        Transform target = WorldState.Instance.GetLocation(targetLocation);

        // If the world state does not provide the location, this task cannot run and
        // should signal Failed to the planner. The planner can then decide how to react.
        if (target == null)
        {
            Debug.LogWarning($"{npc.name}: MoveTowardsTask target '{targetLocation}' not found.");
            return HTN_State.Failed;
        }


        npc.transform.position = Vector3.MoveTowards(
            npc.transform.position,
            target.position,
            npc.WalkingSpeed * Time.deltaTime
        );


        if (Vector3.Distance(
            npc.transform.position,
            target.position) <= stoppingDistance)
        {
            return HTN_State.Success;
        }

        return HTN_State.Running;
    }
}
