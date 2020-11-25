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

    [Header("ColoringAmount")]
    public int colorPerAmount;

    [Header("TimerIfFailed")]
    public float restartTimer;

    [Header("HideInInspector")]
    public float timeDelayBetweenSpawns;
    public float minDisBetweenTargetAndPlayer;

    public SC_Gridtile playerSpawnTile;
    public SC_Gridtile targetSpawnTile;

    public bool playerSpawned;
    public bool targetSpawned;
    public bool creatingPath;
    public bool gridReady;
    bool colorSwap;

    public GameObject tileParent;
    public GameObject obstacleParent;
    public GameObject goalParent;

    public SC_Gridtile[,] grid;
    public List<SC_Gridtile> gridList = new List<SC_Gridtile>();

    private int maxTries = 25;

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
        while (gridList.Count < (xLength * zLength))
        {
            for (int i = 0; i < xLength; i++)
            {
                int maxVerSpawnAmount = 0;
                int switchNumber = 0;
                for (int ia = 0; ia < zLength; ia++)
                {
                    gridTile.xPos = i;
                    gridTile.zPos = ia;
                    grid[i, ia] = Instantiate(gridTile, new Vector3(i, 0f, ia), gridTile.transform.rotation, tileParent.transform);
                    grid[i, ia].gameObject.isStatic = true;
                    gridList.Add(grid[i, ia]);

                    if (ia == switchNumber + colorPerAmount)
                    {
                        switchNumber = ia;
                        colorSwap = !colorSwap;
                    }
                    //For The Checkers Effect

                    //if (colorSwap)
                    //{
                    //    grid[i, ia].SetColor(Color.white);
                    //}
                    //else
                    //{
                    //    grid[i, ia].SetColor(Color.black);
                    //}

                    if (ia == maxVerSpawnAmount + maxSpawnVerticalTileAmount)
                    {
                        maxVerSpawnAmount = ia;
                        yield return new WaitForSeconds(timeDelayBetweenSpawns);
                    }
                }
                yield return new WaitForSeconds(timeDelayBetweenSpawns);
            }
        }
        gridReady = true;
        Debug.Log("Grid Ready To Be Used");
        yield break;
    }

    public IEnumerator RandomSpawns()
    {
        int maxSpawnAmount = 0;
        for (int ib = 0; ib < obstacleAmount; ib++)
        {
            int randomIndex = Random.Range(0, gridList.Count);
            if (!gridList[randomIndex].spawnedOn)
            {
                SC_Gridtile currentTile = gridList[randomIndex];
                GameObject obs = Instantiate(obstacle, new Vector3(currentTile.transform.position.x, 0.5f, currentTile.transform.position.z), obstacle.transform.rotation, obstacleParent.transform);
                obs.isStatic = true;
                currentTile.spawnedOn = true;
            }
            if (ib == maxSpawnAmount + maxSpawnObstacleAmount)
            {
                maxSpawnAmount = ib;
                yield return new WaitForSeconds(timeDelayBetweenSpawns);
            }
        }
        int playerTries = 0;

        while (!playerSpawned)
        {
            if (playerTries < maxTries)
            {
                playerTries++;
                SpawnPlayer();
                yield return new WaitForSeconds(0f);
            }
            else
            {
                Debug.Log("Player Could Not Spawn");
                creatingPath = false;
                yield break;
            }
        }

        int targetTries = 0;

        while (!targetSpawned)
        {
            if (targetTries < maxTries)
            {
                targetTries++;
                SpawnTarget();
                yield return new WaitForSeconds(0f);
            }
            else
            {
                Debug.Log("Target Could Not Spawn");
                creatingPath = false;
                yield break;
            }
        }

        if (playerSpawned && targetSpawned)
        { 
            MakeNewPath();
        }
        yield break;
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
            GameObject plr = Instantiate(player, new Vector3(playerSpawnTile.transform.position.x, 0.75f, playerSpawnTile.transform.position.z), player.transform.rotation, goalParent.transform);
            plr.isStatic = true;
            playerSpawned = true;
        }
    }

    public void SpawnTarget()
    {
        int randomIndex = Random.Range(0, gridList.Count);
        if (!gridList[randomIndex].spawnedOn && gridList[randomIndex] != playerSpawnTile)
        {
            float disToPlayer = Vector3.Distance(playerSpawnTile.transform.position, gridList[randomIndex].transform.position);
            if(disToPlayer > minDisBetweenTargetAndPlayer)
            {
                targetSpawnTile = gridList[randomIndex];
                GameObject trg = Instantiate(target, new Vector3(targetSpawnTile.transform.position.x, 0.5f, targetSpawnTile.transform.position.z), target.transform.rotation, goalParent.transform);
                trg.isStatic = true;
                targetSpawned = true;
            }   
        }
    }

    public void CreateNewPath()
    {
        if (!gridReady)
        {
            Debug.Log("Please wait for Grid To Finish");
            return;
        }
        if (!creatingPath)
        {
            creatingPath = true;
            SC_LoadingBar_AStar.single.StartGenerating();
            ResetManager();
            RestartPath();
        }
        else
        {
            Debug.Log("Please wait for Path To Finish");
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
    }

    public void RestartPath()
    {
        obstacleParent = SpawnParent("ObstacleParent");
        goalParent = SpawnParent("GoalParent");
        StartCoroutine(RandomSpawns());
    }
}
