using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Gridtile : MonoBehaviour
{
    public int xPos; 
    public int zPos;
    public float distanceFromStart, distanceToEnd, totalDistance;
    public bool spawnedOn;
    public SC_Gridtile previousTile;

    private void Awake()
    {
        SetColor(Color.white);
    }

    public void CalcuteDistance(SC_Gridtile target,SC_Gridtile previous)
    {
        previousTile = previous;
        Vector3 thisTile = new Vector3(xPos, 0, zPos);
        distanceFromStart = Vector3.Distance(thisTile, new Vector3(previous.xPos, 0f, previous.zPos));
        distanceToEnd = Vector3.Distance(thisTile, new Vector3(target.xPos, 0f, target.zPos));
        totalDistance = distanceFromStart += distanceToEnd;
    }

    public void SetColor(Color c)
    {
        Renderer r = GetComponent<Renderer>();
        r.material.color = c;
    }
}
