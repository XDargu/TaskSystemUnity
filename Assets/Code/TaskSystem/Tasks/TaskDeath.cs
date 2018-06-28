using UnityEngine;
using System.Collections;

public class TaskDeath : Task {

    Transform mTransform;
    NavigationController mNavigation;

    public float duration = 15.0f;
    float timer;

    // Use this for initialization
    public override void Construct ()
    {
        mTransform = gameObject.GetComponent<Transform>();
        mNavigation = gameObject.GetComponent<NavigationController>();
        mNavigation.MoveTo(mTransform.position);
        timer = duration;

        gameObject.GetComponent<Actions>().Death();
    }
    
    // Update is called once per frame
    public override void Update ()
    {
        timer -= Time.deltaTime;
        timer = Mathf.Max(timer, 0.0f);

        if (timer <= 0)
        {
            // Resurrect maybe
        }
    }

    public override void Destruct ()
    {
    
    }

    public override void OnSignal (Signal signal, string origin)
    {
        
    }
}
