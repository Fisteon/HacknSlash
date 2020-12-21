using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocker : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerStay(Collider other)
    {
        Invoke("DestroyMe", 0.1f); 
    }

    private void DestroyMe()
    {
        Destroy(this.gameObject);
    }
}
