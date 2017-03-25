using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.MyGameFramework;

public class UFOScript: MonoBehaviour
{
    public UFOController ctrl;

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject g = transform.GetChild(i).gameObject;
            g.AddComponent<UFOScript>().ctrl = ctrl;
        }
    }
    // public void onClick()
    // {
    //     // Debug.Log(gameObject.name);
    //     ctrl.onClick();
    // }

    
}
