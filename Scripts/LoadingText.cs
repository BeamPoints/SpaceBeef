using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingText : MonoBehaviour
{
	[SerializeField] bool Loading = false;
	[SerializeField] bool Dots;
	[SerializeField] List<string> infoText;

	Text textbox;
	int tip;

	private void Start()
	{
		if(!Loading)
		{
			ShowTipText();
		}
	}

	void ShowTipText()
	{
		if (textbox == null)
		{
			textbox = GetComponent<Text>();
		}
		if (infoText != null)
		{
			if (!Dots)
			{
				InvokeRepeating("Infotext", 0, 6);
			}
			else
			{
				InvokeRepeating("Loadingdots", 1, 1);
			}
		}
	}

	private void Infotext()
	{
		textbox.text = infoText[Random.Range(0, infoText.Count)];
	}

	private void Loadingdots()
	{
		if (textbox.text == "...")
		{
			textbox.text = ".";
		}
		else if(textbox.text == "..")
		{
			textbox.text = "...";
		}
		else if(textbox.text == ".")
		{
			textbox.text = "..";
		}
		else
		{
			textbox.text = "error";
		}

	}
}

