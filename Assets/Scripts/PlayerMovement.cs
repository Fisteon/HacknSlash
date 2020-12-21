using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour {

    private NavMeshAgent agent;
    private Camera camera;
    private Plane ground;

    private Vector3 lastPosition;

	// Use this for initialization
	void Start () {
        agent = this.GetComponent<NavMeshAgent>();
        //agent.baseOffset = -0.0833333f;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        ground = new Plane(new Vector3(0f, 1f, 0f), new Vector3(0f, 1f, 0f));
        lastPosition = transform.position;

        InvokeRepeating("SummonAround", 0f, 1f);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(1))// && Input.GetKey(KeyCode.LeftShift))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            float point;
            if (ground.Raycast(ray, out point))
            {
                agent.destination = transform.position;

                Vector3 position = ray.GetPoint(point);
                position.y = transform.position.y;
                Quaternion targetRotation = Quaternion.LookRotation(position - transform.position, Vector3.up);
                this.transform.rotation = Quaternion.LerpUnclamped(this.transform.rotation, targetRotation, 40 * Time.deltaTime);
            }
        }
		else if (Input.GetMouseButton(0))
        {
            bool needDestination = true;
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.name == "Monster")
                {
                    agent.destination = hit.transform.position;
                    agent.stoppingDistance = 1;
                    needDestination = false;

                    Vector3 position = hit.transform.position;
                    position.y = transform.position.y;
                    Quaternion targetRotation = Quaternion.LookRotation(position - transform.position, Vector3.up);
                    this.transform.rotation = Quaternion.LerpUnclamped(this.transform.rotation, targetRotation, 40 * Time.deltaTime);
                }
            }

            float point;
            if (needDestination && !CharacterAnimator.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                agent.stoppingDistance = 0;
                if (ground.Raycast(ray, out point))
                {
                    agent.destination = ray.GetPoint(point);
                }
            }            
        }

        if (agent.velocity.magnitude > 0)
        {
            camera.GetComponent<CameraController>().repositionCamera();
            lastPosition = transform.position;
        }
	}

    private void SummonAround()
    {
        RaycastHit hit;
        LayerMask terrainMask = 1 << 10;
        if (Physics.Raycast(transform.position + 20*Vector3.up, Vector3.down, out hit, 100f, terrainMask))
        {
            Platform currentlyStandingOn = hit.transform.GetComponentInParent<Platform>();
            currentlyStandingOn.spawnArea.SpawnMonsters();
            foreach(Platform p in currentlyStandingOn.neighbourPlatforms)
            {
                p.spawnArea.SpawnMonsters();
            }
        }
    }
}
