using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerSound {
	Jump = 0,
	Land = 1,
	Pickup = 2,
	GrappleShoot = 3,
	GrappleHitPull = 4,
	GrappleHitNoPull = 5,
	Error = 6,
	SpawnBox = 7,
	Phase = 8
}

public class PlayerSounds : MonoBehaviour
{
	public AudioClip[] Clips;
	private AudioSource _source;

	private void Start() {
		_source = GetComponent<AudioSource>();
	}

	public void PlaySound(PlayerSound sound) {
		_source.PlayOneShot(Clips[(int)sound]);
	}
}
