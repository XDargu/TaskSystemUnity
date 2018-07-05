using UnityEngine;
using System.Collections;

public enum AgentType
{
    Wanderer = 0,
    Follower,
    Zombie,

    Count
}

public abstract class Agent : MonoBehaviour {

    public AgentType type;
    public int health = 100;

    public virtual void OnSignal(Signal signal, string origin)
    {
    }
}
