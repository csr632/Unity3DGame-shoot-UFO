using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFOController
{
    public UFOAttributes attr;
    GameObject gameObject;
    UFOScript script;

    public UFOController(GameObject _gameObject)
    {
        gameObject = _gameObject;
        script = _gameObject.AddComponent<UFOScript>();
        script.ctrl = this;
    }

    public void appear()
    {
        gameObject.SetActive(true);
    }
    public void disappear()
    {
        gameObject.SetActive(false);
    }

    public GameObject getObj() {
        return gameObject;
    }
    
    public void setAttr(UFOAttributes _attr) {
        attr = _attr;
        gameObject.transform.localScale = gameObject.transform.localScale*_attr.scale;
        foreach(Renderer renderer in gameObject.GetComponentsInChildren<Renderer> ()) {
            renderer.material.color = _attr.color;
        } 
    }

    public void setPosition(Vector3 pos) {
        Rigidbody rigi = gameObject.GetComponent<Rigidbody>();
        if (rigi) {
            rigi.MovePosition(pos);
        }
        else {
            gameObject.transform.position = pos;
        }
    }

    public void crash() {
        Singleton<FirstController>.Instance.UFOCrash(this);
    }
}
