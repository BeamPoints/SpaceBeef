using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIExtensions;

public class PlayerPointsController : MonoBehaviour
{
    [SerializeField] private GameObject playerPointsPanel;
    [SerializeField] private Text playerNameText;
    [SerializeField] private Text playerPointsText;

    private UIDissolve panelDissolve = null;
    private UIDissolve nameDissolve = null;
    private UIDissolve pointsDissolve = null;

    private float timer = 0f;
    private float effectTimer = 0f;
    private bool effectHasStarted = false;
    private bool isDiffuseAbleToStart = false;
    private int playerPoints = 0;
    private int winningPlayerPoints = -1;
    public int WinningPlayerPoints { get { return winningPlayerPoints; } set { winningPlayerPoints = value; } }
    private int playerIndex = -1;
    public int PlayerIndex { get { return playerIndex; } set { playerIndex = value; } }
    private bool isWinner = false;
    public bool IsWinner { get { return isWinner; } set { isWinner = value; } }

    /// <summary>
    /// Muss irgendwie extra instantiated werden. Keine Ahnung warum???
    /// </summary>
    public void InstantiateDissolve()
    {
        panelDissolve = playerPointsPanel.GetComponent<UIDissolve>();
        nameDissolve = playerNameText.GetComponent<UIDissolve>();
        pointsDissolve = playerPointsText.GetComponent<UIDissolve>();
    }

    public void SetPoints(string _playerNumber, int _playerPoints, Color _playerColor)
    {
        playerNameText.color = _playerColor;
        playerPointsText.color = _playerColor;
        panelDissolve.effectFactor =    0f;
        nameDissolve.effectFactor =     0f;
        pointsDissolve.effectFactor =   0f;

        playerNameText.text = "Spieler " + _playerNumber;

        if (isWinner)
        {
            playerPointsText.text = (_playerPoints - 1).ToString();
        }
        if (!isWinner)
        {
            playerPointsText.text = (_playerPoints).ToString();

        }

        playerPoints = _playerPoints;
    }

    public void StartDissolveEffect()
    {
            panelDissolve.effectFactor =    0;
            nameDissolve.effectFactor =     0;
            pointsDissolve.effectFactor =   0;

            //playerPointsText.text = (winningPlayerPoints - 1).ToString();
            panelDissolve.Play();
            nameDissolve.Play();
            pointsDissolve.Play();
            effectHasStarted = true;
            effectTimer = 0f;
    }

    public void Update()
    {
        effectTimer += Time.deltaTime;
        if (effectHasStarted)
        {
            timer += Time.deltaTime;
            if(timer >= pointsDissolve.duration)
            {
                timer = panelDissolve.duration;
                //timer = nameDissolve.duration;
                //timer = pointsDissolve.duration;
                isDiffuseAbleToStart = true;
                effectHasStarted = false;
                playerPointsText.text = playerPoints.ToString();
            }
        }

        if (isDiffuseAbleToStart)
        {
            timer -= Time.deltaTime;
            panelDissolve.effectFactor = timer;
            nameDissolve.effectFactor = timer;
            pointsDissolve.effectFactor = timer;
            if (timer <= 0)
            {
                timer = 0;
                isDiffuseAbleToStart = false;
                effectHasStarted = false;
            }
        }
    }

}
