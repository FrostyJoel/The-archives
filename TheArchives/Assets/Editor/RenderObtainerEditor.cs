using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RandomColorPicker))]
public class RenderObtainerEditor : Editor
{
#if UNITY_EDITOR
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RandomColorPicker myScript = (RandomColorPicker)target;    
        if(GUILayout.Button("Get Renderes"))
        {
            myScript.ObtainRenderes();
        }

        if(GUILayout.Button("Start ChangeGame"))
        {
            myScript.StartSwap();
        }

        if (GUILayout.Button("Stop ChangeGame"))
        {
            myScript.StopSwap();
        }
    }
#endif
}
