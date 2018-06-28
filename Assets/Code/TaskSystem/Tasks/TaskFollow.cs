using UnityEngine;
using System.Collections;

public class TaskFollow : Task 
{
	Transform mTransform;
	Transform leaderTransform;
	NavigationController mNavigation;

	// Use this for initialization
	public override void Construct ()
	{
		leaderTransform = null;
		mTransform = gameObject.GetComponent<Transform>();
		mNavigation = gameObject.GetComponent<NavigationController>();
		mNavigation.speed = 2.0f;
	}

	public void Initialize(Transform leader)
	{
		leaderTransform = leader;
	}

	Vector3 GetFollowPosition ()
	{
		if (leaderTransform == null)
		{
			return mTransform.position;
		}

		Vector3 toLeader = leaderTransform.position - mTransform.position;
		toLeader.Normalize();

        return leaderTransform.position - leaderTransform.forward * 1.5f;
	}
	
	// Update is called once per frame
	public override void Update ()
	{
        mNavigation.MoveTo(GetFollowPosition());
	}

	public override void Destruct ()
	{
	
	}

    public override void OnSignal(Signal signal, string origin)
	{
        origin = origin + "::TaskFollow::" + TaskManagerDebugger.SignalNames[(int)signal];

		switch (signal)
		{
		case Signal.TargetLost:
			{
				TaskManager mTaskManager = gameObject.GetComponent<TaskManager>();
				mTaskManager.UnregisterTask(this, origin);
			}
			break;
            case Signal.TargetDeath:
            {
                TaskManager mTaskManager = gameObject.GetComponent<TaskManager>();
                mTaskManager.UnregisterTask(this, origin);
            }
            break;
		}
	}
}
