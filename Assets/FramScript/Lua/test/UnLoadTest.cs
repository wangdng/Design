using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnLoadTest : MonoBehaviour {

	public void UnLoad()
    {
        ILoadManager.instance.UnLoadSingleBundle("scene01", "scene01/uimain.unity3d");
    }
}
