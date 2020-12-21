using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Vector3 relativePosition;
    private GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("Player");
        relativePosition = this.transform.position - player.transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public void InitializeCamera()
    {
        
    }

    public void repositionCamera()
    {
        this.transform.position = player.transform.position + relativePosition;
    }
}
