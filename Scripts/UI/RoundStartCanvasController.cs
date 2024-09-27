using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class RoundStartCanvasController : MonoBehaviour 
{
	
	[SerializeField] AudioSource[] audio = null;
	[SerializeField] GameManager gameManager = null;
	[SerializeField] Text countDownText = null;

	[SerializeField] float countDownTimer = 5.0f;
	bool isRunning = false;
	int oldCountdownSound;
	int newCountdownSound;

	// Use this for initialization
	void Start () 
	{
		countDownText.text = "";
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!isRunning)
		{
			return;
		}

		countDownTimer -= Time.deltaTime;

		if(countDownTimer <= 3.0f)
			countDownText.text = Mathf.RoundToInt (countDownTimer + 0.5f).ToString ("N0");

		newCountdownSound = Mathf.RoundToInt(countDownTimer);

		if (newCountdownSound != oldCountdownSound) 
		{
			if(newCountdownSound < audio.Length - 1)
				audio [newCountdownSound + 1].Play ();
		}

		oldCountdownSound = newCountdownSound;

		if(countDownTimer <= 0.0f)
		{
			countDownText.text = "BEEF";
			gameManager.StartGame();
		}

		if(countDownTimer <= -1.0f)
		{
			gameObject.SetActive(false);
		}

	}

	public void StartCountDown()
	{
		isRunning = true;
	}

}
