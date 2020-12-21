using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dungeon : MonoBehaviour {

    public GameObject[] platformModels;
    public GameObject startingPlatform;
    public GameObject deadEndPlatform;
    public GameObject blocker;

    [Range(1,10)]
    public int dungeonSize;
    private int depth = 1;

    private List<GameObject> platforms;
    private List<GameObject> helperList;
    private GameObject[,] map = new GameObject[30, 30];

    private NavMeshSurface surface;


	// Use this for initialization
	void Start () {
        platforms = new List<GameObject>();
        helperList = new List<GameObject>();
        surface = this.GetComponent<NavMeshSurface>();

        GenerateInitialPlatform();
        InvokeRepeating("GenerateDungeon", 0f, 0.1f);
    }
    	
	// Update is called once per frame
	void Update () {

    }

    private void GenerateSurface()
    {
        Debug.Log("Generating.....");
        surface.BuildNavMesh();
        GameController.gameController.gameReady = true;
    }

    private void GenerateInitialPlatform()
    {
        startingPlatform = Instantiate(startingPlatform, this.transform);
        startingPlatform.transform.name = "OriginPlatform";
        Destroy(startingPlatform.transform.Find("SpawnArea").GetComponent<Collider>());
        startingPlatform.GetComponent<Platform>().coords['x'] = 15;
        startingPlatform.GetComponent<Platform>().coords['z'] = 15;
        GeneratePlatforms(startingPlatform);
        platforms = helperList;
    }

    private void GenerateDungeon()
    {
        helperList = new List<GameObject>();
        foreach (GameObject p in platforms)
        {
            if (p == null)
            {
                continue;
            }
            if (p.GetComponent<Platform>().exitsUnused != 0)
            {
                Input.GetKey(KeyCode.A);
                GeneratePlatforms(p);
            }
        }
        depth++;
        platforms = helperList;
        if (platforms.Count == 0)
        {
            GenerateSurface();
            PlayerController.progressSegment = 10f;// 100f / (transform.childCount * 10 / 4);
            CancelInvoke();
        }
    }

    private int[] CalculateNewPlatformCoords(GameObject platform, Vector3 direction)
    {
        Dictionary<char, int> coords = platform.GetComponent<Platform>().coords;
        int x = coords['x'];
        int z = coords['z'];

        x = x + (int)direction.x;
        z = z + (int)direction.z;

        int[] newCoords = new int[2];
        newCoords[0] = x;
        newCoords[1] = z;

        return newCoords;
    }

    private Vector3 GetNormalizedDirection(Vector3 point, Vector3 platformCenter)
    {
        return ((-point) + platformCenter).normalized;
    }

    private void GeneratePlatforms(GameObject platform)
    {
        GameObject newPlatform = null;
        EntryPoint[] entryPoints = platform.GetComponent<PlatformEntryPoints>().entryPoints;
        foreach(EntryPoint point in entryPoints)
        {
            if (point.isUsed == false)
            {
                Vector3 direction = GetNormalizedDirection(point.transform.position, platform.transform.position);
                int[] newCoords = CalculateNewPlatformCoords(platform, direction);
                if (map[newCoords[0], newCoords[1]] == null)
                {
                    float chanceForEndPlatform = Mathf.Pow(0.93f, (11 - dungeonSize) * depth);
                    if (Random.Range(0f, 1f) < chanceForEndPlatform)
                    {
                        newPlatform = Instantiate(platformModels[Random.Range(0, platformModels.Length)], this.transform);
                    }
                    else
                    {
                        newPlatform = Instantiate(deadEndPlatform, this.transform);
                    }
                    EntryPoint[] tempPoints = Translate(platform, newPlatform, point);
                    Rotate(tempPoints[0], tempPoints[1], newPlatform);

                    newPlatform.GetComponent<Platform>().isSet = true;
                    newPlatform.isStatic = true;

                    newPlatform.GetComponent<Platform>().coords['x'] = newCoords[0];
                    newPlatform.GetComponent<Platform>().coords['z'] = newCoords[1];
                    CalculateFacingDirection(newPlatform);

                    map[newCoords[0], newCoords[1]] = newPlatform;

                    helperList.Add(newPlatform);

                    platform.GetComponent<Platform>().neighbourPlatforms.Add(newPlatform.GetComponent<Platform>());
                    newPlatform.GetComponent<Platform>().neighbourPlatforms.Add(platform.GetComponent<Platform>());
                }
                else
                {
                    if (!ArePlatformsFacing(newCoords, platform))
                    {
                        newPlatform = Instantiate(blocker);
                        EntryPoint[] tempPoints = Translate(platform, newPlatform, point);
                        Rotate(tempPoints[0], tempPoints[1], newPlatform);
                        newPlatform.transform.position = new Vector3(newPlatform.transform.position.x, 1.5f, newPlatform.transform.position.z);
                    }
                }
            }
        }
    }

    private bool ArePlatformsFacing(int[] occupiedSpaceCoordinates, GameObject platform)
    {
        List<int[]> face = map[occupiedSpaceCoordinates[0], occupiedSpaceCoordinates[1]].GetComponent<Platform>().lookingAt;
        Dictionary<char, int> currentCoordinates = platform.GetComponent<Platform>().coords;
        foreach (int[] f in face)
        {
            if (currentCoordinates['x'] == f[0] && currentCoordinates['z'] == f[1])
            {
                return true;
            }
        }
        return false;
    }

    private void CalculateFacingDirection(GameObject p)
    {
        Platform platform = p.GetComponent<Platform>();
        EntryPoint[] points = platform.GetComponent<PlatformEntryPoints>().entryPoints;
        foreach(EntryPoint point in points)
        {
            int[] lookingAt = new int[2];
            Vector3 lookingDirection = GetNormalizedDirection(point.transform.position, platform.transform.position);
            lookingAt[0] = platform.coords['x'] + (int)lookingDirection.x;
            lookingAt[1] = platform.coords['z'] + (int)lookingDirection.z;
            platform.lookingAt.Add(lookingAt);
        }
    }

    private void Rotate(EntryPoint entryPoint, EntryPoint exitPoint, GameObject movingPlatform)
    { 
        float rotation = Azimuth(-exitPoint.transform.forward) - Azimuth(entryPoint.transform.forward);
        movingPlatform.transform.RotateAround(entryPoint.transform.position, Vector3.up, rotation);
    }

    private EntryPoint[] Translate(GameObject fixedPlatform, GameObject movingPlatform, EntryPoint exitPoint)
    {
        EntryPoint[] movingPlatformPoints = movingPlatform.GetComponent<PlatformEntryPoints>().entryPoints;

        List<int> unusedPoints = new List<int>();
        for (int i = 0; i < movingPlatformPoints.Length; i++)
        {
            if (!movingPlatformPoints[i].isUsed)
            {
                unusedPoints.Add(i);
            }
        }

        EntryPoint entryPoint = movingPlatformPoints[unusedPoints[Random.Range(0, unusedPoints.Count)]];

        Vector3 translation = (-entryPoint.transform.position + exitPoint.transform.position);
        movingPlatform.transform.Translate(translation);

        entryPoint.isUsed = true;
        exitPoint.isUsed = true;

        EntryPoint[] ret = new EntryPoint[2];
        ret[0] = entryPoint;
        ret[1] = exitPoint;

        return ret;
    }


    private float Azimuth(Vector3 vector)
    {
        Vector3 forward = new Vector3(0, 0, 1);
        return Vector3.Angle(forward, vector) * Mathf.Sign(vector.x);
    }
}
