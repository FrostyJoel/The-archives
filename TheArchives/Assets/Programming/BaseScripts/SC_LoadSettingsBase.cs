using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SC_LoadSettingsBase : MonoBehaviour
{
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

    public void AssignColors()
    {
        startColorBackGround = new Color32(190, 50, 50, 255);
        startColorFill = new Color32(230, 34, 34, 255);

        midWayColorBackGround = new Color32(202, 111, 59, 255);
        midWayColorFill = new Color32(255, 141, 77, 255);

        doneColorBackGround = new Color32(50, 190, 50, 255);
        doneColorFill = new Color32(81, 219, 81, 255);
    }

    protected void ChangeColor(Color backGroundColor, Color fillColor)
    {
        backGroundLoadingBar.color = backGroundColor;
        fillLoadingBar.color = fillColor;
    }

    public virtual void StartGenerating()
    {
       
    }

    public virtual void DoneGenerating()
    {
        StopAllCoroutines();
        currentStatetext.text = "Done Generating";
    }

    protected IEnumerator SpinningAnimationDungeon()
    {
        while (loadingBar.value < loadingBar.maxValue)
        {
            if (spinning.transform.rotation == Quaternion.Euler(new Vector3(0, 0, -135)))
            {
                spinning.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            Vector3 currentRot = spinning.transform.rotation.eulerAngles;
            Vector3 newRot = currentRot + new Vector3(0, 0, -45);
            spinning.transform.rotation = Quaternion.Euler(newRot);
            yield return new WaitForSeconds(spinningSpeed);
        }
    }

    protected IEnumerator GeneratingTextDungeon()
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
