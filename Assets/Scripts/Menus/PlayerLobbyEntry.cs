using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using ExitGames.Client.Photon;
using System.Collections.Generic;

namespace Tanks
{
    public class PlayerLobbyEntry : MonoBehaviour
    {
        [SerializeField] private Button readyButton;
        [SerializeField] private GameObject readyText;
        [SerializeField] private Button waitingButton;
        [SerializeField] private GameObject waitingText;

        [SerializeField] private TMP_Text playerName;
        [SerializeField] private Button changeTeamButton;
        [SerializeField] private Image teamHolder;
        [SerializeField] private List<Sprite> teamBackgrounds;
        private Player player;

        /// <summary>
        /// // TODO: Update player team to other clients
        /// </summary>
        public int PlayerTeam 
        { 
            get => player.CustomProperties.ContainsKey("Team") ? (int)player.CustomProperties["Team"] : 0;
            set
            {
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable { { "Team", value } };
                player.SetCustomProperties(hash);
            } 
        }

        /// <summary>
        /// // TODO: Update player ready status to other clients
        /// </summary>
        public bool IsPlayerReady 
        { 
            get => player.CustomProperties.ContainsKey("IsReady") && (bool)player.CustomProperties["IsReady"];
            set
            {
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable { { "IsReady", value } };
                player.SetCustomProperties(hash);

            }
        }

        /// <summary>
        /// // TODO: Get if this entry belongs to the local player
        /// </summary>
        private bool IsLocalPlayer => Equals(player, PhotonNetwork.LocalPlayer); 

        public void Setup(Player entryPlayer) //summarizes player within a room //each player is identified with an ID 
        {
            // TODO: Store and update player information
            player = entryPlayer;
            if (IsLocalPlayer)
            {
                //check player ID and player team
                PlayerTeam = (player.ActorNumber - 1) % PhotonNetwork.CurrentRoom.MaxPlayers; //ActorNumber = player ID
            }

            playerName.text = player.NickName;

            if (!IsLocalPlayer)
                Destroy(changeTeamButton);

            UpdateVisuals();
        }

        public void UpdateVisuals()
        {
            teamHolder.sprite = teamBackgrounds[PlayerTeam];

            waitingText.SetActive(!IsPlayerReady);
            readyText.SetActive(IsPlayerReady);
        }

        private void Start()
        {
            waitingButton.onClick.AddListener(() => OnReadyButtonClick(true));
            readyButton.onClick.AddListener(() => OnReadyButtonClick(false));
            changeTeamButton.onClick.AddListener(OnChangeTeamButtonClicked);

            waitingButton.gameObject.SetActive(IsLocalPlayer);
            readyButton.gameObject.SetActive(false);
        }

        private void OnChangeTeamButtonClicked()
        {
            // TODO: Change player team
            PlayerTeam = (PlayerTeam + 1) % PhotonNetwork.CurrentRoom.MaxPlayers; //% makes sure we're not overreaching that maxplayers when we switch teams
        }

        private void OnReadyButtonClick(bool isReady)
        {
            waitingButton.gameObject.SetActive(!isReady);
            waitingText.SetActive(!isReady);
            readyButton.gameObject.SetActive(isReady);
            readyText.SetActive(isReady);

            IsPlayerReady = isReady;
        }
    }
}