using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarController : MonoBehaviour 
{
	float healthPercentage = -1.0f;
	float maxHealthbarWidth = -999.0f;
	float currentHealthbarWidth = -999.0f;
	RectTransform rectTransform = null;

	public Health playerHealth = null;
	public Color HealthBarColor
	{
		set
		{
			GetComponent<Image>().color = value;
			Color color = GetComponent<Image>().color;
			color.a = 0.5f;
			GetComponent<Image>().color = color;
		}
	}

	public bool hasHealthScript = false;

	// Use this for initialization
	void Start () 
	{
		rectTransform = GetComponent<RectTransform>();
		maxHealthbarWidth = rectTransform.rect.width;
		currentHealthbarWidth = maxHealthbarWidth;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!hasHealthScript)
		{
			return;
		}

		healthPercentage = (float)playerHealth.CurrentHealth / (float)playerHealth.MaxHealth;
		currentHealthbarWidth = maxHealthbarWidth * healthPercentage;
		rectTransform.sizeDelta = new Vector2(currentHealthbarWidth, maxHealthbarWidth);
	}

	public void AddHealthScript (Health _scriptToAdd)
	{
		playerHealth = _scriptToAdd;
		hasHealthScript = true;
		GetComponent<Image>().enabled = true;
	}
}
