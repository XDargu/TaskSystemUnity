using UnityEngine;
using System.Collections;

public class NavigationController : MonoBehaviour {

	public float speed;
    public float slerp = 0.05f;
    public bool doAvoidance = true;

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

    Vector3 GetAvoidance()
    {
        Vector3 avoidance = Vector3.zero;

        if (doAvoidance)
        {
            Collider[] hitColliders = Physics.OverlapSphere(mTransform.position, 1.0f);
            int i = 0;
            while (i < hitColliders.Length)
            {
                Agent agent = hitColliders[i].GetComponent<Agent>();

                if (agent != null && hitColliders[i].gameObject != gameObject)
                {
                    if (agent.health > 0)
                    {
                        Vector3 awayFromAgent = mTransform.position - hitColliders[i].transform.position;
                        float distanceToAgent = awayFromAgent.magnitude;
                        awayFromAgent.Normalize();

                        Debug.Log(gameObject.name + " is being pushed by " + hitColliders[i].gameObject.name);

                        avoidance += awayFromAgent * (1.0f - distanceToAgent) * Time.deltaTime;
                    }
                }

                i++;
            }
        }

        return avoidance;
    }
	
	// Update is called once per frame
	void Update()
    {
        Vector3 position2D = mTransform.position;
        position2D.y = 0;

        Vector3 destination2D = destination;
        destination2D.y = 0;

        Vector3 targetVelocity = destination2D - position2D;

        // Away from target
        if (targetVelocity.magnitude > 0.3f)
        {
            targetVelocity.Normalize();
            Quaternion alignment = Quaternion.LookRotation(velocity, Vector3.up);
            Quaternion targetAlignment = Quaternion.LookRotation(targetVelocity, Vector3.up);
            Quaternion targetRot = Quaternion.Slerp(alignment, targetAlignment, slerp);
            velocity = targetRot * Vector3.forward;
            velocity.Normalize();
        }
        // In target
        else
        {
            velocity = Vector3.zero;
        }
        
        float speedToReachTargetInFrame = velocity.magnitude / Time.deltaTime;

        // dist = min(speed*delta, toTarget)
        // dist = x * delta
        // x = dist / delta

        // Clamp the speed using our speed
        float realSpeed = Mathf.Min(speed, speedToReachTargetInFrame);


        // Distance to do this frame
        float distance = realSpeed * Time.deltaTime;

        Vector3 movementTowardsTarget = velocity * distance;

        mTransform.position += movementTowardsTarget + GetAvoidance();

        if (movementTowardsTarget.magnitude > 0)
        {
            animationActions.SetIsMoving();
            mTransform.LookAt(mTransform.position + velocity);
        }

        animationActions.SetSpeed(realSpeed);
	}

	public void MoveTo (Vector3 moveToDestination)
	{
		destination = moveToDestination;
	}

    void OnDrawGizmos()
    {
        if (mTransform != null)
        {
            if (UnityEditor.Selection.activeGameObject != gameObject)
            {
                return;
            }

            Vector3 position2D = mTransform.position;
            position2D.y = 0;

            Vector3 destination2D = destination;
            destination2D.y = 0;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(destination2D, 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(position2D, 0.2f);
            Gizmos.DrawLine(position2D, destination2D);

            Gizmos.color = Color.green;

            Gizmos.DrawLine(position2D, position2D + velocity);

            float speed = 1.5f;
            float PiOver2 = Mathf.PI;
            float mask = Mathf.Sin(Time.timeSinceLevelLoad * speed * 2.0f) > 0.0f ? 1.0f : 0.0f;
            float value0to1 = Mathf.Abs(Mathf.Cos(Time.timeSinceLevelLoad * speed)) * mask;

            float mask2 = Mathf.Sin((Time.timeSinceLevelLoad * speed * 2.0f) + PiOver2) > 0.0f ? 1.0f : 0.0f;
            value0to1 = value0to1 + (1.0f - Mathf.Abs(Mathf.Cos((Time.timeSinceLevelLoad * speed) + PiOver2))) * mask2;

            Gizmos.DrawSphere(Vector3.Lerp(destination, mTransform.position, value0to1), 0.1f);
        }
    }
}
