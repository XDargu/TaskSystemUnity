using UnityEngine;
using System.Collections;

public class TaskWander : Task {

	Transform mTransform;
	NavigationController mNavigation;

	public float duration = 2.0f;
	float timer;

	// Use this for initialization
	public override void Construct ()
	{
		mTransform = gameObject.GetComponent<Transform>();
		mNavigation = gameObject.GetComponent<NavigationController>();

		timer = duration;

		mNavigation.MoveTo(GetWanderingPosition());
	}

	Vector3 GetWanderingPosition()
	{
		// Random point around
		Vector3 randomPoint = Random.insideUnitSphere;
		randomPoint.y = 0;
		randomPoint.Normalize();

		const float distance = 10.0f;
		randomPoint *= distance;

		return randomPoint + mTransform.position;
	}
	
	// Update is called once per frame
	public override void Update ()
	{
		timer -= Time.deltaTime;

		if (timer <= 0)
		{
			mNavigation.MoveTo(GetWanderingPosition());
			timer = duration;
		}
	}

	public override void Destruct ()
	{
	
	}
}
