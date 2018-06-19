using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFllow : MonoBehaviour {

    Transform Target;

    public Vector3 Distance;

    private Vector3 Speed = Vector3.zero;

    public float MoveTime = 0.5f;

	void Start ()
    {
        Target = GameObject.FindWithTag("Player").transform;
    }
	
	void LateUpdate ()
    {
		if(Target !=null)
        {
           transform.position = Vector3.SmoothDamp(transform.position, Target.position + Distance, ref Speed, MoveTime);
        }
	}
}
