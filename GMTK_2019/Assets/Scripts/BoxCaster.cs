using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCaster : MonoBehaviour {
	public float BoxcastWidth = 1;

	private Vector2 _boxCastSize;
	private RaycastHit2D[] _raycastResults;


	// Start is called before the first frame update
	void Start() {
		_boxCastSize = new Vector2(BoxcastWidth, BoxcastWidth);
		_raycastResults = new RaycastHit2D[20];
	}

	// Update is called once per frame
	void Update() {

	}

	public bool BoxcastInDirection(Vector2 direction, float maxDist, LayerMask targetLayers) {
		int hits = Physics2D.BoxCastNonAlloc(transform.position, _boxCastSize, 0, direction, _raycastResults, maxDist, targetLayers.value);
		if (hits > 0) {
			for (int i = 0; i < hits; i++) {
				if (_raycastResults[i].transform != this.transform) {
					return true;
				}
			}
			return false;
		}
		else {
			return false;
		}
	}

	public bool GetFirstBoxcastHit(Vector2 direction, float maxDist, LayerMask targetLayers, out RaycastHit2D location) {
		int hits = Physics2D.BoxCastNonAlloc(transform.position, _boxCastSize, 0, direction, _raycastResults, maxDist, targetLayers.value);
		if (hits > 0) {
			for (int i = 0; i < hits; i++) {
				if(_raycastResults[i].transform != this.transform) {
					location = _raycastResults[i];
					return true;
				}
			}

			location = new RaycastHit2D();
			return false;
		}
		else {
			location = new RaycastHit2D();
			return false;
		}
	}
}

