using UnityEngine;
using System.Collections;

public class TaskReactLeader : Task 
{
	Transform mTransform;
	Transform leaderTransform;
	NavigationController mNavigation;

	public float duration = 0.5f;
	float timer;

	// Use this for initialization
	public override void Construct ()
	{
		mTransform = gameObject.GetComponent<Transform>();
		mNavigation = gameObject.GetComponent<NavigationController>();
		mNavigation.MoveTo(mTransform.position);

		timer = duration;

        gameObject.GetComponent<Actions>().Jump();
	}

	public void Initialize(Transform leader)
	{
		leaderTransform = leader;
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		timer -= Time.deltaTime;
		timer = Mathf.Max(timer, 0.0f);

		if (timer <= 0)
		{
            OnTimerEnds();
		}
	}

    public void OnTimerEnds()
    {
        TaskManager mTaskManager = gameObject.GetComponent<TaskManager>();
        mTaskManager.UnregisterTask(this, "TaskReactLeader::OnTimerEnds");
    }

	public override void Destruct ()
	{
	
	}

    public override void OnSignal (Signal signal, string origin)
	{
		
	}
}
