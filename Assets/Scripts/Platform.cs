using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    public bool isSet = false;
    public int exitsUnused;

    public Dictionary<char, int> coords;
    public List<int[]> lookingAt;

    public List<Platform> neighbourPlatforms;

    public Spawning spawnArea;

	// Use this for initialization
	void Awake () {
        neighbourPlatforms = new List<Platform>();
        exitsUnused = this.GetComponent<PlatformEntryPoints>().entryPoints.Length;
        coords = new Dictionary<char, int>();

        lookingAt = new List<int[]>();
        spawnArea = GetComponentInChildren<Spawning>();
    }

    private void Start()
    {
        if (transform.name == "OriginPlatform")
        {
            Invoke("SpawnOnlyOrigin", 3f);
        }
    }

    // Update is called once per frame
    void Update () {
        /*if (Input.GetKeyDown(KeyCode.P))
        {
            spawnArea.SpawnMonsters();
        }*/
    }

    private void SpawnOnlyOrigin()
    {
        foreach(Platform p in neighbourPlatforms)
        {
            p.spawnArea.SpawnMonsters();
        }
    }
}
