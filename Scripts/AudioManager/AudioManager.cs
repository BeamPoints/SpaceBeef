using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<SceneReference> toStopAudioScenes;
    [SerializeField] private AudioClip menuThemeClip;
    [SerializeField] private AudioMixerGroup mixerGroup;

    private static AudioManager instance;
    private AudioSource menuTheme;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        menuTheme = gameObject.AddComponent<AudioSource>();
        menuTheme.playOnAwake = false;
        menuTheme.loop = true;
        menuTheme.outputAudioMixerGroup = mixerGroup;
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void Start()
    {
        menuTheme.clip = menuThemeClip;
    }

    private void OnLevelLoaded(Scene newScene, LoadSceneMode mode)
    {
        bool isSceneToStop = false;

        foreach (var scene in toStopAudioScenes)
        {
            if(scene.ScenePath == newScene.path)
            {
                isSceneToStop = true;
                break;
            }
        }

        if(isSceneToStop)
        {
            if(menuTheme != null)
            {
                if (menuTheme.isPlaying)
                {
                    menuTheme.Stop();
                }
            }
        }
        else
        {
            if (menuTheme != null)
            {
                if (!menuTheme.isPlaying)
                {
                    menuTheme.Play();
                }
            }
        }
    }
}
