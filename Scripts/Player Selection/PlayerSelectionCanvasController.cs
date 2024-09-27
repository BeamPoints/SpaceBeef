using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionCanvasController : MonoBehaviour
{
	[Header("Canvas Elements")]
	[SerializeField] private Text playerSelectionTopText = null;
    [SerializeField] private Text playerSelectionBottomText = null;
    [SerializeField] private Image playerSelectionButtonImage = null;
    [SerializeField] private Image playerSelectionSkinImage = null;
    [SerializeField] private Image arrowLeftImage = null;
    [SerializeField] private Image arrowRightImage = null;
	[SerializeField, Space(15)] WeaponSpawner weaponSpawner = null;

	[Header("Xbox Buttons")]
	[SerializeField] private Sprite xboxAccept = null;
    [SerializeField] private Sprite xboxDenied = null;
    [SerializeField] private Sprite xboxGrabWeapon = null;
    [SerializeField] private Sprite xBoxArrowLeft = null;
    [SerializeField] private Sprite xBoxArrowRight = null;
    [SerializeField] private Sprite xBoxStickLeft = null;
    [SerializeField] private Sprite xBoxStickRight = null;
    [SerializeField] private Sprite xBoxShoot = null;
    [SerializeField] private Sprite xBoxStart = null;

	[Header("PS4 Buttons")]
	[SerializeField] private Sprite ps4Accept = null;
    [SerializeField] private Sprite ps4Denied = null;
    [SerializeField] private Sprite ps4GrabWeapon = null;
    [SerializeField] private Sprite ps4ArrowLeft = null;
    [SerializeField] private Sprite ps4ArrowRight = null;
	[SerializeField] private Sprite ps4StickLeft = null;
	[SerializeField] private Sprite ps4StickRight = null;
	[SerializeField] private Sprite ps4Shoot = null;
	[SerializeField] private Sprite ps4Start = null;

	Sprite acceptSprite = null;
	Sprite deniedSprite = null;
	Sprite grabWeaponSprite = null;
	Sprite arrowLeftSprite = null;
	Sprite arrowRightSprite = null;
	Sprite stickLeftSprite = null;
	Sprite stickRightSprite = null;
	Sprite shootSprite = null;
	Sprite startSprite = null;

	[SerializeField, Space] bool isBottomBox = false;


	public void Awake()
    {
#if UNITY_PS4
		acceptSprite = ps4Accept;
		deniedSprite = ps4Denied;
		grabWeaponSprite = ps4GrabWeapon;
		arrowLeftSprite = ps4ArrowLeft;
		arrowRightSprite = ps4ArrowRight;
		stickLeftSprite = ps4StickLeft;
		stickRightSprite = ps4StickRight;
		shootSprite = ps4Shoot;
		startSprite = ps4Start;
#else
		acceptSprite = xboxAccept;
		deniedSprite = xboxDenied;
		grabWeaponSprite = xboxGrabWeapon;
		arrowLeftSprite = xBoxArrowLeft;
		arrowRightSprite = xBoxArrowRight;
		stickLeftSprite = xBoxStickLeft;
		stickRightSprite = xBoxStickRight;
		shootSprite = xBoxShoot;
		startSprite = xBoxStart;
#endif
		playerSelectionSkinImage.enabled = false;
		arrowLeftImage.enabled = false;
		arrowRightImage.enabled = false;

		playerSelectionButtonImage.sprite = acceptSprite;

	}

    public void PlayerJoinUi()
    {
        playerSelectionTopText.text = "Beitreten";
        playerSelectionBottomText.text = "";
        playerSelectionSkinImage.enabled = false;
		arrowLeftImage.enabled = false;
		arrowRightImage.enabled = false;
		playerSelectionButtonImage.enabled = true;
		playerSelectionButtonImage.sprite = acceptSprite;
    }

    public void PlayerSelectColorUi()
    {
        playerSelectionTopText.text = "Farbe wählen";
        playerSelectionBottomText.text = "";
        playerSelectionSkinImage.enabled = true;
		arrowLeftImage.enabled = true;
		arrowRightImage.enabled = true;
        playerSelectionButtonImage.sprite = acceptSprite;
	}

	public void PlayerMoveUI()
	{
		playerSelectionTopText.text = "Bewegen";
		playerSelectionBottomText.text = "";
		playerSelectionSkinImage.enabled = false;
		arrowLeftImage.enabled = false;
		arrowRightImage.enabled = false;
		playerSelectionButtonImage.sprite = stickLeftSprite;
	}

	public void PlayerAimUI()
	{
		playerSelectionTopText.text = "Zielen";
		playerSelectionBottomText.text = "";
		playerSelectionButtonImage.sprite = stickRightSprite;
	}

	public void PlayerPickUpWeaponUi()
    {
        playerSelectionTopText.text = "Waffe aufheben/werfen";
        playerSelectionBottomText.text = "Eine geworfene Waffe kann andere Spieler betäuben";
        playerSelectionButtonImage.sprite = grabWeaponSprite;
		weaponSpawner.SpawnWeapon();

		if(isBottomBox)
		{
			playerSelectionBottomText.text = "Waffe aufheben/werfen";
			playerSelectionTopText.text = "Eine geworfene Waffe kann andere Spieler betäuben";
			playerSelectionTopText.rectTransform.localPosition = new Vector3(playerSelectionTopText.rectTransform.localPosition.x, -245.0f, playerSelectionTopText.rectTransform.localPosition.z);
		}
    }

	public void PlayerShootUI()
	{
		playerSelectionTopText.text = "Schießen";
		playerSelectionBottomText.text = "";
		playerSelectionButtonImage.sprite = shootSprite;

		if(isBottomBox)
		{
			playerSelectionTopText.rectTransform.localPosition = new Vector3(playerSelectionTopText.rectTransform.localPosition.x, -300.0f, playerSelectionTopText.rectTransform.localPosition.z);
		}
	}

	public void PlayerFinishTutorialUI()
    {
        playerSelectionTopText.text = "Zerschieß eine Wand";
        playerSelectionBottomText.text = "und schwebe ins All";
        playerSelectionSkinImage.enabled = false;
        playerSelectionButtonImage.enabled = false;

		if(isBottomBox)
		{
			playerSelectionBottomText.text = "Zerschieß eine Wand";
			playerSelectionTopText.text = "und schwebe ins All";
		}
	}

    public void PlayerJoinedGameUi()
    {
        playerSelectionTopText.text = "Spieler bereit";
        playerSelectionBottomText.text = "";
        playerSelectionSkinImage.enabled = false;
        playerSelectionButtonImage.enabled = false;
    }

	public void PlayerCanStartUI()
	{
		playerSelectionTopText.text = "Starten";
		playerSelectionBottomText.text = "Spieler bereit";
		playerSelectionSkinImage.enabled = false;
		playerSelectionButtonImage.enabled = true;
		playerSelectionButtonImage.sprite = startSprite;
	}

    public void ChangeSkin(Color _skinColor)
    {
        playerSelectionSkinImage.color = _skinColor;
    }

    private PlayerSelectionController playerSelection;
    public PlayerSelectionController PlayerSelection { get { return playerSelection ; } set { playerSelection = value; } }
}
