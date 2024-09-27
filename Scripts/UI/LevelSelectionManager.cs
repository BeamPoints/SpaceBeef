using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ProgressionSystem;
using RewiredConsts;
using Rewired;

public class LevelSelectionManager : MonoBehaviour
{
    [SerializeField] int levelCount = 2;
    [SerializeField] Text levelNameText = null;
    [SerializeField] Image levelImage = null;
	[SerializeField] Canvas loadingscreen = null;
	[SerializeField] Canvas selectionscreen = null;
    [SerializeField] SceneReference fallbackScene = null;

    int activeLevelImageIdx = 0;
    private bool levelSelected = false;
    List<GameObject> Levels = new List<GameObject>();
    
    void Start ()
    {
        foreach(var player in ReInput.players.Players)
        {
            player.controllers.maps.SetAllMapsEnabled(false);
            player.controllers.maps.SetMapsEnabled(true, Category.LEVELSELECTION);
        }
	}
	
	void Update ()
    {
        if (levelSelected)
        {
            foreach (var player in ReInput.players.Players)
            {
                player.controllers.maps.SetAllMapsEnabled(false);
            }

            return;
        }

        if(ReInput.players.Players.Any(x => x.GetButtonDown(Action.LEVELSELECTIONLEFT)))
        {
            activeLevelImageIdx--;
        }
        else if (ReInput.players.Players.Any(x => x.GetButtonDown(Action.LEVELSELECTIONRIGHT)))
        {
            activeLevelImageIdx++;
        }
        
        if(activeLevelImageIdx > ProgressionManager.Instance.StagePool.Count - 1)
        {
            activeLevelImageIdx = 0;

        }
        else if(activeLevelImageIdx < 0)
        {
            activeLevelImageIdx = ProgressionManager.Instance.StagePool.Count - 1;
        }

        ChangeLevel();

        if (ReInput.players.Players.Any(x => x.GetButtonDown(Action.LEVELSELECTIONSUBMIT)))
        {
            LoadLevelScene();
        }
    }

    /// <summary>
    /// Lädt Szene nach index.
    /// </summary>
    /// <param name="_levelIndex"></param>
    public void LoadLevelScene()
    {
        if (ProgressionManager.Instance.StagePool[activeLevelImageIdx].ItemName == "Zufall")
        {
            int isRandomLevel = activeLevelImageIdx;

            while(activeLevelImageIdx == isRandomLevel)
            {
                activeLevelImageIdx = Random.Range(0, ProgressionManager.Instance.StagePool.Count - 1);
            }
        }

		selectionscreen.enabled = false;
		loadingscreen.enabled = true;
		Invoke("Loading",1.5f);
    }

	//nur Zum testen
	void Loading()
	{
        levelSelected = true;
        StartCoroutine(LoadYourAsyncScene());
	}

    /// <summary>
    /// Switched das Image für das ausgewählte Level.
    /// </summary>
    private void ChangeLevel()
    {
        levelImage.sprite = ProgressionManager.Instance.StagePool[activeLevelImageIdx].ItemIcon;
        levelNameText.text = ProgressionManager.Instance.StagePool[activeLevelImageIdx].ItemName;
    }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="scenename"></param>
	/// <returns></returns>
	/// <para>-Patrick-</para>
	IEnumerator LoadYourAsyncScene()
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(ProgressionManager.Instance.StagePool[activeLevelImageIdx].Scene.ScenePath);

        if(asyncLoad == null)
        {
            asyncLoad = SceneManager.LoadSceneAsync(fallbackScene.ScenePath);
        }

        while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}
}
