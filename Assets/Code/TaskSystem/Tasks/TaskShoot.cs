using UnityEngine;
using System.Collections;

public class TaskShoot : Task 
{
    GameObject mBullet;

    Transform mTransform;
    Transform targetTransform;
    NavigationController mNavigation;

    public float duration = 1.0f;
    float timer;

    // Use this for initialization
    public override void Construct ()
    {
        targetTransform = null;
        mTransform = gameObject.GetComponent<Transform>();
        mNavigation = gameObject.GetComponent<NavigationController>();

        timer = duration;
    }

    public void Initialize(Transform target, GameObject bullet)
    {
        targetTransform = target;
        mBullet = bullet;
    }

    public void Shoot()
    {
        gameObject.GetComponent<Actions>().Attack();

        GameObject bullet =  Object.Instantiate(mBullet);
        bullet.GetComponent<Transform>().position = mTransform.position + mTransform.forward * 0.7f + mTransform.up * 1.3f;

        Vector3 toTarget = targetTransform.position - mTransform.position;
        toTarget.Normalize();

        bullet.GetComponent<Rigidbody>().AddForce(toTarget * 20.0f, ForceMode.VelocityChange);
    }
    
    // Update is called once per frame
    public override void Update ()
    {
        gameObject.GetComponent<Actions>().SetAiming();

        mNavigation.MoveTo(mTransform.position);
        mNavigation.LookAt(targetTransform.position);

        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = duration;
            Shoot();
        }
    }

    public override void Destruct ()
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
