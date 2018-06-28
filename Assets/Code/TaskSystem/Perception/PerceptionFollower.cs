using UnityEngine;
using System.Collections;

public class PerceptionFollower : MonoBehaviour 
{
	Agent targetAgent;
	Transform mTransform;

	Follower mFollower;

	float targetRangeFind = 9.0f;
	float targetRangeLose = 11.0f;

    float targetRangeInRange = 5.0f;
    float targetRangeOutOfRAnge = 7.0f;

    bool inRange = false;
    bool alive = true;

	// Use this for initialization
	void Start ()
	{
		targetAgent = null;
		mTransform = gameObject.GetComponent<Transform>();
		mFollower = gameObject.GetComponent<Follower> ();
	}

    Agent TryFindTargetAround()
    {
        Collider[] hitColliders = Physics.OverlapSphere(mTransform.position, targetRangeFind);
        int i = 0;
        while (i < hitColliders.Length)
        {
            Wanderer wanderer = hitColliders[i].GetComponent<Wanderer>();
            if (wanderer != null)
            {
                Agent agent = wanderer.gameObject.GetComponent<Agent>();
                if (agent.health > 0)
                {
                    return agent;
                }
            }

            i++;
        }

        return null;
    }

	void UpdateTarget()
    {
        if (targetAgent == null)
        {
            targetAgent = TryFindTargetAround();

            if (targetAgent != null)
            {
                // Send signal, leader found
                mFollower.OnSignal(Signal.TargetFound, "Perception");
            }
        }
        else
        {
            if (alive)
            {
                Transform mAgentTransform = targetAgent.transform;

                // Ranges
                float distanceToTarget = (mAgentTransform.position - mTransform.position).magnitude;

                if (distanceToTarget > targetRangeLose)
                {
                    // Send signal, leader lost
                    mFollower.OnSignal(Signal.TargetLost, "Perception");
                    targetAgent = null;
                }

                // Check if it's in range to shoot
                if (!inRange && distanceToTarget < targetRangeInRange)
                {
                    inRange = true;
                    mFollower.OnSignal(Signal.TargetInRange, "Perception");
                }
                else if (distanceToTarget > targetRangeOutOfRAnge)
                {
                    inRange = false;
                    mFollower.OnSignal(Signal.TargetOutOfRange, "Perception");
                }

                // Death
                if (targetAgent.health <= 0)
                {
                    alive = false;
                    mFollower.OnSignal(Signal.TargetDeath, "Perception");
                    targetAgent = null;
                }
            }
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateTarget();
	}

	public Transform GetTargetTransform ()
	{
		return targetAgent.transform;
	}

	void OnDrawGizmos ()
	{
		if (mTransform != null)
		{
			UnityEditor.Handles.color = Color.magenta;
			UnityEditor.Handles.DrawWireDisc (mTransform.position, mTransform.up, targetRangeFind);

			//UnityEditor.Handles.color = Color.blue;
			//UnityEditor.Handles.DrawWireDisc (mTransform.position, mTransform.up, targetRangeLose);

            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc (mTransform.position, mTransform.up, targetRangeInRange);

            //UnityEditor.Handles.color = Color.green;
            //UnityEditor.Handles.DrawWireDisc (mTransform.position, mTransform.up, targetRangeOutOfRAnge);
		}
    }
}
