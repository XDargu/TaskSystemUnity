using UnityEngine;
using System.Collections;

public class Zombie : Agent
{
    PerceptionFollower mPerception;
    TaskManager mTaskManager;

    // Use this for initialization
    void Start()
    {
        mTaskManager = gameObject.GetComponent<TaskManager>();
        mPerception = gameObject.GetComponent<PerceptionFollower>();

        TaskWander taskWander = new TaskWander();
        taskWander.priority = 0;
        taskWander.duration = 10;
        mTaskManager.TriggerTask(taskWander, "Wanderer::Start");

        health = 100;

        type = AgentType.Zombie;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 1.0f)
        {
            if (collision.gameObject.name.Contains("Bullet"))
            {
                TaskReactHit taskReact = new TaskReactHit();
                taskReact.priority = 4;
                mTaskManager.TriggerTask(taskReact, "Wanderer::OnCollisionEnter");
            }
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }

    public override void OnSignal(Signal signal, string origin)
    {
        origin = origin + "::Follower";

        switch (signal)
        {
            case Signal.TargetFound:
                {
                    origin += "::" + TaskManagerDebugger.SignalNames[(int)signal];
                    // Start following
                    TaskFollow taskFollow = new TaskFollow();
                    taskFollow.priority = 1;
                    mTaskManager.TriggerTask(taskFollow, origin);
                    taskFollow.Initialize(mPerception.GetTargetAgent().transform);
                }
                break;

            case Signal.TargetInRange:
                {
                    origin += "::" + TaskManagerDebugger.SignalNames[(int)signal];
                    // Attack the target
                    TaskAttackMelee taskAttack = new TaskAttackMelee();
                    taskAttack.priority = 2;
                    mTaskManager.TriggerTask(taskAttack, origin);
                    taskAttack.Initialize(mPerception.GetTargetAgent());
                }
                break;
            case Signal.TargetDeath:
                {
                    // Do something?
                }
                break;
        }

        mTaskManager.OnSignal(signal, origin);
    }
}

