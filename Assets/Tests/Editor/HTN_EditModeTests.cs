using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;

// EditMode tests for basic HTN components
public class HTN_EditModeTests
{
    private GameObject _go;

    [SetUp]
    public void SetUp()
    {
        _go = new GameObject("TestNPC");
    }

    [TearDown]
    public void TearDown()
    {
        if (_go != null)
            Object.DestroyImmediate(_go);
    }

    // Simple test helper Task implementation
    private class TestTask : HTN_Task
    {
        public HTN_State ReturnState = HTN_State.Success;
        public override HTN_State ExecuteTask(NPC_Properties npc_Properties)
        {
            return ReturnState;
        }
    }

    [Test]
    public void NPCProperties_CompoundIndexLifecycle()
    {
        var props = _go.AddComponent<NPC_Properties>();

        var compound = ScriptableObject.CreateInstance<HTN_CompundTask>();

        // Initially zero
        Assert.AreEqual(0, props.GetCompoundTaskIndex(compound));

        // Advance
        props.AdvanceCompoundTask(compound);
        Assert.AreEqual(1, props.GetCompoundTaskIndex(compound));

        // Reset
        props.ResetCompoundTask(compound);
        Assert.AreEqual(0, props.GetCompoundTaskIndex(compound));

        Object.DestroyImmediate(compound);
    }

    [Test]
    public void CompoundTask_SucceedsAfterAllSubtasks()
    {
        var props = _go.AddComponent<NPC_Properties>();

        var compound = ScriptableObject.CreateInstance<HTN_CompundTask>();

        var t1 = ScriptableObject.CreateInstance<TestTask>();
        t1.ReturnState = HTN_State.Success;
        var t2 = ScriptableObject.CreateInstance<TestTask>();
        t2.ReturnState = HTN_State.Success;

        // inject private field _subTasks via reflection
        var field = typeof(HTN_CompundTask).GetField("_subTasks", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(compound, new List<HTN_Task> { t1, t2 });

        // First execute -> first subtask returns Success and compound should be Running
        var res1 = compound.ExecuteTask(props);
        Assert.AreEqual(HTN_State.Running, res1);

        // Second execute -> completes last subtask and returns Success
        var res2 = compound.ExecuteTask(props);
        Assert.AreEqual(HTN_State.Success, res2);

        Object.DestroyImmediate(compound);
        Object.DestroyImmediate(t1);
        Object.DestroyImmediate(t2);
    }

    [Test]
    public void CompoundTask_PropagatesFailedFromSubtask()
    {
        var props = _go.AddComponent<NPC_Properties>();

        var compound = ScriptableObject.CreateInstance<HTN_CompundTask>();

        var t1 = ScriptableObject.CreateInstance<TestTask>();
        t1.ReturnState = HTN_State.Failed;

        var field = typeof(HTN_CompundTask).GetField("_subTasks", BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(compound, new List<HTN_Task> { t1 });

        var res = compound.ExecuteTask(props);
        Assert.AreEqual(HTN_State.Failed, res);

        // index should be reset to 0
        Assert.AreEqual(0, props.GetCompoundTaskIndex(compound));

        Object.DestroyImmediate(compound);
        Object.DestroyImmediate(t1);
    }

    [Test]
    public void Planner_RemovesSuccessAndFailedTasks()
    {
        var plannerGO = new GameObject("PlannerGO");
        var planner = plannerGO.AddComponent<HTN_Planner>();

        // NPC properties for planner
        var props = plannerGO.AddComponent<NPC_Properties>();

        // create tasks
        var successTask = ScriptableObject.CreateInstance<TestTask>();
        successTask.ReturnState = HTN_State.Success;
        var failedTask = ScriptableObject.CreateInstance<TestTask>();
        failedTask.ReturnState = HTN_State.Failed;

        // set planner._plan via reflection
        var planField = typeof(HTN_Planner).GetField("_plan", BindingFlags.NonPublic | BindingFlags.Instance);
        planField.SetValue(planner, new List<HTN_Task> { successTask });

        // call private ExecuteCurrentTask
        var exec = typeof(HTN_Planner).GetMethod("ExecuteCurrentTask", BindingFlags.NonPublic | BindingFlags.Instance);
        exec.Invoke(planner, null);

        var planAfter = (List<HTN_Task>)planField.GetValue(planner);
        Assert.AreEqual(0, planAfter.Count);

        // now test failed task removed as well
        planField.SetValue(planner, new List<HTN_Task> { failedTask });
        exec.Invoke(planner, null);
        planAfter = (List<HTN_Task>)planField.GetValue(planner);
        Assert.AreEqual(0, planAfter.Count);

        Object.DestroyImmediate(plannerGO);
        Object.DestroyImmediate(successTask);
        Object.DestroyImmediate(failedTask);
    }
}
