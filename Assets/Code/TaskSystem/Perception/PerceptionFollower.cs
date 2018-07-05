using UnityEngine;
using System.Collections;

public class PerceptionFollower : MonoBehaviour 
{
	Agent targetAgent;
	Transform mTransform;

	Agent mAgent;

    [Header("Detection ranges")]
    public float targetRangeFind = 9.0f;
    public float targetRangeLose = 11.0f;

    [Header("Attack ranges")]
    public float targetRangeInRange = 5.0f;
    public float targetRangeOutOfRange = 7.0f;

    // R
    bool inRange = false;
    bool alive = true;

    public AgentType[] targetTypes;

    void ResetTrackingVariables()
    {
        inRange = false;
        alive = true;
    }

    // Use this for initialization
    void Start ()
	{
		targetAgent = null;
		mTransform = gameObject.GetComponent<Transform>();
		mAgent = gameObject.GetComponent<Agent> ();
	}

    public void SetDetectionRange(float distance, float hysteresis)
    {
        targetRangeFind = distance;
        targetRangeLose = distance + hysteresis;
    }

    public void SetAttackRange(float distance, float hysteresis)
    {
        targetRangeInRange = distance;
        targetRangeOutOfRange = distance + hysteresis;
    }

    Agent TryFindTargetAround()
    {
        Collider[] hitColliders = Physics.OverlapSphere(mTransform.position, targetRangeFind);
        int i = 0;
        while (i < hitColliders.Length)
        {
            Agent agent = hitColliders[i].GetComponent<Agent>();
            
            if (agent != null)
            {
                for (int j = 0; j < targetTypes.Length; j++)
                {
                    if (agent.type == targetTypes[j])
                    {
                        if (agent.health > 0)
                        {
                            return agent;
                        }
                    }
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
                mAgent.OnSignal(Signal.TargetFound, "Perception");

                ResetTrackingVariables();
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
                    mAgent.OnSignal(Signal.TargetLost, "Perception");
                    targetAgent = null;
                }

                // Check if it's in range to shoot
                if (!inRange && distanceToTarget < targetRangeInRange)
                {
                    inRange = true;
                    mAgent.OnSignal(Signal.TargetInRange, "Perception");
                }
                else if (inRange && distanceToTarget > targetRangeOutOfRange)
                {
                    inRange = false;
                    mAgent.OnSignal(Signal.TargetOutOfRange, "Perception");
                }

                // Death
                if (targetAgent != null && targetAgent.health <= 0)
                {
                    alive = false;
                    mAgent.OnSignal(Signal.TargetDeath, "Perception");
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

	public Agent GetTargetAgent()
	{
		return targetAgent;
	}

	void OnDrawGizmos ()
	{
		if (mTransform != null)
		{
            if (UnityEditor.Selection.activeGameObject != gameObject)
            {
                return;
            }

            UnityEditor.Handles.color = Color.magenta;
			UnityEditor.Handles.DrawWireDisc (mTransform.position, mTransform.up, targetRangeFind);

			//UnityEditor.Handles.color = Color.blue;
			//UnityEditor.Handles.DrawWireDisc (mTransform.position, mTransform.up, targetRangeLose);

            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc (mTransform.position, mTransform.up, targetRangeInRange);

            //UnityEditor.Handles.color = Color.green;
            //UnityEditor.Handles.DrawWireDisc (mTransform.position, mTransform.up, targetRangeOutOfRange);
            
            if (targetAgent)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(targetAgent.transform.position + Vector3.up * 2.0f, 0.2f);
            }
        }
    }
}
