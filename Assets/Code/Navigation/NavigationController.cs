using UnityEngine;
using System.Collections;

public class NavigationController : MonoBehaviour {

	public float speed;

	Transform mTransform;
	Actions animationActions;

	Vector3 destination;
    Vector3 velocity;

	void Awake()
	{
		mTransform = gameObject.GetComponent<Transform>();
		destination = mTransform.position;

        velocity = Vector3.zero;

        animationActions = gameObject.GetComponent<Actions>();
	}
	
	// Use this for initialization
	void Start ()
	{
		
	}

    public void LookAt(Vector3 point)
    {
        point.y = mTransform.position.y;
        mTransform.LookAt(point);
    }
	
	// Update is called once per frame
	void Update()
    {
        Vector3 targetVelocity = destination - mTransform.position;

        if (targetVelocity.magnitude > 0)
        {
            targetVelocity.Normalize();
            targetVelocity.y = 0;
            velocity = Vector3.Lerp(velocity, targetVelocity, 0.05f);
        }
        else
        {
            velocity = targetVelocity;
        }

        //velocity = destination - mTransform.position;
        velocity.y = 0;

        float toTarget = velocity.magnitude / Time.deltaTime;

        velocity.Normalize();

        // dist = min(speed*delta, toTarget)
        // dist = x * delta
        // x = dist / delta

        float actualVelocity = Mathf.Min(speed, toTarget);
        float distance = actualVelocity * Time.deltaTime;

        mTransform.position += velocity * distance;

        if (velocity.magnitude > 0)
        {
            animationActions.SetIsMoving();
            mTransform.LookAt(mTransform.position + velocity);
        }

        animationActions.SetSpeed(actualVelocity * 0.35f);
	}

	public void MoveTo (Vector3 moveToDestination)
	{
		destination = moveToDestination;
	}
}
