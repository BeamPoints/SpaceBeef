using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiImageFader : MonoBehaviour
{
    [SerializeField] private float fadeStep;

    private Image image;
    private float alphaColor;
    private bool isShowing;
    private bool isHiding;
    private bool show;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public IEnumerator ShowImage()
    {
        alphaColor = 0.0f;
        isShowing = true;

        while (image.color.a <= 1.0f)
        {
            alphaColor += fadeStep * Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alphaColor);
            yield return new WaitForEndOfFrame();
		}

        isShowing = false;
        show = true;
    }

    public IEnumerator ShowImageFixed()
    {
        alphaColor = 0.0f;
        isShowing = true;

        while (image.color.a <= 1.0f)
        {
            alphaColor += fadeStep;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alphaColor);
            yield return new WaitForSeconds(0.05f);
        }

        isShowing = false;
        show = true;
    }

    public IEnumerator HideImage()
    {
        alphaColor = 1.0f;
        isHiding = true;

        while (image.color.a >= 0.0f)
        {
            alphaColor -= fadeStep * Time.deltaTime;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alphaColor);
			yield return new WaitForEndOfFrame();
        }

        isHiding = false;
        show = false;
    }

    public IEnumerator HideImageFixed()
    {
        alphaColor = 1.0f;
        isHiding = true;

        while (image.color.a >= 0.0f)
        {
            alphaColor -= fadeStep;
            image.color = new Color(image.color.r, image.color.g, image.color.b, alphaColor);
            yield return new WaitForSeconds(0.05f);
        }

        isHiding = false;
        show = false;
    }

    public void ForceShow()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1.0f);
        show = true;
    }

    public void ForceHide()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0.0f);
        show = false;
    }

    public bool IsShowing { get { return isShowing; } }
    public bool IsHiding { get { return isHiding; } }
    public bool Show { get { return show; } }
}
