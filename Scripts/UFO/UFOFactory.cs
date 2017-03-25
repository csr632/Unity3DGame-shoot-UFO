using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.MyGameFramework;

public class UFOFactory : MonoBehaviour
{
    Queue<UFOController> freeQueue; // 储存空闲状态的UFO
    List<UFOController> usingList;  // 储存正在使用的UFO

    GameObject originalUFO; // UFO的原型，以后创建UFO就克隆这个对象

    int count = 0;
    void Awake()
    {
        freeQueue = new Queue<UFOController>();
        usingList = new List<UFOController>();

        originalUFO = Instantiate(Resources.Load("ufo", typeof(GameObject))) as GameObject;
        originalUFO.SetActive(false);
    }

    public UFOController produceUFO(UFOAttributes attr)
    {
        UFOController newUFO;
        if (freeQueue.Count == 0)       // 如果没有UFO空闲，则克隆一个对象
        {
            // print("Instantiate");
            GameObject newObj = GameObject.Instantiate(originalUFO);
            newUFO = new UFOController(newObj);
            newObj.transform.position += Vector3.forward * Random.value * 5;
            count++;
        }
        else                            // 如果有UFO空闲，则取出这个UFO
        {
            newUFO = freeQueue.Dequeue();
        }
        newUFO.setAttr(attr);           // 将UFO的颜色速度大小设置成参数指定的样子
        usingList.Add(newUFO);          // 将UFO加入使用中的队列
        newUFO.appear();
        return newUFO;
    }

    public UFOController[] produceUFOs(UFOAttributes attr, int n)
    {
        // 一次性产生n个UFO

        UFOController[] arr = new UFOController[n];
        for (int i = 0; i < n; i++)
        {
            arr[i] = produceUFO(attr);
        }
        return arr;
    }

    public void recycle(UFOController UFOCtrl)
    {
        // 回收一个UFO，将其加入空闲队列
        UFOCtrl.disappear();
        usingList.Remove(UFOCtrl);
        freeQueue.Enqueue(UFOCtrl);
    }

    public void recycleAll()
    {
        while(usingList.Count != 0)
        {
            recycle(usingList[0]);
        }
    }

    public List<UFOController> getUsingList()
    {
        return usingList;
    }
}
