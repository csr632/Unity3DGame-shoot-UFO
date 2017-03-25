using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.MyGameFramework;

namespace Com.MyGameFramework {
	
	public class Director : System.Object {
		private static Director _instance;
		public SceneController currentSceneController { get; set; }

		public static Director getInstance() {
			if (_instance == null) {
				_instance = new Director ();
			}
			return _instance;
		}
	}

	public interface SceneController {
		void loadResources ();
	}
}