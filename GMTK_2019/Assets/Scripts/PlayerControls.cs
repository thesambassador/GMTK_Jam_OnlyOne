using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;
using Rewired;

public class ActionNames {
	public static string HorizontalMovement = "HorizontalMovement";
	public static string Jump = "Jump";
	public static string Ability = "Ability";
	public static string VerticalMovement = "VerticalMovement";
}

public class PlayerControls : MonoBehaviour
{
	Player _controls;
	Rigidbody2D _rb;
	Animator _animator;
	Vector2 _boxcastSize;
	RaycastHit2D[] _raycastResults;
	float _coyoteTimer = -1;
	float _jumpCooldownTimer = -1;
	Vector2 _horizontalMovementForce;
	public SpriteRenderer MainCharSprite;

	[Header("Config Values")]
	[Header("HorizontalMovement")]
	public AnimationCurve AccelerationVelocityCurveGround;
	public AnimationCurve AccelerationVelocityCurveAir;
	public float MaxAcceleration;
	public float MaxSpeed;

	[Header("Jump Stuff")]
	public float JumpInitialVelocity;
	public float JumpReleaseDamping;
	public float JumpHoldForce;

	[Header("OnGround Stuff")]
	public float CoyoteTime;
	public float JumpCooldown;
	public float OnGroundRaycastDist;
	public LayerMask GroundLayers;
	public float BoxcastWidth = 1;

	[Header("Live Values")]
	public bool OnGround = false;
	public bool CanJump = false;

    // Start is called before the first frame update
    void Start()
    {
		_controls = ReInput.players.GetPlayer(0);
		_rb = GetComponent<Rigidbody2D>();
		_animator = GetComponentInChildren<Animator>();

		_boxcastSize = new Vector2(BoxcastWidth, BoxcastWidth);
		_raycastResults = new RaycastHit2D[20];
		_horizontalMovementForce = new Vector2(0, 0);
	}

    // Update is called once per frame
    void Update()
    {
		ProcessOnGround();
		ProcessHorizontalMovement();
		ProcessJump();
    }

	void ProcessHorizontalMovement() {
		AnimationCurve accelCurve = AccelerationVelocityCurveGround;
		//if (!OnGround) { accelCurve = AccelerationVelocityCurveAir; }

		float xMovement = _controls.GetAxis(ActionNames.HorizontalMovement);

		float targetVel = xMovement * MaxSpeed;
		float velDiff = targetVel - _rb.velocity.x;
		float normalizedVelDiff = Mathf.Abs(velDiff) / MaxSpeed;

		float accel = accelCurve.Evaluate(normalizedVelDiff / MaxSpeed) * MaxAcceleration * Mathf.Sign(velDiff);
		_horizontalMovementForce.x = accel;

		print("targetvel: " + targetVel + " -- normalizedVelDiff " + normalizedVelDiff + " -- accel: " + accel);
		
		//animation shit:
		if(_rb.velocity.x > .05) {
			MainCharSprite.flipX = false;
			_animator.SetBool("Walking", true);
		}
		else if(_rb.velocity.x < -.05 ){
			MainCharSprite.flipX = true;
			_animator.SetBool("Walking", true);
		}
		else {
			_animator.SetBool("Walking", false);
		}

	}

	void ProcessJump() {
		_jumpCooldownTimer -= Time.deltaTime;
		//initial jump
		if (CanJump) {
			if (_controls.GetButtonDown(ActionNames.Jump)) {

				_rb.AddForce(new Vector2(0, JumpInitialVelocity), ForceMode2D.Impulse);
				CanJump = false;
				_jumpCooldownTimer = JumpCooldown;
			}
		}

		//jump hold force

		//jump damping
		if (!OnGround) {
			if(_rb.velocity.y > 0) {
				if (_controls.GetButtonUp(ActionNames.Jump)) {
					_rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * JumpReleaseDamping);
				}
			}
		}
	}

	void ProcessOnGround() {
		float hits = Physics2D.BoxCastNonAlloc(transform.position, _boxcastSize, 0, Vector2.down, _raycastResults, OnGroundRaycastDist, GroundLayers.value);
		if(hits > 0) {
			_animator.SetBool("InAir", false);
			OnGround = true;
			_coyoteTimer = CoyoteTime;
			if (_jumpCooldownTimer <= 0) {
				CanJump = true;
			}
		}
		else {
			OnGround = false;
			_animator.SetBool("InAir", true);
			_coyoteTimer -= Time.deltaTime;
			if(_coyoteTimer > 0 && _jumpCooldownTimer <= 0) {
				CanJump = true;
			}
			else {
				CanJump = false;
			}
		}
	}

	private void FixedUpdate() {
		_rb.AddForce(_horizontalMovementForce);
	}
}
