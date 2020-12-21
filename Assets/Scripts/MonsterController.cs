using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterController : MonoBehaviour {

    public Slider monsterHP;
    private RectTransform monsterHPPosition;
    private RectTransform canvasSize;
    public float health = 100;
    public Camera camera;

    bool isAlive = true;
    bool hpVisible = false;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        if (hpVisible)
        {
            RepositionHealthBar();
        }
	}

    public void DamageTaken(int damage)
    {
        if (!hpVisible && isAlive)
        {
            canvasSize = GameObject.Find("UserOverlay").GetComponent<RectTransform>();
            monsterHP = Instantiate(monsterHP, transform.position, Quaternion.identity, canvasSize.transform);
            monsterHP.value = 100;
            monsterHPPosition = monsterHP.GetComponent<RectTransform>();
            camera = GameObject.Find("Main Camera").GetComponent<Camera>();
            hpVisible = true;
        }

        health -= damage;
        monsterHP.value -= damage;
        if (health <= 0)
        {
            StartCoroutine(Death());
            hpVisible = false;
        }
    }

    private IEnumerator Death()
    {
        if (isAlive)
        {
            isAlive = false;
            GetComponent<MonsterMovement>().obstacle.enabled = false;
            Destroy(GetComponent<MonsterMovement>());
            Destroy(GetComponent<CapsuleCollider>());
            Destroy(monsterHP.gameObject);
            PlayerController.triggerOnKill();
            GetComponent<MonsterMovement>().animator.Play("Death");
            yield return new WaitForSeconds(2f);
            Destroy(this.gameObject);
        }
    }

    private void RepositionHealthBar()
    {
        Vector3 monsterPosition = this.transform.position;
        monsterPosition.y += 1.5f;
        Vector2 worldPosition = camera.WorldToViewportPoint(monsterPosition);
        Vector2 screenPosition = new Vector2((worldPosition.x * canvasSize.sizeDelta.x - canvasSize.sizeDelta.x * 0.5f), (worldPosition.y * canvasSize.sizeDelta.y - canvasSize.sizeDelta.y * 0.5f));
        monsterHPPosition.anchoredPosition = screenPosition;
    }
}
