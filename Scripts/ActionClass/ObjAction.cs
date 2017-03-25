using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.MyGameFramework;


namespace Com.MyGameFramework
{
    public class ObjAction : ScriptableObject
    {

		public bool enable = true;
		public bool destroy = false;

		public GameObject gameObject;
        public Transform transform;
        public ActionCallback whoToNotify;

        public virtual void Start()
        {
            throw new System.NotImplementedException();
        }

        public virtual void Update()
        {
            throw new System.NotImplementedException();
        }
    }

    
}