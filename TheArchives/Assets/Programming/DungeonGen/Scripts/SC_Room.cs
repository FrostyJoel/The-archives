using System.Collections.Generic;
using UnityEngine;

public class SC_Room : MonoBehaviour
{
    [Header("Needed to work",order = 0)]
    [Space]
    [Header("AttachPoint", order = 1)]
    public AttachPoint[] attachPoints;
    [Header("RoomProp")]
    public BoxCollider roomCollider;
    public AvailableSlots roomType;
    [Header("Not Needed to work", order = 4)]
    [Space]
    [Header("Enemies", order = 5)]
    public int maxAmountOfEnemies;
    public Transform[] spawnPosEnemies;
    [Header("Objects in room Parent")]
    public Transform propsGeo;
    [Header("Coloring off Floor")]
    public GameObject[] floors;

    [HideInInspector]
    public List<GameObject> enemiesInRoom = new List<GameObject>();
    [HideInInspector]
    public bool hasEnemies;
    [HideInInspector]
    public List<SpawnablePosAndRot> spawnablePosAndRots = new List<SpawnablePosAndRot>();
    [HideInInspector]
    public bool fullyAttached;
    [HideInInspector]
    public bool isChecker;
    [HideInInspector]
    public bool isChecked;
    [HideInInspector]
    public MeshRenderer[] meshRenderers;
}



[System.Serializable]
public class SpawnablePosAndRot
{
    public Vector3 pos;
    public Quaternion rot;
    public GameObject room;
}

[System.Serializable]
public class AttachPoint
{
    public Transform point;
    [HideInInspector]
    public GameObject wall;
    public bool mapWall;
    [HideInInspector]
    public BoxCollider AttachCollider
    {
        get
        {
            BoxCollider myCollider = new BoxCollider();
            if (point)
            {
                myCollider = point.GetComponent<BoxCollider>();
            }
            return myCollider;
        }
    }
    public AvailableSlots nextSpawn;
    [HideInInspector]
    public bool attached;
    [HideInInspector]
    public bool canBeAttached;
    [HideInInspector]
    public SC_Room attachedTo;
    [HideInInspector]
    public Vector3 off;
}
