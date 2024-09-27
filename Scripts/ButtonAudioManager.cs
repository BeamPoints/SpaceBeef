using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class ButtonAudioManager : MonoBehaviour, ISelectHandler
{
	AudioSource buttonAudio = null;
	Button button = null;
	[SerializeField] AudioClip[] audioClips = null;
	// Use this for initialization
	void Awake () 
	{
		buttonAudio = GetComponent<AudioSource>();
		button = GetComponent<Button>();
		button.onClick.AddListener(OnClick);
	}

	public void OnSelect(BaseEventData eventData)
	{
		buttonAudio.clip = audioClips[0];
		buttonAudio.Play();
	}

	public void OnClick()
	{
		buttonAudio.clip = audioClips[1];
		buttonAudio.Play();

	}
}
