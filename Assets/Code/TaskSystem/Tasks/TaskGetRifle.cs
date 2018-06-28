using UnityEngine;
using System.Collections;

public class TaskGetRifle : Task {

    Transform mTransform;
    Agent mAgent;

    // Use this for initialization
    public override void Construct ()
    {
        mAgent = gameObject.GetComponent<Agent>();
        mTransform = gameObject.GetComponent<Transform>();
    }
    
    // Update is called once per frame
    public override void Update()
    {
        
    }

    public override void Destruct ()
    {
    
    }

    public override void OnSignal (Signal signal, string origin)
    {
        
    }
}
