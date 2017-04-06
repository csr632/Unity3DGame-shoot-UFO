using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsActionManager : MonoBehaviour {

	public void addForce(GameObject gameObj, Vector3 force) {
		ConstantForce originalForce = gameObj.GetComponent<ConstantForce>();
		if (originalForce) {
			originalForce.enabled = true;
			originalForce.force = force;
		} else {
			gameObj.AddComponent<Rigidbody>().useGravity = false;
			gameObj.AddComponent<ConstantForce>().force = force;
		}
	}

	public void removeForce(GameObject gameObj) {
		gameObj.GetComponent<ConstantForce>().enabled = false;
	}
}
