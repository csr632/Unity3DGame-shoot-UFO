using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Com.MyGameFramework;

namespace Com.MyGameFramework {
    public interface ActionCallback {
        void actionDone(ObjAction source);
    }
}