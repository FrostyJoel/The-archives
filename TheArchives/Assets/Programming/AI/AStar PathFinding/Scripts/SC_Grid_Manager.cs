using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Grid_Manager : MonoBehaviour
{
    public static SC_Grid_Manager single;
    private void Awake()
    {
        if (single == null)
        {
            single = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    [Header("Objects")]
    public SC_Gridtile gridTile;
    public GameObject obstacle;
    public GameObject player;
    public GameObject target;

    [Header("Size")]
    public int xLength;
    public int zLength;
    public int obstacleAmount;

    [Header("MaxSpawn")]
    public int maxSpawnVerticalTileAmount;
    public int maxSpawnObstacleAmount;
    public int maxColorPathAmount;

    [Header("HideInInspector")]
    public float timeDelay;

    public SC_Gridtile playerSpawnTile;
    public SC_Gridtile targetSpawnTile;

    public bool playerSpawned;
    public bool targetSpawned;
    public bool pathDone;

    public GameObject tileParent;
    public GameObject obstacleParent;
    public GameObject goalParent;

    public SC_Gridtile[,] grid;
    public List<SC_Gridtile> gridList = new List<SC_Gridtile>();

    void Start()
    {
        if (tileParent == null)
        {
            tileParent = SpawnParent("TileParent");
        }
        else
        {
            DestroyParent(tileParent);
            return;
        }

        if (obstacleParent == null)
        {
            obstacleParent = SpawnParent("ObstacleParent");
        }
        else
        {
            DestroyParent(obstacleParent);
            return;
        }

        if (goalParent == null)
        {
            goalParent = SpawnParent("GoalParent");
        }
        else
        {
            DestroyParent(goalParent);
            return;
        }
        StartCoroutine(GridSpawning());
    }

    public GameObject SpawnParent(string parentName)
    {
        GameObject parent = new GameObject(parentName);
        parent.transform.parent = transform;
        return parent;
    }

    public void DestroyParent(GameObject parent)
    {
        Destroy(parent);
    }

    public IEnumerator GridSpawning()
    {
        grid = new SC_Gridtile[xLength, zLength];
        int maxSpawnAmount = 0;
        while (gridList.Count < (xLength * zLength))
        {
            for (int i = 0; i < xLength; i++)
            {
                for (int ia = 0; ia < zLength; ia++)
                {
                    gridTile.xPos = i;
                    gridTile.zPos = ia;
                    grid[i, ia] = Instantiate(gridTile, new Vector3(i, 0f, ia), gridTile.transform.rotation, tileParent.transform);
                    gridList.Add(grid[i, ia]);

                    if (ia == maxSpawnAmount + maxSpawnVerticalTileAmount)
                    {
                        maxSpawnAmount = ia;
                        yield return new WaitForSeconds(timeDelay);
                    }
                }
                yield return new WaitForSeconds(timeDelay);
            }
        }

        StartCoroutine(RandomSpawns());
    }

    public IEnumerator RandomSpawns()
    {
        int maxSpawnAmount = 0;
        for (int ib = 0; ib < obstacleAmount; ib++)
        {
            Debug.Log("RandomObstacle");
            int randomIndex = Random.Range(0, gridList.Count);
            if (!gridList[randomIndex].spawnedOn)
            {
                SC_Gridtile currentTile = gridList[randomIndex];
                Instantiate(obstacle, new Vector3(currentTile.transform.position.x, 0.5f, currentTile.transform.position.z), obstacle.transform.rotation, obstacleParent.transform);
                currentTile.spawnedOn = true;
                Debug.Log("Hey i'm Tree");
            }
            if (ib == maxSpawnAmount + maxSpawnObstacleAmount)
            {
                maxSpawnAmount = ib;
                yield return new WaitForSeconds(timeDelay);
            }
        }

        while (!playerSpawned)
        {
            SpawnPlayer();
        }

        while (!targetSpawned)
        {
            SpawnTarget();
        }

        if (playerSpawned && targetSpawned)
        {
            print("Both Spawned");
            MakeNewPath();
        }
    }

    public void MakeNewPath()
    {
        SC_Astar_Pathfinder path = new SC_Astar_Pathfinder();
        StartCoroutine(path.StartSearch(playerSpawnTile, targetSpawnTile));
    }

    public void SpawnPlayer()
    {
        int randomIndex = Random.Range(0, gridList.Count);
        if (!gridList[randomIndex].spawnedOn && gridList[randomIndex] != targetSpawnTile)
        {
            playerSpawnTile = gridList[randomIndex];
            Instantiate(player, new Vector3(playerSpawnTile.transform.position.x, 0.75f, playerSpawnTile.transform.position.z), player.transform.rotation, goalParent.transform);
            playerSpawned = true;
        }
    }

    public void SpawnTarget()
    {
        int randomIndex = Random.Range(0, gridList.Count);
        if (!gridList[randomIndex].spawnedOn && gridList[randomIndex] != playerSpawnTile)
        {
            targetSpawnTile = gridList[randomIndex];
            Instantiate(target, new Vector3(targetSpawnTile.transform.position.x, 0.5f, targetSpawnTile.transform.position.z), target.transform.rotation, goalParent.transform);
            targetSpawned = true;
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (pathDone)
            {
                ResetManager();
                RestartPath();
            }
            else
            {
                Debug.Log("Please wait for PathToFinish");
            }
        }
    }
    public void ResetManager()
    {
        DestroyParent(obstacleParent);
        DestroyParent(goalParent);
        foreach (SC_Gridtile gridtile in grid)
        {
            gridtile.spawnedOn = false;
            gridtile.SetColor(Color.white);
        }
        playerSpawnTile = null;
        playerSpawned = false;
        targetSpawnTile = null;
        targetSpawned = false;
        pathDone = false;
    }
    public void RestartPath()
    {
        obstacleParent = SpawnParent("ObstacleParent");
        goalParent = SpawnParent("GoalParent");
        StartCoroutine(RandomSpawns());
    }
}
