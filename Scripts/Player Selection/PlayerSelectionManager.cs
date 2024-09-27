using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ProgressionSystem;

public class PlayerSelectionManager : MonoBehaviour
{
    [SerializeField] private List<PlayerSelectionCanvasController> playerCanvas;
    [SerializeField] private List<Transform> playerSpawns;
    [SerializeField] private PlayerSelectionController playerSelectionPrefab;
    [SerializeField] private AudioSource skinSelectedSound;
    [SerializeField] private AudioSource skinNotAvailabeSound;
    [SerializeField] private string nextScene;
    [SerializeField] private Text roundAmountText; 

    private Dictionary<ProgressionSkinData, bool> allSkins;
    public Dictionary<ProgressionSkinData, bool> AllSkins { get { return allSkins; } }

    private List<PlayerSelectionController> players;
    public List<PlayerSelectionController> Players { get { return players; } }
    
    public AudioSource SkinSelectedSound { get { return skinSelectedSound; } }
    public AudioSource SkinNotAvailabeSound { get { return skinNotAvailabeSound; } }

    private int roundAmount = 5;

    private void Awake()
    {
        players = new List<PlayerSelectionController>();
        allSkins = new Dictionary<ProgressionSkinData, bool>();

        if (PlayerPrefs.HasKey("rounds"))
        {
            PlayerPrefs.DeleteKey("rounds");
        }
        if (PlayerPrefs.HasKey("roundsPlayed"))
        {
            PlayerPrefs.DeleteKey("roundsPlayed");
        }
        SaveRounds(roundAmount);

        for (int i = 0; i < 4; i++)
            if (PlayerPrefs.HasKey("player" + i + "points"))
                PlayerPrefs.DeleteKey("player" + i + "points");
    }

    private void Start()
    {
        foreach (var skins in ProgressionManager.Instance.SkinPool)
        {
            if(!allSkins.ContainsKey(skins))
                allSkins.Add(skins, true);
        }

        for (int i = 0; i < 4; i++)
        {
            PlayerSelectionController player = Instantiate(playerSelectionPrefab, playerSpawns[i].position, Quaternion.identity);
            player.SelectionManager = this;
            player.PlayerCanvas = playerCanvas[i];
            player.SetupPlayerSelectionController(i);
            players.Add(player);
        }
    }

    private void SaveRounds(int _rounds)
    {
        roundAmountText.text = _rounds + "";
        PlayerPrefs.SetInt("rounds", _rounds);
    }

    public void ChangeRoundAmount(bool _increment)
    {
        if (_increment)
        {
            roundAmount++;
        }
        else
        {
            roundAmount--;
        }

        if (roundAmount > 99)
        {
            roundAmount = 99;
        }

        if (roundAmount < 1)
        {
            roundAmount = 1;
        }

        SaveRounds(roundAmount);
    }

    public bool IsSkinAvailable(ProgressionSkinData _skin)
    {
        return allSkins[_skin];
    }

    public bool IsSkinAvailable(int _skinIndex)
    {
        return allSkins.Values.ElementAt(_skinIndex);
    }

    public int GetIndexBySkin(ProgressionSkinData _skin)
    {
        return allSkins.Values.ToList().IndexOf(_skin);
    }

    public ProgressionSkinData GetSkinByIndex(int _skinIndex)
    {
        ProgressionSkinData skin = allSkins.Keys.ElementAt(_skinIndex);
        return skin;
    }

    public ProgressionSkinData SelectSkin(int _skinIndex)
    {
        ProgressionSkinData skin = allSkins.Keys.ElementAt(_skinIndex);
        allSkins[skin] = false;
        return skin;
    }

    public void DeselectSkin(int _skinIndex)
    {
        ProgressionSkinData skin = allSkins.Keys.ElementAt(_skinIndex);
        allSkins[skin] = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(nextScene);
    }
}
