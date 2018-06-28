using UnityEngine;
using System.Collections;

public class TaskReactHit : Task {

    Agent mAgent;

    public float duration = 0.1f;
    float timer;

    // Use this for initialization
    public override void Construct ()
    {
        timer = duration;

        gameObject.GetComponent<Actions>().Damage();

        mAgent = gameObject.GetComponent<Agent>();
        mAgent.health -= 20;
    }
    
    // Update is called once per frame
    public override void Update()
    {
        timer -= Time.deltaTime;
        timer = Mathf.Max(timer, 0.0f);

        if (timer <= 0)
        {
            OnTimerEnds();
        }
    }

    public void OnTimerEnds()
    {
        TaskManager mTaskManager = gameObject.GetComponent<TaskManager>();
        mTaskManager.UnregisterTask(this, "TaskReactHit::OnTimerEnds");

        if (mAgent.health <= 0)
        {
            OnHealthDepleted();
        }
    }

    public void OnHealthDepleted()
    {
        TaskManager mTaskManager = gameObject.GetComponent<TaskManager>();
        TaskDeath taskDeath = new TaskDeath();
        taskDeath.priority = 5;
        mTaskManager.TriggerTask(taskDeath, "TaskReactHit::OnHealthDepleted");
    }

    public override void Destruct ()
    {
    
    }

    public override void OnSignal (Signal signal, string origin)
    {
        
    }
}
