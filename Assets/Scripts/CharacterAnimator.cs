using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAnimator : MonoBehaviour {

    NavMeshAgent agent;
    public static Animator animator;
    PlayerController player;

	// Use this for initialization
	void Start () {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        player = GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        float speedPercent = agent.velocity.magnitude / agent.speed;
        animator.SetFloat("speedPercent", speedPercent, 0.1f, Time.deltaTime);

        animator.SetBool("inCombat", player.inCombat);

        if (Input.GetKeyDown(KeyCode.H))
        {
            agent.SetDestination(this.transform.position);
            animator.SetTrigger("attack");
            player.Attack(PlayerController.AttackTypes.Normal);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            agent.SetDestination(this.transform.position);
            animator.SetTrigger("attackSweep");
            player.Attack(PlayerController.AttackTypes.Sweep);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (player.playerMana.value >= 80)
            {
                agent.SetDestination(this.transform.position);
                animator.SetTrigger("spell");
                player.Attack(PlayerController.AttackTypes.Fireball);
            }
        }
    }
}
