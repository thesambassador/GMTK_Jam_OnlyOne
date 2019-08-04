using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleLine : MonoBehaviour
{
	Vector2 startPos;
	public Transform lineTransform;
	public Transform sourceTransform;

    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
		lineTransform.position = sourceTransform.position;
		float dist = lineTransform.parent.position.x - lineTransform.position.x;
		print(dist);
		Vector3 curScale = lineTransform.localScale;
		curScale.y = dist;
		lineTransform.localScale = curScale;
    }
}
