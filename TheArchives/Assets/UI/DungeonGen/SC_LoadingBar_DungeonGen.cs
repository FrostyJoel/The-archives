using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_LoadingBar_DungeonGen : SC_LoadSettingsBase
{
    public static SC_LoadingBar_DungeonGen single;

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

    // Start is called before the first frame update
    void Start()
    {
        AssignColors();
        loadingBar.value = SC_RoomManager.single.currentAmountOfRooms;
        loadingBar.maxValue = SC_RoomManager.single.maxAmountOfRooms;
    }

    // Update is called once per frame
    public void Update()
    {
        if (SC_RoomManager.single.creatingDungeon)
        {
            loadingBar.value = SC_RoomManager.single.currentAmountOfRooms;

            if (loadingBar.value == 0)
            {
                ChangeColor(startColorBackGround, startColorFill);
            }
            else if (loadingBar.value > loadingBar.maxValue / 2)
            {
                ChangeColor(midWayColorBackGround, midWayColorFill);
            }
        }
    }

    public override void StartGenerating()
    {
        loadingBar.value = SC_RoomManager.single.currentAmountOfRooms;
        StartCoroutine(SpinningAnimationDungeon());
        StartCoroutine(GeneratingTextDungeon());
    }

    public override void DoneGenerating()
    {
        loadingBar.value = SC_RoomManager.single.currentAmountOfRooms;
        ChangeColor(doneColorBackGround, doneColorBackGround);
        base.DoneGenerating();
    }
}
