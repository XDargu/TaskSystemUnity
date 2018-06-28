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
}
