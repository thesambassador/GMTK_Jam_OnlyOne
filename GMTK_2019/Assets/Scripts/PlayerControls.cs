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
	float _coyoteTimer = -1;
	float _jumpCooldownTimer = -1;
	int _numAirJumpsRemaining = 0;
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
	public int NumAdditionalJumps = 0;

	[Header("OnGround Stuff")]
	public float CoyoteTime;
	public float JumpCooldown;
	public float OnGroundRaycastDist;
	public LayerMask GroundLayers;

	[Header("Other")]
	public bool CanPush = false;

	[Header("Live Values")]
	public bool OnGround = false;
	public bool CanJump = false;
	public bool facingRight = true;
	public PlayerAbility CurrentAbility;

	public BoxCaster boxCaster;
	public PlayerSounds SoundPlayer;

    // Start is called before the first frame update
    void Start()
    {
		_controls = ReInput.players.GetPlayer(0);
		_rb = GetComponent<Rigidbody2D>();
		_animator = GetComponentInChildren<Animator>();

		_horizontalMovementForce = new Vector2(0, 0);

		SoundPlayer = GetComponent<PlayerSounds>();
	}

    // Update is called once per frame
    void Update()
    {
		ProcessOnGround();
		ProcessHorizontalMovement();
		ProcessJump();
		ProcessAbility();
    }

	void ProcessHorizontalMovement() {
		AnimationCurve accelCurve = AccelerationVelocityCurveGround;
		if (!OnGround) { accelCurve = AccelerationVelocityCurveAir; }

		float xMovement = _controls.GetAxis(ActionNames.HorizontalMovement);

		float targetVel = xMovement * MaxSpeed;
		float velDiff = targetVel - _rb.velocity.x;
		float normalizedVelDiff = Mathf.Abs(velDiff) / MaxSpeed;

		float accel = accelCurve.Evaluate(normalizedVelDiff / MaxSpeed) * MaxAcceleration * Mathf.Sign(velDiff);
		_horizontalMovementForce.x = accel;

		//print("targetvel: " + targetVel + " -- normalizedVelDiff " + normalizedVelDiff + " -- accel: " + accel);
		
		//animation shit:
		if(_rb.velocity.x > .05) {
			MainCharSprite.flipX = false;
			facingRight = true;
			_animator.SetBool("Walking", true);
		}
		else if(_rb.velocity.x < -.05 ){
			MainCharSprite.flipX = true;
			facingRight = false;
			if (Mathf.Abs(xMovement) > .1f) {
				_animator.SetBool("Walking", true);
			}
			else {
				_animator.SetBool("Walking", false);
			}
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
				SoundPlayer.PlaySound(PlayerSound.Jump);
				CanJump = false;
				_jumpCooldownTimer = JumpCooldown;
			}
		}

		//jump damping/double jumps
		else if (!OnGround) {
			if (_rb.velocity.y > 0) {
				if (_controls.GetButtonUp(ActionNames.Jump)) {
					_rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * JumpReleaseDamping);
				}
			}

			if (_numAirJumpsRemaining > 0) {
				if (_controls.GetButtonDown(ActionNames.Jump)){
					Vector2 vel = _rb.velocity;
					vel.y = 0;
					_rb.velocity = vel;
					_rb.AddForce(new Vector2(0, JumpInitialVelocity), ForceMode2D.Impulse);
					SoundPlayer.PlaySound(PlayerSound.Jump);
					_numAirJumpsRemaining--;
				}
			}
		}

		//jump hold force


	}

	void ProcessOnGround() {
		RaycastHit2D groundHit;
		if(boxCaster.GetFirstBoxcastHit(Vector2.down, OnGroundRaycastDist, GroundLayers, out groundHit)) {
			//print(groundHit.point.y);
			if (groundHit.point.y < transform.position.y - .35f) {
				_animator.SetBool("InAir", false);
				if (!OnGround) {
					SoundPlayer.PlaySound(PlayerSound.Land);
				}

				OnGround = true;
				_numAirJumpsRemaining = NumAdditionalJumps;
				_coyoteTimer = CoyoteTime;
				if (_jumpCooldownTimer <= 0) {
					CanJump = true;
				}
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

	void ProcessAbility() {
		if(CurrentAbility != null) {
			CurrentAbility.UpdateAbility();
			if (_controls.GetButtonDown(ActionNames.Ability)){
				CurrentAbility.UseAbility();
			}
		}
		
	}

	public void SetNumJumps(int newNumJumps) {
		_numAirJumpsRemaining = newNumJumps;
		NumAdditionalJumps = newNumJumps;
	}

	private void FixedUpdate() {
		_rb.AddForce(_horizontalMovementForce);
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		PlayerAbility ability = collision.GetComponent<PlayerAbility>();
		if(ability != null && ability.CanBePickedUp) {
			if(CurrentAbility != null) {
				CurrentAbility.DropAbility(ability.transform.position);
			}
			ability.PickupAbility(this);
			SoundPlayer.PlaySound(PlayerSound.Pickup);
			LevelManager.HighlightAbilityAnimation();
			CurrentAbility = ability;
		}
		else if(collision.tag == "Goal") {
			PausePlayerControls();
			LevelManager.CompleteLevel();
		}
	}

	private void OnCollisionStay2D(Collision2D collision) {
		
		if(collision.gameObject.tag == "PushBlock") {
			if (OnGround && Mathf.Abs(transform.position.y - collision.transform.position.y) < .5f && CanPush) {
				float xInput = _controls.GetAxis(ActionNames.HorizontalMovement);
				PushableBlock pb = collision.gameObject.GetComponent<PushableBlock>();
				if (transform.position.x < collision.transform.position.x && xInput > 0) {
					pb.TryPush(true);
				}
				else if (transform.position.x > collision.transform.position.x && xInput < 0) {
					pb.TryPush(false);
				}
			}
		}
	}

	private void OnCollisionExit2D(Collision2D collision) {
		if (collision.gameObject.tag == "PushBlock") {
			PushableBlock pb = collision.gameObject.GetComponent<PushableBlock>();
			pb.ResetPushTimer();
		}
	}

	public void PausePlayerControls() {
		_rb.simulated = false;
	}

	public void ResumePlayerControls() {
		_rb.simulated = true;
	}


}
