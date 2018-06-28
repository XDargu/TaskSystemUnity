﻿using UnityEngine;
using System.Collections;

public class Follower : Agent 
{
    public GameObject bullet;

	TaskManager mTaskManager;
	PerceptionFollower mPerception;

	// Use this for initialization
	void Start ()
	{
		mTaskManager = gameObject.GetComponent<TaskManager>();
		mPerception = gameObject.GetComponent<PerceptionFollower>();

		TaskWander taskWander = new TaskWander();
		taskWander.priority = 0;
		mTaskManager.TriggerTask(taskWander, "Follower");

        gameObject.GetComponent<PlayerController>().SetArsenal("Rifle");
	}
	
	// Update is called once per frame
	void Update ()
	{
	}

    public void OnSignal (Signal signal, string origin)
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
				taskFollow.Initialize(mPerception.GetTargetTransform());

				// React to the finding
				TaskReactLeader taskReact = new TaskReactLeader();
				taskReact.priority = 3;
                mTaskManager.TriggerTask(taskReact, origin);
				taskReact.Initialize(mPerception.GetTargetTransform());
			}
			break;

		case Signal.TargetInRange:
			{
                origin += "::" + TaskManagerDebugger.SignalNames[(int)signal];
                // Shoot at target
                TaskShoot taskShoot = new TaskShoot();
                taskShoot.priority = 2;
                mTaskManager.TriggerTask(taskShoot, origin);
                taskShoot.Initialize(mPerception.GetTargetTransform(), bullet);
			}
			break;
        case Signal.TargetDeath:
            {
                gameObject.GetComponent<NavigationController>().speed = 1;
            }
            break;
		}

        mTaskManager.OnSignal(signal, origin);
	}
}