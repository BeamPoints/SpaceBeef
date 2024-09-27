using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProgressionSystem;
using Rewired;
using RewiredConsts;

public class PlayerSelectionController : MonoBehaviour
{
    [SerializeField]
    private PlayerController playerPrefab;

    private PlayerSelectionManager selectionManager;
    public PlayerSelectionManager SelectionManager { set { selectionManager = value; } }

    PlayerSelectionCanvasController playerCanvas;
    public PlayerSelectionCanvasController PlayerCanvas { set { playerCanvas = value; } }

    private PlayerController playerController;
    public bool IsPlayerReady { get { return playerController != null; } }
    private int playerIndex;

    private bool joinPressed;

    private bool possibleToPlay;
    public bool PossibleToPlay { set { possibleToPlay = value; } }

    private bool joinedGame;
    public bool JoinedGame { get { return joinedGame; } set { joinedGame = value; } }

    private bool changeRounds;
    public bool ChangeRounds { get { return changeRounds; } set { changeRounds = value; } }

    private int skinIndex;
    public int SkinIndex { get { return skinIndex; } set { skinIndex = value; } }

    private ProgressionSkinData actualSkin;

    Rewired.Player player = null; //Handles Input management
    public Rewired.Player Player { get { return player; } }

    bool playerHasColor = false;
    bool playerHasMoved = false;
    bool playerHasAimed = false;
    bool playerHasShot = false;

    private void Update()
    {
        if (changeRounds)
        {
            if (player.GetButtonDown(Action.CHANGECOLORRIGHT))
                selectionManager.ChangeRoundAmount(true);
            else if (player.GetButtonDown(Action.CHANGECOLORLEFT))
                selectionManager.ChangeRoundAmount(false);
        }

        if (joinedGame)
        {
            if (possibleToPlay)
            {
                playerCanvas.PlayerCanStartUI();
            }

            if (player.GetButtonDown(Action.CONFIRMPLAYERSELECTION) && possibleToPlay)
                selectionManager.StartGame();

            return;
        }

        joinPressed = false;

        if (playerController == null && player.GetButtonDown(Action.JOINLOBBY))
        {
            playerController = Instantiate(playerPrefab, transform.position, Quaternion.identity);
            playerCanvas.PlayerSelectColorUi();
            SetupPlayerSelectionController(playerIndex);
            playerController.PlayerSelection = this;
            playerController.SetPlayerControls(player.id, true);
            joinPressed = true;
        }

        if (playerController != null && player.GetButtonDown(Action.LEAVELOBBY))
        {
            Destroy(playerController.gameObject);
            playerCanvas.PlayerJoinUi();
        }

        if (playerController != null && player.GetButtonDown(Action.RETURNLOBBY))
        {
            playerController.SetPlayerControls(player.id, true);
            ReturnSkinSelectedUi();
            playerCanvas.PlayerJoinUi();
            Destroy(playerController.gameObject);
        }

        if (playerController == null)
            return;

        if (player.GetButtonDown(Action.CHANGECOLORRIGHT))
            skinIndex++;
        else if (player.GetButtonDown(Action.CHANGECOLORLEFT))
            skinIndex--;

        if (skinIndex < 0)
            skinIndex = selectionManager.AllSkins.Count - 1;
        else if (skinIndex > selectionManager.AllSkins.Count - 1)
            skinIndex = 0;

        SelectSkin(skinIndex);

        if (player.GetButtonDown(Action.JOINLOBBY) && !joinPressed)
        {
            if (selectionManager.IsSkinAvailable(skinIndex))
            {
                playerController.SetPlayerControls(playerIndex, false);
                playerCanvas.PlayerMoveUI();
                actualSkin = selectionManager.SelectSkin(skinIndex);
                selectionManager.SkinSelectedSound.Play();
                playerHasColor = true;
            }
            else
            {
                selectionManager.SkinNotAvailabeSound.Play();
            }
        }

        if (playerHasColor && !playerHasMoved)
        {
            if (player.GetButton(Action.MOVE_HORIZONTAL) || player.GetButton(Action.MOVE_VERTICAL))
            {
                playerHasMoved = true;
                playerCanvas.PlayerAimUI();
            }
        }

        if (playerHasMoved && !playerHasAimed)
        {
            if (player.GetButton(Action.AIMHORIZONTAL) || player.GetButton(Action.AIMVERTICAL))
            {
                playerHasAimed = true;
                playerCanvas.PlayerPickUpWeaponUi();
            }
        }

        if (playerController.WeaponEquipped && !playerHasShot)
        {
            playerCanvas.PlayerShootUI();

            if (player.GetButtonDown(Action.SHOOT))
            {
                playerHasShot = true;
                playerCanvas.PlayerFinishTutorialUI();
            }
        }
    }

    private void SelectSkin(int _skinIndex)
    {
        ProgressionSkinData selectedSkin = selectionManager.GetSkinByIndex(_skinIndex);
        playerCanvas.ChangeSkin(selectedSkin.SkinColor);
        playerController.ChangeMaterial(selectedSkin.Material, selectedSkin.SkinColor);
    }

    public void SetupPlayerSelectionController(int _playerIndex)
    {
        playerIndex = _playerIndex;
        player = ReInput.players.GetPlayer(_playerIndex);
        player.controllers.maps.SetAllMapsEnabled(false);
        player.controllers.maps.SetMapsEnabled(true, Category.PLAYERSELECTION);
    }

    public void PlayerJoinedGame()
    {
        playerCanvas.PlayerJoinedGameUi();
    }

    public void ReturnSkinSelectedUi()
    {
        if (actualSkin != null)
        {
            selectionManager.DeselectSkin(skinIndex);
            actualSkin = null;
        }

        playerCanvas.PlayerSelectColorUi();
    }
} 