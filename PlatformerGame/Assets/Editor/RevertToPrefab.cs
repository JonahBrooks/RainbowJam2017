using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RevertToPrefab : MonoBehaviour {

    [MenuItem("Tools/Revert to Prefab %r")]
    static void Revert()
    {
        var selection = Selection.gameObjects;

        if (selection.Length > 0)
        {
            for (var i = 0; i < selection.Length; i++)
            {
                PrefabUtility.RevertPrefabInstance(selection[i]);
            }
        }
        else
        {
            Debug.Log("Cannot revert to prefab - nothing selected");
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
