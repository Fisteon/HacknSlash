using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMovement : MonoBehaviour {

    private NavMeshAgent agent;
    public NavMeshObstacle obstacle;
    private Vector3 startingPosition;
    private bool targetAcquired;
    private bool targetReached;
    public GameObject target;
    public Animator animator;
    private float attackCooldown = 2f;

    // Use this for initialization
    void Start () {
        animator = GetComponentInChildren<Animator>();
        startingPosition = this.transform.position;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 2;
        obstacle = GetComponent<NavMeshObstacle>();
        InvokeRepeating("IdleMove", 0f, 2.0f);
        targetReached = false;
    }
	
	// Update is called once per frame
	void Update () {

        float speedPercent = agent.velocity.magnitude / agent.speed;
        if (!targetAcquired && speedPercent > 0.5f)
        {
            speedPercent = 0.5f;
        }
        animator.SetFloat("speedPercent", speedPercent, 0.1f, Time.deltaTime);

        if (targetAcquired)
        {
            agent.speed = 6;
            FollowTarget();
        }
    }

    private void IdleMove()
    {
        Vector3 idleMove = Random.insideUnitCircle * 5;
        idleMove.z = idleMove.y;
        idleMove.y = 0;
        agent.SetDestination(startingPosition + idleMove);
    }

    private void FollowTarget()
    {
        if (Vector3.Distance(this.transform.position, target.transform.position) < 2)
        {
            agent.enabled = false;
            obstacle.enabled = true;

            if (agent.velocity.magnitude == 0)
            {
                Vector3 position = target.transform.position;
                position.y = transform.position.y;
                Quaternion targetRotation = Quaternion.LookRotation(position - transform.position, Vector3.up);
                this.transform.rotation = Quaternion.LerpUnclamped(this.transform.rotation, targetRotation, 20 * Time.deltaTime);
            }

            if (!IsInvoking("Attack"))
            {
                InvokeRepeating("Attack", 0f, 1f);
            }
        }
        else
        {
            CancelInvoke();
            obstacle.enabled = false;
            agent.enabled = true;
            agent.SetDestination(target.transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target == null)
        {
            if (other.name == "Player")
            {
                target = other.gameObject;
                targetAcquired = true;
                CancelInvoke();
            }
        }
    }

    private void Attack()
    {
        animator.SetTrigger("attack");
        target.GetComponent<PlayerController>().Attacked(2);
    }
}
