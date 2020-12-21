using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHit : MonoBehaviour {

    float time;
    float attackDelay = 1.2f;

    GameObject player;

	// Use this for initialization
	void Start () {
        time = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time > time + attackDelay)
        {
            if (player != null)
            {
                player.GetComponent<PlayerController>().Attacked(25);
            }
            Destroy(this.gameObject);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            Debug.Log("Player is in");
            player = other.transform.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "Player")
        {
            player = null;
        }
    }
}
