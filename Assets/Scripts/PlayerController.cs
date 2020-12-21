using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private float health = 100;
    private Camera camera;
    public Slider playerHP;
    public Slider playerMana;
    public Image fireballIcon;
    public enum AttackTypes { Normal, Sweep, Fireball };
    public GameObject FireballPrefab;
    private List<Collider> enemiesInRange;

    public bool inCombat;

    public static int sweepAttackDamage = 20;
    public static int singleTargetDamage = 40;
    public static int fireDamage = 30;

    public delegate void MyDelegate();
    public static MyDelegate triggerOnKill;

    public static float progressSegment;

    // Use this for initialization
    void Start()
    {
        playerHP.maxValue = 100;
        playerHP.value = 100;
        playerMana.value = 100;
        inCombat = false;
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();

        triggerOnKill += MonsterKilled;

        enemiesInRange = new List<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMana.value < 80)
        {
            fireballIcon.color = new Color(fireballIcon.color.r, fireballIcon.color.g, fireballIcon.color.b, 0.5f);
        }
        else
        {
            fireballIcon.color = new Color(fireballIcon.color.r, fireballIcon.color.g, fireballIcon.color.b, 1f);
        }
        playerMana.value += 0.3f;
        if (enemiesInRange.Count > 0 && !inCombat)
        {
            inCombat = true;
        }
        if (enemiesInRange.Count == 0 && inCombat)
        {
            inCombat = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Enemy")
        {
            enemiesInRange.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        enemiesInRange.Remove(other);
    }

    public void Attack(AttackTypes attackType)
    {
        if (attackType == AttackTypes.Sweep)
        {
            List<GameObject> enemiesHit = new List<GameObject>();
            List<Vector3> coneVectors = getConeVectors(this.transform.position, this.transform.forward, 45);
            foreach (Vector3 v in coneVectors)
            {
                RaycastHit[] enemiesInRange = Physics.RaycastAll(transform.position, (v - transform.position).normalized, 2);
                for (int i = 0; i < enemiesInRange.Length; i++)
                {
                    if (enemiesInRange[i].transform.gameObject.name == "Monster" || enemiesInRange[i].transform.gameObject.name == "Boss")
                    {
                        if (!enemiesHit.Contains(enemiesInRange[i].transform.gameObject))
                        {
                            enemiesHit.Add(enemiesInRange[i].transform.gameObject);
                        }
                    }
                }
            }
            foreach (GameObject m in enemiesHit)
            {
                if (m.name == "Monster")
                {
                    MonsterController monster = m.GetComponent<MonsterController>();
                    monster.DamageTaken(sweepAttackDamage);
                }
                else if (m.name == "Boss")
                {
                    GameController.gameController.BossHP.value -= sweepAttackDamage;
                }
            }
        }

        if (attackType == AttackTypes.Normal)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1.5f))
            {
                if (hit.transform.gameObject.name == "Monster")
                {
                    hit.transform.GetComponent<MonsterController>().DamageTaken(singleTargetDamage);
                }
                else if (hit.transform.gameObject.name == "Boss")
                {
                    GameController.gameController.BossHP.value -= singleTargetDamage;
                }
            }
        }

        if (attackType == AttackTypes.Fireball)
        {
            playerMana.value -= 80;
            Vector3 FireballSpawnPosition = this.transform.position;
            FireballSpawnPosition.y = 4;
            Instantiate(FireballPrefab, FireballSpawnPosition, this.transform.rotation);
        }
    }

    private List<Vector3> getConeVectors(Vector3 origin, Vector3 forward, int angleToSearch)
    {
        List<Vector3> result = new List<Vector3>();

        for (int x = -angleToSearch; x <= angleToSearch; x += 5)
        {
            Vector3 range = Quaternion.Euler(0, x, 0) * forward;
            result.Add(origin + 2 * range);
        }

        return result;
    }


    public void Attacked(int damageTaken)
    {
        health -= damageTaken;
        playerHP.value -= damageTaken;

        if (health <= 0)
        {
            Debug.Log("GAME OVER!");
            // game over screen
        }
    }

    private void MonsterKilled()
    {
        health += 5;
        playerHP.value += 5;
        GameController.gameController.progressBar.value += progressSegment;
    }
}
