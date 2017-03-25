using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRecycle : MonoBehaviour {
	// 这个类挂载在爆炸对象上，让爆炸对象过一段时间以后自动回收自己
	public ExplosionFactory factory;

	public void startTimer(float time) {
		Invoke("selfRecycle", time);
	}

	private void selfRecycle() {
		factory.recycle(gameObject);
	}
}
