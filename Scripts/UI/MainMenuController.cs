using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using Rewired;
using RewiredConsts;

public class MainMenuController : MonoBehaviour 
{

    [Header("Canvas")]
	[SerializeField] Canvas MenuCanvas = null;
    [SerializeField] List<Button> menuButtons = null;
    [SerializeField] Canvas CreditsCanvas = null;
    [SerializeField] Animator creditsAnimator = null;
    [SerializeField] Canvas OptionsCanvas = null;
    [SerializeField] List<Slider> optionSliders = null;
    [SerializeField] Canvas LoadingScreen = null;
	[SerializeField] GameObject quitButton = null;

    [Header("Audio")]
    [SerializeField] AudioMixer mixer;
	
	[Space(30)]
	[SerializeField] SceneReference nextScene;

    EventSystem eventSystem;
    bool showCredits = false;
    bool showOptions = false;

    // Use this for initialization
    void Start () 
	{
#if UNITY_PS4
		quitButton.SetActive(false);
#endif

		foreach(var player in ReInput.players.Players)
		{
			player.controllers.maps.SetAllMapsEnabled(false);
			player.controllers.maps.SetMapsEnabled(true, Category.MENU_UI_CONTROL);
		}

        eventSystem = EventSystem.current;
        MenuCanvas.enabled = true;
    }
	
	// Update is called once per frame
	void Update () 
	{	
        if (ReInput.players.Players.Any(x => x.GetButtonDown(Action.UICANCEL)))
		{
            if(showCredits)
            {
                SetShowCredits(false);
                //creditsAnimator.SetBool("isScrolling", false);
                //Debug.Log(creditsAnimator.GetBool("isScrolling"));
            }

            if (showOptions)
            {
                SetShowOptions(false);
            }
        }
    }

	void Loading()
	{
		StartCoroutine(LoadYourAsyncScene(nextScene.ScenePath));
    }

    public void Play()
    {
        LoadingScreen.enabled = true;
        MenuCanvas.enabled = false;
        CreditsCanvas.enabled = false;
        Invoke("Loading", 1.5f);
    }

    public void SetShowCredits(bool _value)
	{
		showCredits = _value;

        CreditsCanvas.enabled = showCredits;
        MenuCanvas.enabled = !showCredits;

        creditsAnimator.SetBool("isScrolling", _value);
        Debug.Log(creditsAnimator.GetBool("isScrolling"));

        foreach (var button in menuButtons)
        {
            button.enabled = !showCredits;
        }

        if (!showCredits)
        {
            eventSystem.SetSelectedGameObject(menuButtons[2].gameObject);
            //creditsAnimator.SetBool("isScrolling", false);
        }
    }

    public void SetShowOptions(bool _value)
    {
        showOptions = _value;

        OptionsCanvas.enabled = showOptions;

        foreach (var sliders in optionSliders)
        {
            sliders.enabled = showOptions;
        }

        if(showOptions)
        {
            eventSystem.SetSelectedGameObject(optionSliders[0].gameObject);
        }
        
        MenuCanvas.enabled = !showOptions;

        foreach (var button in menuButtons)
        {
            button.enabled = !showOptions;
        }

        if (!showOptions)
        {
            eventSystem.SetSelectedGameObject(menuButtons[1].gameObject);
        }
    }

    public void ChangeMusicVolume(float _value)
    {
        mixer.SetFloat("Music", Mathf.Log(_value) * 20);

        if (_value <= 0.0f)
        {
            mixer.SetFloat("Music", -80);
        }
    }

    public void ChangeEfxVolume(float _value)
    {
        mixer.SetFloat("Efx", Mathf.Log(_value) * 20);

        if (_value <= 0.0f)
        {
            mixer.SetFloat("Efx", -80);
        }
    }

    public void Quit()
	{
		Application.Quit();
	}

	IEnumerator LoadYourAsyncScene(string scenename)
	{
		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scenename);

		while (!asyncLoad.isDone)
		{
			yield return null;
		}

        LoadingScreen.enabled = false;
    }
}
