using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManagerAdapter: ActionManagerTarget {
	FirstSceneActionManager normalAM;
	PhysicsActionManager PhysicsAM;


	public ActionManagerAdapter(GameObject main) {
		normalAM = main.AddComponent<FirstSceneActionManager>();
		PhysicsAM = main.AddComponent<PhysicsActionManager>();
	}

	public void addAction(GameObject gameObj, Dictionary<string, object> option) {
		if (option["whichActionManager"].Equals(0))
		//	use normalAM
		{
			Debug.Log("use normalAM");
			normalAM.addRandomAction(gameObj, (float)option["speed"]);
		}

		else
		//	use PhysicsAM
		{
			Debug.Log("use PhysicsAM");
			PhysicsAM.addForce(gameObj, (Vector3)option["force"]);
		}
	}

	public void addActionForArr(GameObject[] Arr, Dictionary<string, object> option) {
		if (option["whichActionManager"].Equals(0))
		//	use normalAM
		{
			Debug.Log("use normalAM");
			float speed = (float)option["speed"];
			foreach (GameObject gameObj in Arr) {
				normalAM.addRandomAction(gameObj, speed);
			}
		}

		else
		//	use PhysicsAM
		{
			Debug.Log("use PhysicsAM");
			Vector3 force = (Vector3)option["force"];
			foreach (GameObject gameObj in Arr) {
				PhysicsAM.addForce(gameObj, force);
			}
		}
	}

	public void addActionForArr(UFOController[] Arr, Dictionary<string, object> option) {
		if (option["whichActionManager"].Equals(0))
		//	use normalAM
		{
			Debug.Log("use normalAM");
			float speed = (float)option["speed"];
			foreach (UFOController ctrl in Arr) {
				normalAM.addRandomAction(ctrl.getObj(), speed);
			}
		}

		else
		//	use PhysicsAM
		{
			Debug.Log("use PhysicsAM");
			Vector3 force = (Vector3)option["force"];
			foreach (UFOController ctrl in Arr) {
				PhysicsAM.addForce(ctrl.getObj(), force);
			}
		}
	}

	public void removeActionOf(GameObject gameObj, Dictionary<string, object> option){
		if (option["whichActionManager"].Equals(0))
		//	use normalAM
		{
			Debug.Log("use normalAM");
			normalAM.removeActionOf(gameObj);
		}

		else
		//	use PhysicsAM
		{
			Debug.Log("use PhysicsAM");
			PhysicsAM.removeForce(gameObj);
		}
	}
	
}
