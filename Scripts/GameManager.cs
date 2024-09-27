using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using ProgressionSystem;
using Cinemachine;
using Rewired;
using RewiredConsts;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] private PlayerPointsController playerPointsPrefab;
    [SerializeField] private GameObject playerPointsPanel;
    [SerializeField] private CinemachineTargetGroup targetGroup;
    [SerializeField] private List<Transform> playerSpawnPoints;
    [Header("Gameplay")]
    [SerializeField] private float winTimer = 3.0f;
	[Header("Canvas")]
	[SerializeField] EventSystem eventSystem = null;
    [Space(15), SerializeField] private Canvas roundEndCanvas;
    [SerializeField] private Text roundEndText;
	[SerializeField] GameObject firstSelectionRoundEndCanvas = null;
	[Space(15), SerializeField] private Canvas pauseCanvas = null;
	[SerializeField] GameObject firstSelectionPauseMenu = null;
	[Space, SerializeField] List<HealthbarController> healthbars = new List<HealthbarController>();
    [SerializeField]
    SceneReference mainMenuScene = null;
    [Header("Exp")]
    [SerializeField] private int expPerKill;
    [SerializeField] private Image expIconImage;
    [SerializeField] private Text expTextValue;
    [SerializeField] private Text expAmountValue;
    [SerializeField] private Slider expValueSlider;
    [SerializeField] private Text levelUpText;
    [Header("Audio")]
    [SerializeField] private AudioMixerSnapshot gameSnapshot;
    [SerializeField] private AudioMixerSnapshot menuSnapshot;
    [SerializeField] private List<AudioSource> themes;
    [Header("Debug Mode")]
    [SerializeField] private bool playWithoutPrefs;
    [SerializeField, Range(2, 4)] private int debugPlayerAmount;

    List<PlayerController> players = new List<PlayerController>();

    private int playerAmount = 0;
    private int playerTotalAmount = 0;
    private int actualExp;
    private int earnedExp;
    private int newExp;
    private int actualLevel;
    private int newLevel;
    private bool newLevelSlide;
    private bool roundEnded = false;
    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } set { isPaused = value; } }
    private int roundsPlayed = 0;
    private bool roundsSetted = false;

	[SerializeField] RoundStartCanvasController roundStartCanvas = null;

    void Update()
    {
        CheckIfPlayerDied();
        CheckForWinCondition();
    }

    /********************************************************************************************************************/
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    /********************************************************************************************************************/
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene.ScenePath);
    }

    /********************************************************************************************************************/
    void StartUp()
    {
        if(PlayerPrefs.HasKey("roundsPlayed"))
            roundsPlayed = PlayerPrefs.GetInt("roundsPlayed");

        playerAmount = PlayerPrefs.GetInt("playeramount");

        if (playWithoutPrefs)
            playerAmount = debugPlayerAmount;

        playerTotalAmount = playerAmount;

        for (int i = 0; i < playerAmount; i++)
        {
            PlayerController player = Instantiate(playerPrefab, playerSpawnPoints[i].position, Quaternion.identity);
            player.gameObject.name = "Spieler " + (i + 1);

			player.SetPlayerControls(i, false);
			player.DisableUserInput();
			int skinIndex = PlayerPrefs.GetInt("skin" + i);
            player.ChangeMaterial(ProgressionManager.Instance.SkinPool[skinIndex].Material, ProgressionManager.Instance.SkinPool[skinIndex].SkinColor);
            player.GameManager = this;
            players.Add(player);
            targetGroup.m_Targets[i].target = player.transform;
            targetGroup.m_Targets[i].weight = 1f;

			healthbars[i].AddHealthScript(player.GetComponent<Health>());
			healthbars[i].HealthBarColor = ProgressionManager.Instance.SkinPool[skinIndex].SkinColor;

			player.PlayerIndicator.GetComponent<Image>().color = ProgressionManager.Instance.SkinPool[skinIndex].SkinColor;
        }

        themes[Random.Range(0, themes.Count)].Play();
        roundStartCanvas.StartCountDown();
    }

	public void StartGame()
	{
		foreach(PlayerController player in players)
		{
			player.Player.controllers.maps.SetMapsEnabled(true, Category.CHARACTERCONTROL);
			player.HidePlayerIndicator();
		}
    }

    /********************************************************************************************************************/
    void CheckIfPlayerDied()
    {
        for (int i = 0; i < playerAmount; i++)
        {
            if (players.ElementAtOrDefault(i) == null)
            {
                StartCoroutine(RemoveTarget(i));
				players.RemoveAt(i);
				playerAmount--;
			}
		}
    }

    /********************************************************************************************************************/
    void CheckForWinCondition()
    {
        if (players.Count > 1)
        {
            return;
        }
        else if (players.Count == 1 && !roundEnded)
        {
            winTimer -= Time.deltaTime;
            string playerNumber = "";

            if (players.Count > 0)
            {
                playerNumber = players[0].gameObject.name;
                players[0].GameEnded();
            }

            roundEndText.text = playerNumber + " hat gewonnen!";
        }
        else if (players.Count == 0 && !roundEnded)
        {
            winTimer -= Time.deltaTime;

            if (players.Count > 0)
            {
                players[0].GameEnded();
            }

            if (players.Count > 1)
            {
                players[1].GameEnded();
            }

            roundEndText.text = "Unentschieden!";
        }

        if (winTimer <= 0.0f && !roundEnded)
        {
            roundEnded = true;
            
            EndRounds();

            foreach (Rewired.Player player in ReInput.players.Players)
			{
				player.controllers.maps.SetAllMapsEnabled(false);
				player.controllers.maps.SetMapsEnabled(true, Category.MENU_UI_CONTROL);
			}

            roundEndCanvas.gameObject.SetActive(true);
			eventSystem.SetSelectedGameObject(firstSelectionRoundEndCanvas);
		}
    }

    /********************************************************************************************************************/
    void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        StartUp();
    }

    private void EndRounds()
    {
        if (!roundsSetted)
        {
            roundsSetted = true;
            roundsPlayed++;
        }
        Debug.Log(roundsPlayed); 
        if(players.Count > 0)
            players[0].AddPoints(1);
        
        PlayerPrefs.SetInt("roundsPlayed", roundsPlayed);
        List<PlayerPointsController> playerPoints = new List<PlayerPointsController>();

        for (int i = 0; i < playerTotalAmount; i++)
        {
            PlayerPointsController pointController = Instantiate(playerPointsPrefab, playerPointsPanel.transform);
            pointController.InstantiateDissolve();

            if(players.Count > 0 && ("Spieler " + (i + 1)) == players[0].name)
            {
                pointController.IsWinner = true;
                pointController.WinningPlayerPoints = PlayerPrefs.GetInt("player" + i + "points");
                pointController.Invoke("StartDissolveEffect", 0.5f);
                pointController.PlayerIndex = i;
                playerPoints.Add(pointController);
            }

            int skinIndex = PlayerPrefs.GetInt("skin" + i);
            pointController.SetPoints((i + 1).ToString(), PlayerPrefs.GetInt("player" + i + "points"), ProgressionManager.Instance.SkinPool[skinIndex].SkinColor);
        }

        if (roundsPlayed >= PlayerPrefs.GetInt("rounds"))
        {
            int playerIndex = playerPoints.OrderByDescending(x => x.WinningPlayerPoints).ToList()[0].PlayerIndex;

            roundEndText.color = ProgressionManager.Instance.SkinPool[PlayerPrefs.GetInt("skin" + playerIndex)].SkinColor;
            roundEndText.text = "Spieler " + (playerIndex + 1) + " hat die Partie gewonnen.";
            roundEndCanvas.GetComponent<RoundEndCanvasController>().ReturnToMenu();
        }

        CalculateExpHud();
    }

    private void CalculateExpHud()
    {
        actualExp = ProgressionManager.Instance.Exp;
        actualLevel = ProgressionManager.Instance.ActualLevelNumber;

        expValueSlider.minValue = ProgressionManager.Instance.CharacterLevelDatabase.CharacterLevels[actualLevel].levelRangeMinValue;
        expValueSlider.maxValue = ProgressionManager.Instance.CharacterLevelDatabase.CharacterLevels[actualLevel].levelRangeMaxValue;
        expValueSlider.value = actualExp;

        expIconImage.sprite = ProgressionManager.Instance.CharacterLevelDatabase.CharacterLevels[actualLevel].levelIcon;
        expTextValue.text = ProgressionManager.Instance.CharacterLevelDatabase.CharacterLevels[actualLevel].levelName;

        earnedExp = (playerTotalAmount - playerAmount) * expPerKill;
        newExp = actualExp + earnedExp;

        ProgressionManager.Instance.SaveProgression(newExp);
        newLevel = ProgressionManager.Instance.ActualLevelNumber;

        if (newLevel != actualLevel)
        {
            newLevelSlide = true;
            levelUpText.text = "Neue Inhalte freisgeschaltet...";
        }

        StartCoroutine(ActivateExpSlider());
    }

    private IEnumerator ActivateExpSlider()
    {
        if (!newLevelSlide)
        {
            while (expValueSlider.value < newExp)
            {
                expValueSlider.value += 1;
                expAmountValue.text = expValueSlider.value.ToString();
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            levelUpText.text = "";

            while (expValueSlider.value < expValueSlider.maxValue)
            {
                expValueSlider.value += 1;
                expAmountValue.text = expValueSlider.value.ToString();
                yield return new WaitForSeconds(0.01f);
            }

            levelUpText.text = ProgressionManager.Instance.GetItemNamePerLevel(actualLevel + 1);
        }

        if(newLevelSlide)
        {
            expValueSlider.minValue = ProgressionManager.Instance.CharacterLevelDatabase.CharacterLevels[newLevel].levelRangeMinValue;
            expValueSlider.maxValue = ProgressionManager.Instance.CharacterLevelDatabase.CharacterLevels[newLevel].levelRangeMaxValue;
            expValueSlider.value = expValueSlider.minValue;

            expIconImage.sprite = ProgressionManager.Instance.CharacterLevelDatabase.CharacterLevels[newLevel].levelIcon;
            expTextValue.text = ProgressionManager.Instance.CharacterLevelDatabase.CharacterLevels[newLevel].levelName;

            while (expValueSlider.value < newExp)
            {
                expValueSlider.value += 1;
                expAmountValue.text = expValueSlider.value.ToString();
                yield return new WaitForSeconds(0.01f);
            }
        }
    }

    private IEnumerator RemoveTarget(int _playerIndex)
    {
        while(targetGroup.m_Targets[_playerIndex].weight > 0.0f)
        {
            targetGroup.m_Targets[_playerIndex].weight -= 0.01f;
            yield return new WaitForSeconds(0.1f);
        }

        targetGroup.m_Targets[_playerIndex].target = null;
    }

    /// <summary>
    /// Call when pause or play mode has changed
    /// </summary>
    /// <param name="_pause">Set to true if pause is true</param>
    /// Rene Gamper
    public void PauseAudio()
    {
        if (isPaused)
        {
            menuSnapshot.TransitionTo(0.5f);
        }
        else
        {
            gameSnapshot.TransitionTo(0.5f);
        }
    }

    public void PauseGame()
    {
		isPaused = true;
        Time.timeScale = 0f;
        for(int i = 0; i < players.Count; i++)
        {
            players[i].Player.controllers.maps.SetAllMapsEnabled(false);
            players[i].Player.controllers.maps.SetMapsEnabled(true, Category.PAUSEMENU_CONTROL);
        }

		pauseCanvas.gameObject.SetActive(true);
		eventSystem.SetSelectedGameObject(firstSelectionPauseMenu);
		firstSelectionPauseMenu.GetComponent<Animator>().SetBool("Normal", false);
		firstSelectionPauseMenu.GetComponent<Animator>().SetBool("Highlighted", true);

		PauseAudio();
    }

    public void UnpauseGame()
    {
        isPaused = false;
        PauseAudio();
        pauseCanvas.gameObject.SetActive(false);

        for (int i = 0; i < players.Count; i++)
        {
            players[i].Player.controllers.maps.SetAllMapsEnabled(false);
            players[i].Player.controllers.maps.SetMapsEnabled(true, Category.CHARACTERCONTROL);
        }

        Time.timeScale = 1f;
    }
}