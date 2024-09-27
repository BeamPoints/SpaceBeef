using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using RewiredConsts;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
	[SerializeField] private Transform gunPoint = null;
    public Transform GunPoint { get { return gunPoint; } }

	[SerializeField] private float throwForce = 400;
    [SerializeField] private bool isStunned = false;
    public bool IsStunned { get { return isStunned; } set { isStunned = value; } }
	[SerializeField] private float stunTimer = 0f;
	public float StunTimer { get { return stunTimer; } set { stunTimer = value; } }

    private Health health;
    private WeaponManager weaponManager = null;
    private GameManager gameManager = null;
    public GameManager GameManager { get { return gameManager; } set { gameManager = value; } }
	private bool isPressed = false;
	private bool weaponEquipped = false;
	public bool WeaponEquipped { get { return weaponEquipped; } set { weaponEquipped = value; } }
	bool giveWeapon = false;
	public bool GiveWeapon { get { return giveWeapon; } set { giveWeapon = value; } }
	bool throwWeapon = false;

    [SerializeField] private Renderer playerRenderer;
    [SerializeField] private TrailRenderer playerTrailRenderer;

    Rewired.Player player = null; //Handles Input management
	public Rewired.Player Player { get { return player; } }

    private int playerIndex;
    public int PlayerIndex { get { return playerIndex; } set { playerIndex = value; } }

    private PlayerSelectionController playerSelection;
    public PlayerSelectionController PlayerSelection { get { return playerSelection; } set { playerSelection = value; } }

    private bool inPlayerSelection;
    public bool InPlayerSelection { get { return inPlayerSelection; } set { inPlayerSelection = value; } }

    private int winPoints;

	[SerializeField] GameObject playerIndicator = null;
	public GameObject PlayerIndicator { get { return playerIndicator; } }

	[SerializeField] UiImageFader imageFader = null;
    [SerializeField] private bool fadeIndicator = true;

    [SerializeField] private GameObject stunParticlePrefab;
    private Transform stunParticle;
    private bool stunParticleSpawned;

    private void Awake()
	{
        health = GetComponent<Health>();
	}
    
	// Use this for initialization
	void Start ()
	{
		WeaponEquipped = false;
		GetComponent<PlayerMovement>().rb = GetComponent<Rigidbody>();
		GetComponentInChildren<Animator>().SetInteger("State", -1);

		if(!inPlayerSelection)
		{
			playerIndicator.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
        if (!isStunned)
        {
		    GetComponent<PlayerMovement>().HandleMovementRotation();

		    if(weaponEquipped && weaponManager != null && throwWeapon)
		    {
		    	weaponManager.ThrowWeapon(throwForce);
		    	GetComponentInChildren<Animator>().SetInteger("State", -1);
		    	weaponEquipped = false;
		    	throwWeapon = false;
		    }
        }
	}

	private void Update()
    {
        if (player == null)
            return;

        weaponManager = GetComponentInChildren<WeaponManager>();

		giveWeapon = false;

        if (gameManager != null && isStunned && !gameManager.IsPaused)
        {
            if(!stunParticleSpawned)
            {
                stunParticle = Instantiate(stunParticlePrefab, transform.position + Vector3.up * 2, Quaternion.identity).transform;
                stunParticleSpawned = true;
            }

            stunTimer -= Time.deltaTime;
        }

        if(stunParticle != null)
        {
            stunParticle.position = transform.position + Vector3.up;
        }

        if(stunTimer <= 0f)
        {
            isStunned = false;
            stunParticleSpawned = false;
            stunTimer = 0f;
        }

        if (!isStunned)
        {
		    if(weaponEquipped)
		    {
		    	if(player.GetButtonDown(Action.THROWWEAPON))
		    	{
		    		throwWeapon = true;
		    	}
		    }
		    else
		    {
		    	if(player.GetButtonDown(Action.PICKUPWEAPON))
		    	{
		    		giveWeapon = true;
		    	}
		    }

            if(WeaponEquipped)
		    {
                if(player.GetButton(Action.SHOOT))
                {
                    if (weaponManager != null)
                    {
                        weaponManager.CallShoot(false);
                    }

                    // Trigger as ButtonDown
                    if (isPressed == false)
                    {
                        if (weaponManager != null)
                        {
                            weaponManager.CallShoot(true);
                        }
                        isPressed = true;
                    }
                }
                else
                {
                    if (weaponManager != null)
                    {
                        weaponManager.StopLaserSound();
                    }
                }
		    }
            

		    if (player.GetButtonUp(Action.SHOOT))
		    {
		    	isPressed = false;
                if (weaponManager)
                {
		    	    weaponManager.CallStopLaser();
                }
		    }


		    if(health.CurrentHealth <= 0)
		    {
		    	if(weaponManager)
		    	{
                    weaponManager.transform.localScale = weaponManager.transform.lossyScale * 5f;
		    		weaponManager.transform.parent = null;
                    weaponManager.CallStopLaser();
		    	}
		    }
        }

        //if(inPlayerSelection && player.GetButtonUp(Action.RETURNLOBBY))
        //{
        //    SetPlayerControls(playerIndex, true);
        //    playerSelection.ReturnSkinSelectedUi();
        //}
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "Weapon" && collision.gameObject.GetComponent<WeaponManager>().CanStun)
        {
            isStunned = true;
            stunTimer = 1f;
            Destroy(collision.gameObject, 0.5f);
            health.DecrementHealth();

            if(weaponManager != null)
            {
                weaponManager.CanStun = false;
                weaponManager.ActivateCollisionWithOwner();
                weaponManager.CallStopLaser();
            }
        }
    }

    /// <summary>
    /// Attention!!! Please do not use this method. It will just used
    /// in the selection scene.
    /// </summary>
    /// <param name="_playerIndex">Selected controller nummer</param>
    public void SetPlayerControls(int _playerIndex, bool _selection)
    {
        playerIndex = _playerIndex;
        player = ReInput.players.GetPlayer(_playerIndex);
        player.controllers.maps.SetAllMapsEnabled(false);
        inPlayerSelection = _selection;

        if (_selection)
            player.controllers.maps.SetMapsEnabled(true, Category.PLAYERSELECTION);
        else
            player.controllers.maps.SetMapsEnabled(true, Category.CHARACTERCONTROL);
        
        if (PlayerPrefs.HasKey("player" + playerIndex + "points"))
            winPoints = PlayerPrefs.GetInt("player" + playerIndex + "points");
        else
            winPoints = 0;
    }

    public void ChangeMaterial(Material _playerMaterial, Color _color)
    {
        playerRenderer.material = _playerMaterial;
        Color trailColor = _color * Mathf.LinearToGammaSpace(3.0f); ;
        playerTrailRenderer.material.SetColor("_EmissionColor", trailColor);
        playerTrailRenderer.material.SetColor("_Color", _color);
        playerTrailRenderer.startColor = _color;
        playerTrailRenderer.endColor = _color;
    }

    public void AddPoints(int _points)
    {
        winPoints += _points;
        PlayerPrefs.SetInt("player" + playerIndex + "points", winPoints);
	}

	public void DisableUserInput()
	{
		player.controllers.maps.SetAllMapsEnabled(false);
	}

	public void HidePlayerIndicator()
	{
        if(fadeIndicator)
		    StartCoroutine(imageFader.HideImage());
	}

    public void GameEnded()
    {
        if(weaponManager != null)
        {
            weaponManager.StopLaserSound();
            weaponManager.CallStopLaser();
        }
    }
}
