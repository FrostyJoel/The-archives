﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SC_RoomManager : MonoBehaviour
{
    public static SC_RoomManager single;
    [Header("StarterRoomTag")]
    public AvailableSlots startRoomType;
    [Header("Predetermined Rotations")]
    public StandardRoomRots sRoomRots;
    [Header("Dungeon Settings")]
    public float spawnDelay;
    public float restartTimer;
    public int maxAmountOfRooms;
    [Header("WallPrefab & Layers")]
    public GameObject mainWall;
    public LayerMask overlapLayer;
    public LayerMask attachPointsLayer;

    [HideInInspector]
    public bool startCreation;
    [HideInInspector]
    public List<SC_Room> allspawnedRooms = new List<SC_Room>();
    [HideInInspector]
    public List<GameObject> allFinishedSpawningRooms = new List<GameObject>();
    [HideInInspector]
    public SC_Room currRoomToCheck;
    [HideInInspector]
    public GameObject customDungeonParent;
    [HideInInspector]
    public int currentAmountOfRooms;
    [HideInInspector]
    public bool creatingDungeon;


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
        }
    }

    public void CreateNewDungeonButton()
    {
        if (!creatingDungeon)
        {
            CreateNewDungeon();
        }
        else
        {
            Debug.Log("Already creating New Dungeon");
        }
    }

    public void CreateNewDungeon()
    {
        currRoomToCheck = null;
        currentAmountOfRooms = 0;
        allspawnedRooms.Clear();
        allFinishedSpawningRooms.Clear();

        if (customDungeonParent != null)
        {
            DestroyDungeon();
        }

        creatingDungeon = true;
        SC_LoadingBar_DungeonGen.single.StartGenerating();

        customDungeonParent = new GameObject("Custom Dungeon");

        GameObject mainRoom = SC_RoomPooler.single.SpawnFromPool(startRoomType, transform.position, Quaternion.identity);
        //RoomCheckApply
        if (mainRoom != null)
        {
            mainRoom.SetActive(false);
            SpawnRoom(mainRoom);

            GetNextAvaialableRoom();
        }
    }

    public void DestroyDungeon()
    {
        CancelInvoke();
        Destroy(customDungeonParent);
    }

    public void GetNextAvaialableRoom()
    {
        for (int i = 0; i < allspawnedRooms.Count; i++)
        {
            currRoomToCheck = allspawnedRooms[i];
            if (!currRoomToCheck.fullyAttached)
            {
                StartCoroutine(CheckRoomAndSpawn(currRoomToCheck));
                break;
            }
            else
            {
                if (currRoomToCheck.roomType == AvailableSlots.Rooms)
                {
                    if (!allFinishedSpawningRooms.Contains(currRoomToCheck.gameObject))
                    {
                        allFinishedSpawningRooms.Add(currRoomToCheck.gameObject);
                    }
                }
                //allspawnedRooms.Remove(allspawnedRooms[i]);
            }
        }
        if (creatingDungeon)
        {
            if (!IsInvoking(nameof(RestartDungeon)))
            {
                Invoke(nameof(RestartDungeon),SpawnTimer());
            }
        }

    }
    public float SpawnTimer()
    {
        float timer;
        timer = spawnDelay + restartTimer;
        return timer;
    }

    public void RestartDungeon()
    {
        CreateNewDungeon();
    }

    public IEnumerator CheckRoomAndSpawn(SC_Room roomToCheck)
    {
        //Debug.Log(currentAmountOfRooms);
        if (currentAmountOfRooms < maxAmountOfRooms)
        {
            while (!CheckIfFullyAttached(roomToCheck))
            {
                for (int i = 0; i < roomToCheck.attachPoints.Length; i++)
                {
                    AttachPoint currAttachpoint = roomToCheck.attachPoints[i];
                    if (!currAttachpoint.attached)
                    {
                        GameObject newRoomObject = SC_RoomPooler.single.SpawnFromPool(currAttachpoint.nextSpawn, Vector3.zero, Quaternion.identity);
                        SC_Room newRoomScript = newRoomObject.GetComponent<SC_Room>();
                        AttachPoint[] newRoomAttachPoints = newRoomScript.attachPoints;
                        List<SpawnablePosAndRot> newRoomPosAndRot = newRoomScript.spawnablePosAndRots;

                        for (int iA = 0; iA < sRoomRots.presetRots.Length; iA++)
                        {
                            for (int iB = 0; iB < newRoomAttachPoints.Length; iB++)
                            {
                                SetRotSpawnedRoom(newRoomScript, iA);
                                SetOffsetSpawnedRoom(currAttachpoint, newRoomAttachPoints[iB], newRoomScript,iA);
                                foreach (AttachPoint newRoomAttachPoint in newRoomAttachPoints)
                                {
                                    if (IsOverlappingWithOtherAttachPoint(newRoomAttachPoint))
                                    {
                                        //Debug.Log("CanBeAttached");
                                        if (!IsCollidingWithOtherRoom(newRoomObject))
                                        {
                                            //Debug.Log(newRoomObject.name + " NewPosAndRot Found");
                                            SpawnablePosAndRot posAndRot = new SpawnablePosAndRot
                                            {
                                                pos = newRoomObject.transform.position,
                                                rot = newRoomObject.transform.rotation,
                                                room = newRoomObject
                                            };

                                            if (!newRoomPosAndRot.Contains(posAndRot))
                                            {
                                                newRoomPosAndRot.Add(posAndRot);
                                            }
                                        }
                                    }
                                }
                                yield return new WaitForSeconds(spawnDelay);
                            }
                        }

                        if (newRoomPosAndRot.Count > 0)
                        {
                            int randomPosAndRot = Random.Range(0, newRoomPosAndRot.Count - 1);
                            GameObject finalRoom = newRoomPosAndRot[randomPosAndRot].room;
                            finalRoom.transform.position = newRoomPosAndRot[randomPosAndRot].pos;
                            finalRoom.transform.rotation = newRoomPosAndRot[randomPosAndRot].rot;
                            newRoomPosAndRot.Clear();
                            SpawnRoom(finalRoom);
                        }
                        else
                        {
                            newRoomObject.SetActive(false);
                            currAttachpoint.mapWall = true;
                            currAttachpoint.wall.SetActive(true);
                            currAttachpoint.attached = true;
                        }
                    }
                }
            }

            if (!roomToCheck.fullyAttached && CheckIfFullyAttached(roomToCheck))
            {
                roomToCheck.fullyAttached = true;
                GetNextAvaialableRoom();
            }
        }
        else
        {
            CancelInvoke(nameof(RestartDungeon));
            Debug.Log("Dungeon Finished");
            creatingDungeon = false;
            SC_LoadingBar_DungeonGen.single.DoneGenerating();
            foreach (SC_Room test in allspawnedRooms)
            {
                foreach (AttachPoint points in test.attachPoints)
                {
                    if (!points.attached)
                    {
                        points.mapWall = true;
                        points.wall.SetActive(true);
                        SetAttachment(test);
                    }
                }
            }
        }

    }

    private void SetRotSpawnedRoom(SC_Room spawnedRoom,int index)
    {
        spawnedRoom.transform.rotation = Quaternion.Euler(sRoomRots.presetRots[index].rot);
    }

    private void SetOffsetSpawnedRoom(AttachPoint starterRoomAttachpoint, AttachPoint spawnedRoomAttachpoint, SC_Room spawnedRoom, int RotationIndex)
    {
        float multi = 1f;
        Vector3 spawnroomAttachpointPos = spawnedRoomAttachpoint.point.localPosition;
        Vector3 newOffset = Vector3.zero;

        if (spawnroomAttachpointPos.x != 0 && spawnroomAttachpointPos.z != 0)
        {
            multi = -1f;
        }

        if(RotationIndex == (int)Rots.XMin || RotationIndex == (int)Rots.ZPlus)
        {
            multi *= -1f;
        }

        if (RotationIndex == (int)Rots.XPlus || RotationIndex == (int)Rots.XMin)
        {
            newOffset = new Vector3(spawnroomAttachpointPos.z * multi,
              spawnroomAttachpointPos.y,
              spawnroomAttachpointPos.x * multi);
        }

        if (RotationIndex == (int)Rots.ZPlus || RotationIndex == (int)Rots.ZMin)
        {
            newOffset = new Vector3(spawnroomAttachpointPos.x * multi,
             spawnroomAttachpointPos.y,
             spawnroomAttachpointPos.z * multi);
        }

        spawnedRoom.transform.position = starterRoomAttachpoint.point.position + newOffset;
        //spawnedRoom.transform.position = starterRoom.transform.position + starterRoomAttachpoint.off + spawnedRoomAttachpoint.off;
    }

    private bool IsCollidingWithOtherRoom(GameObject pooledRoomObject)
    {
        SC_Room pooledRoom = pooledRoomObject.GetComponent<SC_Room>();
        string oldTag = pooledRoom.roomCollider.tag;
        pooledRoom.roomCollider.tag = "Ignore";

        Collider[] col = Physics.OverlapBox(pooledRoomObject.transform.position + pooledRoom.roomCollider.center,
            pooledRoom.roomCollider.size /2f,
            pooledRoomObject.transform.rotation,
            overlapLayer);

        bool isOverlapingWithOthers = IsOverlappingWithOthers(col, pooledRoomObject);
        pooledRoom.roomCollider.tag = oldTag;

        return isOverlapingWithOthers;
    }

    bool IsOverlappingWithOthers(Collider[] colliding, GameObject selfParent)
    {
        bool isOverlapingWithOthers = false;
        for (int i = 0; i < colliding.Length; i++)
        {
            if (!colliding[i].gameObject.CompareTag("Ignore") && colliding[i].GetComponentInParent<SC_Room>().gameObject != selfParent)
            {
                //Debug.Log(selfParent.name + " Is Colliding With: " + colliding[i].GetComponentInParent<SC_Room>().transform.name);
                isOverlapingWithOthers = true;
            }
        }
        return isOverlapingWithOthers;
    }
    public bool IsOverlappingWithOtherAttachPoint(AttachPoint currentAttachPoint)
    {
        bool canBeAttached = false;

        string oldTag = currentAttachPoint.AttachCollider.tag;
        currentAttachPoint.AttachCollider.tag = "Ignore";

        Collider[] col = Physics.OverlapBox(currentAttachPoint.point.position + currentAttachPoint.AttachCollider.center,
            currentAttachPoint.AttachCollider.size * 0.5f,
            Quaternion.identity,
            attachPointsLayer);

        if (IsOverlappingWithOthers(col, currentAttachPoint.point.gameObject))
        {
            canBeAttached = true;
        }

        currentAttachPoint.AttachCollider.tag = oldTag;

        return canBeAttached;
    }

    public void SpawnRoom(GameObject room)
    {
        //Debug.Log("Spawning");
        GameObject newRoom = Instantiate(room);
        SC_Room newRoomScript = newRoom.GetComponent<SC_Room>();

        if (newRoomScript.floors.Length > 0)
        {
            foreach (GameObject floor in newRoomScript.floors)
            {
                Color randomColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
                floor.GetComponent<Renderer>().material.color = randomColor;
            }
        }

        newRoom.SetActive(true);
        newRoomScript.isChecker = false;
        newRoom.transform.SetParent(customDungeonParent.transform);
        allspawnedRooms.Add(newRoomScript);
        
        //For Testing
        //room.SetActive(false);

        if (newRoomScript.roomType == AvailableSlots.Rooms)
        {
            currentAmountOfRooms++;
        }
        if(newRoomScript.roomType == AvailableSlots.MainRooms && newRoomScript.spawnPosEnemies.Count() > 0)
        {
            SC_GameManager.single.playerSpawnPos = newRoomScript.spawnPosEnemies.ToList();
        }
        CancelInvoke(nameof(RestartDungeon));
        SetAttachment(newRoomScript);
    }

    public void SetAttachment(SC_Room ownerRoom)
    {
        foreach (AttachPoint attach in ownerRoom.attachPoints)
        {
            AttachPoint ownersAttachPoint = attach;

            string oldTag = ownersAttachPoint.AttachCollider.tag;
            ownersAttachPoint.AttachCollider.tag = "Ignore";

            Collider[] col = Physics.OverlapBox(ownersAttachPoint.point.position + ownersAttachPoint.AttachCollider.center,
                ownersAttachPoint.AttachCollider.size * 0.5f,
                Quaternion.identity, attachPointsLayer);

            //Debug.Log(ownerRoom.name + " Has " + col.Length + " Collisions In Total ");
            //Debug.Log(ownerRoom.name + " Has " + overlapsWithSelf + " Collisions With Self");

            for (int iA = 0; iA < col.Length; iA++)
            {
                if (!col[iA].gameObject.CompareTag("Ignore"))
                {
                    //Debug.Log("NotOwner");
                    SC_Room otherRoom = col[iA].GetComponentInParent<SC_Room>();
                    for (int iB = 0; iB < otherRoom.attachPoints.Length; iB++)
                    {
                        AttachPoint otherAttachPoint = otherRoom.attachPoints[iB];
                        if(otherAttachPoint.AttachCollider == col[iA])
                        {
                            if (!otherAttachPoint.attached && !ownersAttachPoint.attached)
                            {
                                if (!otherRoom.isChecker)
                                {
                                    //Debug.Log("Attaching");
                                    Destroy(otherAttachPoint.wall);

                                    ownersAttachPoint.attached = true;
                                    ownersAttachPoint.attachedTo = otherRoom;

                                    //Debug.Log(otherAttachPoint.name + " Is Attached");

                                    otherAttachPoint.attached = true;
                                    otherAttachPoint.attachedTo = ownerRoom;
                                    otherAttachPoint.wall = ownersAttachPoint.wall;
                                    break;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            ownersAttachPoint.AttachCollider.tag = oldTag;
        }
    }
    bool CheckIfFullyAttached(SC_Room roomToCheck)
    {
        bool allAttached = true;
        AttachPoint[] attachPoints = roomToCheck.attachPoints;
        for (int i = 0; i < attachPoints.Length; i++)
        {
            if (!attachPoints[i].attached)
                allAttached = false;
        }
        return allAttached;
    }
}

public enum Rots
{
    ZPlus,
    ZMin,
    XPlus,
    XMin
}

[System.Serializable]
public class PresetRoomRots
{
    public Rots axisRot;
    public Vector3 rot;
}

[System.Serializable]
public class StandardRoomRots
{
    public PresetRoomRots[] presetRots;
}

public interface IPosCheck
{
    bool InsideBounds(Transform attachPoint);
}
