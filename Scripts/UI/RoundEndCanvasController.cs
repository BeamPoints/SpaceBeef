using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ProgressionSystem;

public class RoundEndCanvasController : MonoBehaviour
{
    [SerializeField] private SceneReference levelSelectScene;
    [SerializeField] private SceneReference mainMenu;
	[SerializeField] Canvas loadingScreen;
    [SerializeField] private bool randomLevelSelection;
    [SerializeField] SceneReference mainMenuScene = null;
    [SerializeField] SceneReference fallbackScene = null;

    private bool returnToMenu = false;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    /********************************************************************************************************************/
    public void RestartRound()
    {
		GetComponent<Canvas>().enabled = false;
		loadingScreen.enabled = true;

        if(returnToMenu)
        {
            StartCoroutine(LoadYourAsyncScene(mainMenuScene.ScenePath));
        }
        else
        {
            if (!randomLevelSelection)
            {
                StartCoroutine(LoadYourAsyncScene(levelSelectScene.ScenePath));
            }
            else
            {
                int randomLevel = Random.Range(0, ProgressionManager.Instance.StagePool.Count - 1);

                if (ProgressionManager.Instance.StagePool[randomLevel].ItemName == "Zufall")
                {
                    int isRandomLevel = randomLevel;

                    while (randomLevel == isRandomLevel)
                    {
                        randomLevel = Random.Range(0, ProgressionManager.Instance.StagePool.Count - 1);
                    }
                }

                StartCoroutine(LoadYourAsyncScene(ProgressionManager.Instance.StagePool[randomLevel].Scene.ScenePath));
            }
        }
    }

	/********************************************************************************************************************/
	public void GoToMainMenu()
	{
		GetComponent<Canvas>().enabled = false;
		loadingScreen.enabled = true;
		StartCoroutine(LoadYourAsyncScene(mainMenu.ScenePath));
	}

	/********************************************************************************************************************/
	public void Quit()
    {
        Application.Quit();
    }

    public void ReturnToMenu()
    {
        returnToMenu = true;
    }

	IEnumerator LoadYourAsyncScene(string scenename)
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenename);
        
        if (asyncLoad == null)
        {
            asyncLoad = SceneManager.LoadSceneAsync(fallbackScene.ScenePath);
        }

        while (!asyncLoad.isDone)
		{
			yield return null;
		}

	}
}
