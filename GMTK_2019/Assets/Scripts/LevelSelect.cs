using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using TMPro;
using Rewired;
using NaughtyAttributes;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
	Player _controls;
	int _numLevels;
	public GameObject LevelSquarePrefab;
	public GameObject LockedLevelSquarePrefab;
	public Image LevelPreviewImage;
	public Sprite[] LevelPreviewSprites;

	public GameObject[] LevelSquares;

	public int CurrentSelection = -1;
	public Transform SelectorTransform;

	public AudioSource MenuMusic;
	public float FadeTime = .5f;
	public CanvasGroup MenuGroup;

	public bool CanSelect = false;

	public int FurthestLevel = 1;

    // Start is called before the first frame update
    void Awake()
    {
		FurthestLevel = PlayerPrefs.GetInt("FurthestLevel", 1);
		RectTransform thisrect = GetComponent<RectTransform>();
		HorizontalLayoutGroup group = GetComponent<HorizontalLayoutGroup>();
		_controls = ReInput.players.GetPlayer(0);
		_numLevels = SceneManager.sceneCountInBuildSettings - 2;
		LevelSquares = new GameObject[_numLevels];
		for (int i = 0; i < _numLevels; i++) {
			GameObject prefab = LevelSquarePrefab;
			if((i+1) > FurthestLevel) {
				prefab = LockedLevelSquarePrefab;
			}

			LevelSquares[i] = Instantiate(prefab);
			TextMeshProUGUI text = LevelSquares[i].GetComponentInChildren<TextMeshProUGUI>();
			text.text = (i + 1).ToString();
			LevelSquares[i].transform.SetParent(this.transform, false);
			LayoutRebuilder.ForceRebuildLayoutImmediate(thisrect);
		}
		StartCoroutine(DelaySetSelection());
	}

	[Button("Movenext")]
	void MoveNext() {
		SelectIndex(CurrentSelection + 1);
	}

    // Update is called once per frame
    void Update()
    {
		if (CanSelect) {
			if (_controls.GetButtonDown("Left")) {
				SelectIndex(CurrentSelection - 1);
			}
			else if (_controls.GetButtonDown("Right")) {
				SelectIndex(CurrentSelection + 1);
			}

			if (_controls.GetButtonDown("Jump")) {
				CanSelect = false;
				StartCoroutine(FadeOutAndLoad(CurrentSelection+1));
			}
		}
    }

	void SelectIndex(int newIndex) {
		if(newIndex < 0) {
			newIndex = FurthestLevel - 1;
		}
		else if(newIndex >= FurthestLevel) {
			newIndex = 0;
		}
		SelectorTransform.position = LevelSquares[newIndex].transform.position;
		//Vector3 newScale = LevelSquares[newIndex].transform.localScale;
		CurrentSelection = newIndex;
		LevelPreviewImage.sprite = LevelPreviewSprites[CurrentSelection];
	}

	IEnumerator DelaySetSelection() {
		yield return new WaitForSeconds(.05f);
		SelectIndex(0);
	}

	IEnumerator FadeOutAndLoad(int levelIndex) {

		float time = FadeTime;
		while (time > 0) {
			if (!LevelManager.MusicMuted) {
				MenuMusic.volume = (time / FadeTime) / .5f;
			}
			MenuGroup.alpha = time / FadeTime;
			time -= Time.deltaTime;
			yield return null;
		}
		MenuGroup.alpha = 0;

		SceneManager.LoadScene(levelIndex);
	}

}
