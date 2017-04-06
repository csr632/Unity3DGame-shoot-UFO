using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ActionManagerTarget {
	void addAction(GameObject gameObj, Dictionary<string, object> option);

	void addActionForArr(GameObject[] Arr, Dictionary<string, object> option);

	void addActionForArr(UFOController[] Arr, Dictionary<string, object> option);

	void removeActionOf(GameObject obj, Dictionary<string, object> option);
	
}
