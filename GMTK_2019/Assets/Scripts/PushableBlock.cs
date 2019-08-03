using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

public class PushableBlock : MonoBehaviour {
	public AnimationCurve PushSpeedCurve;
	public bool isFalling = false;
	public bool isPushing = false;
	public bool CanPush{
		get{ return !isFalling && !isPushing; }
	}

	private float _pushTimer = -1;
	public float PushTime = .1f;
	public float HalfBoxSize = .5f;
	public LayerMask GroundLayers;

	public BoxCaster BoxcastHelper;

	// Start is called before the first frame update
	void Start()
    {
		CheckGround();
		ResetPushTimer();

	}

    // Update is called once per frame
    void Update()
    {
		

	}

	[Button("BoxcastCheck")]
	void CheckForRightThings() {
		RaycastHit2D result;
		if (BoxcastHelper.GetFirstBoxcastHit(Vector2.right, 10, GroundLayers, out result)) {
			print(result.transform.gameObject.name);
		}
	}

	public void CheckGround() {
		RaycastHit2D result;
		if(BoxcastHelper.GetFirstBoxcastHit(Vector2.down, 100, GroundLayers, out result)) {
			float dist = transform.position.y - result.point.y - HalfBoxSize;
			print(dist);
			if(dist > 0) {
				StartCoroutine(Fall(dist));
			}
		}
	}

	public void ResetPushTimer() {
		_pushTimer = PushTime;
	}

	
	public void TryPush(bool pushingRight) {
		if (CanPush) {
			_pushTimer -= Time.deltaTime;
			if (_pushTimer <= 0) {
				Vector2 direction = Vector2.right;
				if (!pushingRight) direction = Vector2.left;

				if (!BoxcastHelper.BoxcastInDirection(direction, 1, GroundLayers)) {
					StartCoroutine(Push(direction.x));
				}
			}
		}
	}

	[Button("TestPushRight")]
	public void PushRight() {
		TryPush(true);
	}

	public IEnumerator Push(float pushDist) {
		isPushing = true;
		Vector2 startPosition = transform.position;
		Vector2 endPosition = startPosition;
		endPosition.x += pushDist;
		float time = 0;
		Vector2 curPos = startPosition;
		while (time < PushSpeedCurve.keys[PushSpeedCurve.length-1].time) {
			float newX = Mathf.Lerp(startPosition.x, endPosition.x, PushSpeedCurve.Evaluate(time));
			curPos.x = newX;
			transform.position = curPos;

			time += Time.deltaTime;
			yield return null;
		}
		transform.position = endPosition;

		isPushing = false;
		ResetPushTimer();
		CheckGround();
	}

	public IEnumerator Fall(float fallDist) {
		print("falling");
		isFalling = true;
		Vector2 startPosition = transform.position;
		Vector2 endPosition = startPosition;
		endPosition.y -= fallDist;
		float speed = 0;
		while(Vector2.Distance(transform.position, endPosition) > 0) {
			speed -= Physics2D.gravity.y * Time.deltaTime;
			transform.position = Vector2.MoveTowards(transform.position, endPosition, speed * Time.deltaTime);
			yield return null;
		}

		isFalling = false;
	}
	
}
