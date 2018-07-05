using UnityEngine;
using System.Collections;

public class TaskReactLeader : Task 
{
	Transform mTransform;
	Transform leaderTransform;
	NavigationController mNavigation;

    bool jumping;

    // Use this for initialization
    public override void Construct ()
	{
		mTransform = gameObject.GetComponent<Transform>();
		mNavigation = gameObject.GetComponent<NavigationController>();
		mNavigation.MoveTo(mTransform.position);

        // Throw jump request
        gameObject.GetComponent<Actions>().Jump();
        jumping = false;

    }

	public void Initialize(Transform leader)
	{
		leaderTransform = leader;
	}
	
	// Update is called once per frame
	public override void Update ()
	{
        bool currentlyJumping = gameObject.GetComponent<Actions>().IsJumping();

        // It was jumping and not it´s not, it finished
        if (jumping && !currentlyJumping)
        {
            OnAnimationEnds();
        }

        // Jumping just started
        if (currentlyJumping && !jumping)
        {
            jumping = true;
        }
	}

    public void OnAnimationEnds()
    {
        TaskManager mTaskManager = gameObject.GetComponent<TaskManager>();
        mTaskManager.UnregisterTask(this, "TaskReactLeader::OnAnimationEnds");
    }

    public override void Destruct ()
	{
	
	}

    public override void OnSignal (Signal signal, string origin)
	{
		
	}
}
