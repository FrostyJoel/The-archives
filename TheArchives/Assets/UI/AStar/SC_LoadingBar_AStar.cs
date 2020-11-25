using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_LoadingBar_AStar : SC_LoadSettingsBase
{
    public static SC_LoadingBar_AStar single;

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
    }
    
    public override void StartGenerating()
    {
        StartCoroutine(SpinningAnimationAstar());
        StartCoroutine(GeneratingTextAstar());
    }

    public override void DoneGenerating()
    {
        base.DoneGenerating();
    }

    public void FailedToGenerate()
    {
        base.DoneGenerating();
        currentStatetext.text = "Creating Path is Impossible";
        SC_Grid_Manager.single.Invoke(nameof(SC_Grid_Manager.single.CreateNewPath), SC_Grid_Manager.single.restartTimer);
    }

    protected IEnumerator SpinningAnimationAstar()
    {
        while (true)
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

    protected IEnumerator GeneratingTextAstar()
    {
        while (true)
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
