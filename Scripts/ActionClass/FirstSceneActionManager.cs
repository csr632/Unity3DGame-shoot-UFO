using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.MyGameFramework;


public class FirstSceneActionManager : ActionManager
{
    public void addRandomAction(GameObject gameObj, float speed) {
        Vector3 currentPos = gameObj.transform.position;
        Vector3 randomTarget1 = new Vector3(
            Random.Range(currentPos.x-7, currentPos.x+7),
            Random.Range(1, currentPos.y+5),
            Random.Range(currentPos.z-7, currentPos.z+7)
            );
        MoveToAction moveAction1 = MoveToAction.getAction(randomTarget1, speed);

        Vector3 randomTarget2 = new Vector3(
            Random.Range(currentPos.x-7, currentPos.x+7),
            Random.Range(1, currentPos.y+5),
            Random.Range(currentPos.z-7, currentPos.z+7)
            );
        MoveToAction moveAction2 = MoveToAction.getAction(randomTarget2, speed);

        SequenceAction sequenceAction = SequenceAction.getAction(new List<ObjAction>{moveAction1, moveAction2}, -1);

        addAction(gameObj, sequenceAction, this);
    }

    public void addRandomActionForArr(UFOController[] arr, float speed) {
        for (int i = 0; i < arr.Length; i++) {
            addRandomAction(arr[i].getObj(), speed);
        }
    }
}
