using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Astar_Pathfinder
{
    public List<SC_Gridtile> todo = new List<SC_Gridtile>();
    public List<SC_Gridtile> done = new List<SC_Gridtile>();
    public List<SC_Gridtile> path = new List<SC_Gridtile>();
    SC_Gridtile smallestDis;
    SC_Gridtile temp;

    public IEnumerator StartSearch(SC_Gridtile startPos, SC_Gridtile endPos)
    {
        todo = new List<SC_Gridtile>();
        done = new List<SC_Gridtile>();
        todo.Add(startPos);
        smallestDis = startPos;
        smallestDis.CalcuteDistance(endPos, startPos);
        while (todo.Count > 0)
        {
            smallestDis = todo[0];
            for (int i = 0; i < todo.Count; i++)
            {
                if (smallestDis != null && todo[i].totalDistance <= smallestDis.totalDistance)
                {
                    if (temp != todo[i])
                    {
                        temp = todo[i];
                        smallestDis = temp;
                    }
                }
            }
            done.Add(smallestDis);
            done.Add(temp);
            todo.Remove(temp);

            if (smallestDis == endPos)
            {
                endPos.CalcuteDistance(endPos, smallestDis.previousTile);
                done.Reverse();
                for (int ic = 0; ic < done.Count; ic++)
                {
                    if (done[ic] == smallestDis.previousTile && !path.Contains(done[ic]))
                    {
                        path.Add(done[ic]);
                        done[ic].SetColor(Color.blue);
                        smallestDis = smallestDis.previousTile;
                        yield return new WaitForSeconds(SC_Grid_Manager.single.timeDelayBetweenSpawns);
                    }
                } 
                if (smallestDis == startPos)
                {
                    path.Reverse();
                    SC_Grid_Manager.single.createingPath = false;
                    break;
                }

            }

            SC_Gridtile[,] grid = SC_Grid_Manager.single.grid;
            List<SC_Gridtile> neighbour = new List<SC_Gridtile>();

            int k, j;
            j = smallestDis.xPos;
            k = smallestDis.zPos;

            //Left
            if (j - 1 >= 0 && j != 0)
            {
                if (!grid[j - 1, k].spawnedOn)
                {
                    neighbour.Add(grid[j - 1, k]);
                }
            }

            //Right
            if (j + 1 < grid.GetLength(0) && j != grid.GetLength(0) - 1)
            {
                if (!grid[j + 1, k].spawnedOn)
                {
                    neighbour.Add(grid[j + 1, k]);
                }
            }

            //Below
            if (k - 1 >= 0 && k != 0)
            {
                if (!grid[j, k - 1].spawnedOn)
                {
                    neighbour.Add(grid[j, k - 1]);
                }
            }

            //Above
            if (k + 1 < grid.GetLength(1) && k != grid.GetLength(1) - 1)
            {
                if (!grid[j, k + 1].spawnedOn)
                {
                    neighbour.Add(grid[j, k + 1]);
                }
            }

            foreach (SC_Gridtile neigbouringGrid in neighbour)
            {
                if (!done.Contains(neigbouringGrid) && !todo.Contains(neigbouringGrid))
                {
                    neigbouringGrid.CalcuteDistance(endPos, smallestDis);
                    todo.Add(neigbouringGrid);
                }
            }
        }
        if (todo.Count <= 0)
        {
            SC_Grid_Manager.single.createingPath = false;
            Debug.LogError("Path Could not be Created");
        }
    }
}
