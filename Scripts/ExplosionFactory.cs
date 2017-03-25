using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionFactory : MonoBehaviour
{

    protected Queue<GameObject> freeQueue = new Queue<GameObject>();
    protected List<GameObject> usingList = new List<GameObject>();
    public GameObject original;


    void Start()
    {
        original = GameObject.Instantiate(Resources.Load("Explosion")) as GameObject;
        original.SetActive(false);
    }
    public void explodeAt(Vector3 pos)
    {
        GameObject newExplosion;
        if (freeQueue.Count == 0)
        {
            newExplosion = GameObject.Instantiate(original);
            newExplosion.AddComponent<SelfRecycle>().factory = this;
        }
        else
        {
            newExplosion = freeQueue.Dequeue();
        }
        usingList.Add(newExplosion);

        SelfRecycle selfRecycle = newExplosion.GetComponent<SelfRecycle>();
        selfRecycle.startTimer(1.2F);

        newExplosion.SetActive(true);
        newExplosion.transform.position = pos;
    }

    public void recycle(GameObject explosion)
    {
        explosion.SetActive(false);
        freeQueue.Enqueue(explosion);
    }
}
