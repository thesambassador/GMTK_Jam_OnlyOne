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
	
    // Start is called before the first frame update
    void Start()
    {
		instance = this;
		_controls = ReInput.players.GetPlayer(0);
		int buildIndex = SceneManager.GetActiveScene().buildIndex;
		PlayerPrefs.SetInt("FurthestLevel", buildIndex);
	}

    // Update is called once per frame
    void Update()
    {
		if (_controls.GetButtonDown("Restart")) {
			RestartLevel();
		}
	}

	public static void RestartLevel() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	[Button("CompleteLevel")]
	public static void CompleteLevel() {
		int buildIndex = SceneManager.GetActiveScene().buildIndex;
		if(buildIndex < SceneManager.sceneCountInBuildSettings - 1) {
			LoadLevel(buildIndex+1);
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
