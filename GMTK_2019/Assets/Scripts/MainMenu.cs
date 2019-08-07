using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Rewired;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	bool _starting = false;

	Player _player;

	public float FadeTime = .5f;
	public CanvasGroup MenuGroup;
	public AudioSource MenuMusic;

	public CanvasGroup LevelSelectGroup;
	public LevelSelect Select;

	public int actionButtonPresses = 0;

    // Start is called before the first frame update
    void Start()
    {
		_player = ReInput.players.GetPlayer(0);
		MenuMusic = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
		if (_player.GetButtonDown(ActionNames.Jump) && !_starting) {
			_starting = true;
			StartCoroutine(FadeOutAndLoad());
		}

		if (_player.GetButtonDown("Mute")) {
			LevelManager.MusicMuted = !LevelManager.MusicMuted;
			if (LevelManager.MusicMuted) {
				MenuMusic.volume = 0;
			}
			else {
				MenuMusic.volume = .5f;
			}
		}

		if (_player.GetButtonDown(ActionNames.Ability)) {
			actionButtonPresses++;
			if(actionButtonPresses >= 5) {
				PlayerPrefs.SetInt("FurthestLevel", 10);
				SceneManager.LoadScene(0);
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Application.Quit();
		}
	}
	
	IEnumerator FadeOutAndLoad() {

		float time = FadeTime;
		while(time > 0) {
			//if (!LevelManager.MusicMuted) {
			//	MenuMusic.volume = (time / FadeTime) / .5f;
			//}
			MenuGroup.alpha = time / FadeTime;
			time -= Time.deltaTime;
			yield return null;
		}
		MenuGroup.alpha = 0;

		int FurthestLevel = PlayerPrefs.GetInt("FurthestLevel", 1);
		if (FurthestLevel == 1) {
			SceneManager.LoadScene(1);
		}
		else {

			Select.CanSelect = true;
			time = 0;
			LevelSelectGroup.gameObject.SetActive(true);
			while (time < FadeTime) {
				//if (!LevelManager.MusicMuted) {
				//	MenuMusic.volume = (time / FadeTime) / .5f;
				//}
				LevelSelectGroup.alpha = time / FadeTime;
				time += Time.deltaTime;
				yield return null;
			}

			LevelSelectGroup.alpha = 1;
		}
	}
}
