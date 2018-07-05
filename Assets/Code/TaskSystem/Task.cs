using UnityEngine;
using System.Collections;

public enum Signal
{
    RifleFound = 0,
    RifleLost,
    RiflePickedUp,

	TargetFound,
	TargetLost,

    TargetInRange,
    TargetOutOfRange,

    TargetDeath,

    Hit,

    Count
}

public enum TaskType
{
    Default,
    Parallel,

    Count
}

public abstract class Task
{
    public int priority = 0;
    public TaskType type = TaskType.Default;
	public GameObject gameObject = null;

	// Use this for initialization
	public abstract void Construct();
	
	// Update is called once per frame
	public abstract void Update();

	public abstract void Destruct();

	public virtual void Pause()
	{
	}

	public virtual void Resume ()
	{
	}

	public virtual void OnSignal(Signal signal, string origin)
	{
	}
}
