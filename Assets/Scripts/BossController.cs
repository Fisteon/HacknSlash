using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class BossController : MonoBehaviour {

    Animator animator;
    public NavMeshAgent agent;
    public GameObject player;
    public GameObject hitArea;
    private GameObject m_hitArea;
    bool alive = true;

    private float attackCooldown = 5f;
    private float nextAttack = 0f;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (GameController.gameController.characterTransfered && alive)
        {
            if (GameController.gameController.BossHP.value <= 0 && alive)
            {
                alive = false;
                StartCoroutine(Death());
                return;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                return;
            }
            else if (Vector3.Distance(transform.position, player.transform.position) <= 5)
            {
                //agent.destination = transform.position;
                animator.SetFloat("speedPercent", 0f);
                agent.enabled = false;

                if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
                {
                    Vector3 position = player.transform.position;
                    position.y = transform.position.y;
                    Quaternion targetRotation = Quaternion.LookRotation(position - transform.position, Vector3.up);
                    this.transform.rotation = Quaternion.LerpUnclamped(this.transform.rotation, targetRotation, 60 * Time.deltaTime);
                }

                if (Time.time >= nextAttack)
                {
                    Attack();
                    nextAttack = Time.time + attackCooldown;
                }
            }
            else
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
                {
                    float speedPercent = agent.velocity.magnitude / agent.speed;
                    animator.SetFloat("speedPercent", speedPercent);

                    agent.enabled = true;
                    agent.destination = player.transform.position;
                }
            }
        }
	}

    IEnumerator Death()
    {
        animator.SetTrigger("death");
        Destroy(m_hitArea.gameObject);
        GameController.gameController.victoryText.enabled = true;
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);

        Debug.Log("You win!");
    }

    void Attack()
    {
        animator.SetTrigger("attack");
        m_hitArea = Instantiate(hitArea, transform.position + 3 * transform.forward.normalized + Vector3.up, Quaternion.identity);
    }
}
