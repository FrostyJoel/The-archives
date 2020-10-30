using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SC_GameManager : MonoBehaviour
{
    public static SC_GameManager single;
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    public GameObject hitEffect;
    public int amountOfRooms;
    public int minimumAmountOfEnemyRooms;
    public Camera tempStarterCam;
    public float winOffset;


    [Header ("HideInInspector")]
    public bool gameStart = false;
    public int enemyRoomAmount;
    public bool devMode;
    public bool enemyHpDropActivated;
    public bool killingenemyActivated;
    public List<Transform> playerSpawnPos = new List<Transform>();
    public List<GameObject> allEnemyRooms = new List<GameObject>();

    private void Awake()
    {
        if (single != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            single = this;
            DontDestroyOnLoad(this);
        }

        //tempStarterCam = FindObjectOfType<Camera>();

        if (GetComponentInChildren<SC_RoomManager>() != null)
        { 
            GetComponentInChildren<SC_RoomManager>().maxAmountOfRooms = amountOfRooms;
        }
    }

    public void ResetManager()
    {
        gameStart = false;
        enemyRoomAmount = 0;
        playerSpawnPos.Clear();
        allEnemyRooms.Clear();
        enemyHpDropActivated = false;
        killingenemyActivated = false;
    }

    public void GetRandomRoomToSpawnEnemies(List<GameObject> spawnedRooms)
    {
        allEnemyRooms = spawnedRooms;
        enemyRoomAmount = Random.Range(minimumAmountOfEnemyRooms, spawnedRooms.Count);
        List<GameObject> enemySpawnedRooms = new List<GameObject>();
        for (int i = 0; i < enemyRoomAmount; i++)
        {
            int randomIndex = Random.Range(0, spawnedRooms.Count);
            GameObject RandomRoom = spawnedRooms[randomIndex];
            if (!enemySpawnedRooms.Contains(RandomRoom) && !RandomRoom.GetComponent<SC_Room>().hasEnemies)
            {
                enemySpawnedRooms.Add(RandomRoom);
                RandomRoom.GetComponent<SC_Room>().hasEnemies = true;
            }
        }

        foreach (GameObject room in enemySpawnedRooms)
        {
            Debug.Log(room.name);
            RandomEnemyAmount(room.GetComponent<SC_Room>());
        }
        SpawnPlayer();
    }

    public void RandomEnemyAmount(SC_Room room)
    {
        if(room.spawnPosEnemies.Length <= 0) { return; }
        List<Transform> enemySpawnPos = new List<Transform>();
        int randomEnemyAmountIndex = Random.Range(1, room.maxAmountOfEnemies);
        for (int i = 0; i < randomEnemyAmountIndex; i++)
        {
            int randomIndex = Random.Range(0, room.spawnPosEnemies.Length);
            Transform randomTransform = room.spawnPosEnemies[randomIndex];

            if (!enemySpawnPos.Contains(randomTransform))
            {
                enemySpawnPos.Add(randomTransform);
            }
        }

        foreach (Transform spawnPos in enemySpawnPos)
        {
            SpawnEnemy(spawnPos, room);
        }
    }

    public void SpawnEnemy(Transform spawnPoint,SC_Room room)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint);
    }

    public void SpawnPlayer()
    {

        int randomIndex = Random.Range(0, playerSpawnPos.Count);

        Instantiate(playerPrefab.gameObject, playerSpawnPos[randomIndex].position, playerSpawnPos[randomIndex].rotation, null);
        tempStarterCam.gameObject.SetActive(false);
        gameStart = true;

        Debug.Log(gameStart);
    }
}
