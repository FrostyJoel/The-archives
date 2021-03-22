using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColorPicker : MonoBehaviour
{
    [Header ("Color Changer")]
    [SerializeField] private MeshRenderer[] render;
    [SerializeField] private Material[] allMats;
    [SerializeField] private float colorTimeDelay = 2f;
    [Header("Jump Changer")]
    [SerializeField] private float upDraftAmount = 1f;
    [SerializeField] private float jumpTimeDelay = 3f;
    
    public void ObtainRenderes()
    {
        render = GetComponentsInChildren<MeshRenderer>();
    }

    public void StartSwap()
    {
        if (render.Length > 0)
        {
            StartCoroutine(ColorChange());
            StartCoroutine(Uplift());
        }
        else
        {
            Debug.Log("No Renders Assgined");
        }
    }

    public void StopSwap()
    {
        StopAllCoroutines();
    }

    IEnumerator Uplift()
    {
        while (true)
        {
            foreach (MeshRenderer r in render)
            {
                int chance = Random.Range(0, 2);
                if(chance == 0)
                {
                    Vector3 upLift = new Vector3(r.transform.position.x, r.transform.position.y + upDraftAmount, r.transform.position.z);
                    r.transform.position = upLift;
                }
            }
            yield return new WaitForSeconds(jumpTimeDelay);
        }
    }

    IEnumerator ColorChange()
    {
        while (true)
        {
            foreach (MeshRenderer r in render)
            {
                Material randomMat = allMats[Random.Range(0, allMats.Length)];
                r.material = randomMat;
            }
            yield return new WaitForSeconds(colorTimeDelay);
        }
    }
}
