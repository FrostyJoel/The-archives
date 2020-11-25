using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SC_LoadingBar_DungeonGen : MonoBehaviour
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

    [Header("LoadingBarSettings")]
    public Slider loadingBar;
    public Image backGroundLoadingBar;
    public Image fillLoadingBar;

    [Header("LoadingBarColors")]
    public Color startColorBackGround;
    public Color startColorFill;
    public Color midWayColorBackGround;
    public Color midWayColorFill;
    public Color doneColorBackGround;
    public Color doneColorFill;

    [Header("Speed in Seconds")]
    public float spinningSpeed;
    public float textDelaySpeed;

    [Header("Rest")]
    public TextMeshProUGUI currentStatetext;
    public Image spinning;
    

    // Start is called before the first frame update
    void Start()
    {
        loadingBar.value = SC_RoomManager.single.currentAmountOfRooms;
        loadingBar.maxValue = SC_RoomManager.single.maxAmountOfRooms;
    }

    // Update is called once per frame
    void Update()
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

    private void ChangeColor(Color backGroundColor, Color fillColor)
    {
        backGroundLoadingBar.color = backGroundColor;
        fillLoadingBar.color = fillColor;
    }

    public void StartGenerating()
    {
        loadingBar.value = SC_RoomManager.single.currentAmountOfRooms;
        StartCoroutine(SpinningAnimation());
        StartCoroutine(GeneratingText());
    }

    public void DoneGenerating()
    {
        StopAllCoroutines();
        loadingBar.value = SC_RoomManager.single.currentAmountOfRooms;

        ChangeColor(doneColorBackGround, doneColorBackGround);
        currentStatetext.text = "Done Generating";
    }

    public IEnumerator SpinningAnimation()
    {
        while (loadingBar.value < loadingBar.maxValue)
        {
            if(spinning.transform.rotation == Quaternion.Euler(new Vector3(0, 0, -135)))
            {
                spinning.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            Vector3 currentRot = spinning.transform.rotation.eulerAngles;
            Vector3 newRot = currentRot + new Vector3(0, 0, -45);
            spinning.transform.rotation = Quaternion.Euler(newRot);
            yield return new WaitForSeconds(spinningSpeed);
        }
    }

    public IEnumerator GeneratingText()
    {
        while (loadingBar.value < loadingBar.maxValue)
        {
            currentStatetext.text = "Now Generating";
            yield return new WaitForSeconds(textDelaySpeed);
            currentStatetext.text = "Now Generating.";
            yield return new WaitForSeconds(textDelaySpeed);
            currentStatetext.text = "Now Generating..";
            yield return new WaitForSeconds(textDelaySpeed);
            currentStatetext.text = "Now Generating...";
            yield return new WaitForSeconds(textDelaySpeed);
        }
    }
}
