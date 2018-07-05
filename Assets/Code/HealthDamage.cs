using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDamage : MonoBehaviour {

    Agent mAgent;

    public GameObject[] hideBracket1;
    public GameObject[] hideBracket2;
    public GameObject[] hideBracket3;

    // Use this for initialization
    void Start ()
    {
        mAgent = GetComponent<Agent>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (mAgent.health <= 20.0f)
        {
            foreach (GameObject gameObject in hideBracket3)
            {
                gameObject.SetActive(false);
            }
        }
        else if (mAgent.health <= 60.0f)
        {
            foreach (GameObject gameObject in hideBracket2)
            {
                gameObject.SetActive(false);
            }
        }
        else if (mAgent.health <= 85.0f)
        {
            foreach (GameObject gameObject in hideBracket1)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
