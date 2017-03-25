using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T: MonoBehaviour {
	// 单例模式，可以在任何代码中轻松获取到单例对象
	// 比如，在任何位置使用Debug.Log(Singleton<UFOFactory>.Instance);都能打印出UFOFactory对象
	private static T instance;

	public static T Instance {
		get {
			if (instance == null) {
				instance = (T)Object.FindObjectOfType(typeof(T));
				if (instance == null) {
					Debug.LogError("Cant find instance of "+typeof(T));
				}
			}
			return instance;
		}
	}
}
