using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerJoinCheck : MonoBehaviour
{
    [SerializeField] PlayerSelectionManager selectionManager;

    private List<PlayerSelectionController> allJoinedPlayer;

    private void Awake()
    {
        // Reset players for game
        for (int i = 0; i < 4; i++)
            if (PlayerPrefs.HasKey("player" + i))
                PlayerPrefs.DeleteKey("player" + i);

		// Reset skins for game
		for(int i = 0; i < 4; i++)
			if(PlayerPrefs.HasKey("skin" + i))
				PlayerPrefs.DeleteKey("skin" + i);

		PlayerPrefs.SetInt("playeramount", 0);
        allJoinedPlayer = new List<PlayerSelectionController>();
    }

    private void OnTriggerEnter(Collider _other)
    {
        if(_other.CompareTag("Player"))
        {
            // Set player for game
            int playerIndex = _other.GetComponent<PlayerController>().PlayerIndex;
            PlayerSelectionController currentPlayer = selectionManager.Players[playerIndex];
            currentPlayer.PlayerJoinedGame();
            allJoinedPlayer.Add(currentPlayer);

            if (allJoinedPlayer.Count > 1)
			{
				foreach(PlayerSelectionController player in allJoinedPlayer)
				{
					player.PossibleToPlay = true;
                    player.ChangeRounds = true;

                }
			}

            currentPlayer.SetupPlayerSelectionController(playerIndex);
            PlayerPrefs.SetInt("playeramount", allJoinedPlayer.Count);
            PlayerPrefs.SetInt("player" + playerIndex, playerIndex);
            PlayerPrefs.SetInt("skin" + playerIndex, currentPlayer.SkinIndex);
            currentPlayer.JoinedGame = true;
            _other.GetComponent<Health>().GetDamage(int.MaxValue);
        }
    }
}
