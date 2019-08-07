using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScreenshotTaker : MonoBehaviour
{

	string path;

	public int StartIndex = 1;
	public int EndIndex = -1;

	bool sceneDoneLoading = false;

    // Start is called before the first frame update
    void Start()
    {
		DontDestroyOnLoad(this.gameObject);
		print(Application.dataPath);
		path = Application.dataPath;
		if(EndIndex == -1) {
			EndIndex = SceneManager.sceneCountInBuildSettings - 2;
		}
		SceneManager.sceneLoaded += OnSceneLoaded;
		StartCoroutine(CycleThroughScenes());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	IEnumerator CycleThroughScenes() {
		int index = StartIndex;

		while(index < EndIndex) {
			string fullPath = path + "/levelScreenshot" + (index) + ".png";
			
			sceneDoneLoading = false;
			SceneManager.LoadScene(index);
			while(sceneDoneLoading == false) {
				yield return null;
			}
			yield return new WaitForSeconds(1);
			ScreenCapture.CaptureScreenshot(fullPath);
			print("saved at " + fullPath);
			index++;
		}

	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		sceneDoneLoading = true;
	}
}
