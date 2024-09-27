using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Rewired;
using RewiredConsts;

public class IntroController : MonoBehaviour
{
    [SerializeField] private UiImageFader titleImage;
    [SerializeField] private UiImageFader publisherImage;
    [SerializeField] private UiImageFader studioImage;
    [SerializeField] private SceneReference nextScene;
    [SerializeField] private AudioSource introTheme;
    [SerializeField] private float waitTime;

    Rewired.Player player = null; //Handles Input management

    private float passedWaitTime;
    private bool showTitle;
    private bool titleShowing;
    private bool fadeAudio;

    private void Awake()
    {
        foreach (var player in ReInput.players.Players)
        {
            player.controllers.maps.SetAllMapsEnabled(false);
            player.controllers.maps.SetMapsEnabled(true, Category.PLAYERSELECTION);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        titleImage.ForceHide();
        publisherImage.ForceHide();
        studioImage.ForceHide();
        StartCoroutine(publisherImage.ShowImageFixed());
    }

    private void Update()
    {
        if(publisherImage.Show && !publisherImage.IsHiding && !studioImage.Show && !showTitle)
        {
            passedWaitTime += Time.deltaTime;

            if(passedWaitTime >= waitTime)
            {
                StartCoroutine(publisherImage.HideImageFixed());
                passedWaitTime = 0.0f;
            }
        }

        if(!publisherImage.Show && !publisherImage.IsHiding && !studioImage.Show && !showTitle)
        {
            passedWaitTime += Time.deltaTime;

            if (passedWaitTime >= waitTime)
            {
                StartCoroutine(studioImage.ShowImageFixed());
                passedWaitTime = 0.0f;
            }
        }

        if (!publisherImage.Show && !studioImage.IsShowing && studioImage.Show && !showTitle)
        {
            passedWaitTime += Time.deltaTime;

            if (passedWaitTime >= waitTime)
            {
                StartCoroutine(studioImage.HideImageFixed());
                passedWaitTime = 0.0f;
                showTitle = true;
            }
        }

        if(showTitle && !titleShowing)
        {
            StartCoroutine(titleImage.ShowImageFixed());
            titleShowing = true;
        }

        if(titleShowing)
        {
            passedWaitTime += Time.deltaTime;

            if(passedWaitTime >= waitTime + 5.0f)
            {
                fadeAudio = true;
                Invoke("LoadScene", 2.0f);
            }
        }

        if (ReInput.players.Players.Any(x => x.GetButtonDown(Action.CONFIRMPLAYERSELECTION)))
        {
            fadeAudio = true;
            Invoke("LoadScene", 2.0f);
        }

        if(fadeAudio)
        {
            introTheme.volume -= 0.03f;
        }
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(nextScene.ScenePath);
    }
}
