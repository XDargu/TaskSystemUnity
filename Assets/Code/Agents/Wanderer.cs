using UnityEngine;
using System.Collections;

public class Wanderer : Agent {

    TaskManager mTaskManager;

	// Use this for initialization
	void Start ()
	{
		mTaskManager = gameObject.GetComponent<TaskManager>();

		TaskWander taskWander = new TaskWander();
		taskWander.priority = 0;
		mTaskManager.TriggerTask(taskWander, "Wanderer::Start");

        health = 100;
        type = AgentType.Wanderer;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 1.0f)
        {
            if (collision.gameObject.name.Contains("Bullet"))
            {
                OnSignal(Signal.Hit, "TaskAttackMelee::Attack");
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
            case Signal.Hit:
                {
                    TaskReactHit taskReact = new TaskReactHit();
                    taskReact.priority = 4;
                    mTaskManager.TriggerTask(taskReact, origin);
                }
                break;
        }

        mTaskManager.OnSignal(signal, origin);
    }
}
