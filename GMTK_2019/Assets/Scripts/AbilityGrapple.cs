using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityGrapple : PlayerAbility
{
	public float MaxGrappleDist;
	public float GrappleSpeed;
	public float GrappleExtendSpeed = .75f;
	public LayerMask GrappleCollideMask;

	public GrappleLine GrappleEffectPrefab;

	protected override void InternalPickup() {
		
	}

	protected override void InternalUpdate() {

	}

	protected override void InternalDrop() {

	}
	protected override void InternalUse() {
		Vector2 direction = Vector2.right;
		if (!_player.facingRight) direction = Vector2.left;

		RaycastHit2D result;
		if (_player.boxCaster.GetFirstBoxcastHit(direction, MaxGrappleDist, GrappleCollideMask, out result)) {
			Grappleable proposedGrapple = result.transform.GetComponent<Grappleable>();
			if(proposedGrapple != null) {
				_player.StartCoroutine(GrappleToPoint(result.point, true));
			}
			else {
				_player.StartCoroutine(GrappleToPoint(result.point, false));
			}
		}
		else {
			_player.StartCoroutine(GrappleToPoint(_player.transform.position + (Vector3)(direction * MaxGrappleDist), false));
		}
	}

	public IEnumerator GrappleToPoint(Vector2 point, bool actuallyGrapple) {
		_player.SoundPlayer.PlaySound(PlayerSound.GrappleShoot);
		_player.PausePlayerControls();
		Vector2 startPosition = _player.transform.position;

		//do grapple extension animation
		GrappleLine grappleObject = Instantiate(GrappleEffectPrefab);
		grappleObject.transform.position = _player.transform.position;
		grappleObject.sourceTransform = _player.transform;

		Vector2 curPos = startPosition;
		while (Vector2.Distance(grappleObject.transform.position, point) > .01) {
			curPos = Vector2.MoveTowards(grappleObject.transform.position, point, GrappleExtendSpeed * Time.deltaTime);
			grappleObject.transform.position = curPos;
			yield return null;
		}
		grappleObject.transform.position = point;

		//now pull player to the right point
		if (actuallyGrapple) {
			_player.SoundPlayer.PlaySound(PlayerSound.GrappleHitPull);
			Vector2 endPosition = point;
			endPosition.x -= .35f * Mathf.Sign(endPosition.x - startPosition.x);
			Debug.DrawLine(startPosition, endPosition);
			curPos = startPosition;
			while (Vector2.Distance(_player.transform.position, endPosition) > .01) {
				curPos = Vector2.MoveTowards(_player.transform.position, endPosition, GrappleSpeed * Time.deltaTime);
				_player.transform.position = curPos;
				yield return null;
			}
			_player.transform.position = endPosition;
		}
		else {
			_player.SoundPlayer.PlaySound(PlayerSound.GrappleHitNoPull);
		}
		Destroy(grappleObject.gameObject);

		_player.ResumePlayerControls();

	}
}
