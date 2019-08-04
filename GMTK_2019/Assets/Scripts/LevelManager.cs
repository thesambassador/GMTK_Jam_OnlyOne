using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using NaughtyAttributes;
using Rewired;

public class LevelManager : MonoBehaviour
{
	Player _controls;
	public static LevelManager instance;
	public Animator UIAnimator;
	public AudioSource MusicAudio;

	public AudioClip EndLevelMusic;

	public static bool MusicMuted = false;
	
    // Start is called before the first frame update
    void Start()
    {
		instance = this;
		_controls = ReInput.players.GetPlayer(0);
		int buildIndex = SceneManager.GetActiveScene().buildIndex;
		PlayerPrefs.SetInt("FurthestLevel", buildIndex);

		if (MusicMuted) {
			MusicAudio.volume = 0;
		}
	}

    // Update is called once per frame
    void Update()
    {
		if (_controls.GetButtonDown("Restart")) {
			RestartLevel();
		}

		if (_controls.GetButtonDown("Mute")){
			MusicMuted = !MusicMuted;
			if (MusicMuted) {
				MusicAudio.volume = 0;
			}
		}
	}

	public static void RestartLevel() {
		instance.UIAnimator.SetTrigger("RestartLevel");
		instance.StartCoroutine(instance.RestartDelay());
	}

	public IEnumerator RestartDelay() {
		yield return new WaitForSeconds(.15f);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	[Button("CompleteLevel")]
	public static void CompleteLevel() {
		instance.UIAnimator.SetTrigger("EndLevel");
		instance.MusicAudio.Stop();
		instance.MusicAudio.PlayOneShot(instance.EndLevelMusic);
		instance.StartCoroutine(instance.WaitForKeypressToNextLevel());
	}

	public IEnumerator WaitForKeypressToNextLevel() {
		yield return new WaitForSeconds(.5f);
		while (!_controls.GetButton(ActionNames.Jump)) {
			yield return null;
		}

		LoadNextLevel();
	}

	public static void LoadNextLevel() {
		int buildIndex = SceneManager.GetActiveScene().buildIndex;
		if (buildIndex < SceneManager.sceneCountInBuildSettings - 1) {
			LoadLevel(buildIndex + 1);
		}
		else {
			BackToMainMenu();
		}
	}

	public static void LoadLevel(int levelToLoad) {
		SceneManager.LoadScene(levelToLoad);
	}

	public static void BackToMainMenu() {
		SceneManager.LoadScene(0);
	}
	
}
