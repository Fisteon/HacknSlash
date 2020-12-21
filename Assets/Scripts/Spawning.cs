using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawning : MonoBehaviour
{

    public GameObject monster;
    public int monsterQuantity = 10;
    private float length;
    private float width;

    private bool spawnFinished = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnMonsters()
    {
        Collider spawnArea = GetComponent<Collider>();
        if (spawnArea == null || spawnFinished)
        {
            return;
        }

        length = spawnArea.bounds.size.x / 2;
        width = spawnArea.bounds.size.z / 2;

        for (int i = 0; i < monsterQuantity; i++)
        {
            float x = Random.Range(0, length);
            float z = Random.Range(0, width);

            Vector3 position = this.transform.position + new Vector3(x, 0.558f, z);
            GameObject p = Instantiate(monster, position, Quaternion.identity);
            p.transform.name = "Monster";
        }
        spawnFinished = true;
    }

    public void SpawnBoss()
    {
        GameObject p = Instantiate(monster);
        p.transform.name = "Boss";
    }
}
