using UnityEngine;
using System.Collections;

public class TaskAttackMelee : Task
{
    Transform mTransform;
    Agent targetAgent;
    NavigationController mNavigation;

    public float duration = 1.0f;
    float timer;

    // Use this for initialization
    public override void Construct()
    {
        targetAgent = null;
        mTransform = gameObject.GetComponent<Transform>();
        mNavigation = gameObject.GetComponent<NavigationController>();

        timer = duration;
    }

    public void Initialize(Agent target)
    {
        targetAgent = target;
    }

    public void Attack()
    {
        targetAgent.OnSignal(Signal.Hit, "TaskAttackMelee::Attack");
    }

    // Update is called once per frame
    public override void Update()
    {
        gameObject.GetComponent<Actions>().AttackMelee();
        
        mNavigation.MoveTo(mTransform.position);
        mNavigation.LookAt(targetAgent.transform.position);

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = duration;
            Attack();
        }
    }

    public override void Destruct()
    {
        gameObject.GetComponent<Actions>().StopAiming();
    }

    public override void OnSignal(Signal signal, string origin)
    {
        origin = origin + "::TaskShoot::" + TaskManagerDebugger.SignalNames[(int)signal];
        switch (signal)
        {
            case Signal.TargetOutOfRange:
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
