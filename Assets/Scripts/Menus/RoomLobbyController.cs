using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Linq;

namespace Tanks
{
    public class RoomLobbyController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button closeButton;

        [SerializeField] private PlayerLobbyEntry playerLobbyEntryPrefab;
        [SerializeField] private RectTransform entriesHolder;

        // TODO: Create and Delete player entries
        private Dictionary<Player, PlayerLobbyEntry> lobbyEntries; //keep tracks of our player entry

        //update the property to check that every player is ready 
        //check for all the values in lobby entries and check that player.isready is true 
        private bool IsEveryPlayerReady => lobbyEntries.Values.ToList().TrueForAll(entry => entry.IsPlayerReady);

        private void AddLobbyEntry(Player player)
        {
            var entry = Instantiate(playerLobbyEntryPrefab, entriesHolder);
            entry.Setup(player);

            // TODO: track created player lobby entries
            lobbyEntries.Add(player, entry);
        }

        private void Start()
        {
            LoadingGraphics.Disable();
            DestroyHolderChildren();

            closeButton.onClick.AddListener(OnCloseButtonClicked);
            startButton.onClick.AddListener(OnStartButtonClicked);
            startButton.gameObject.SetActive(false);

            PhotonNetwork.AutomaticallySyncScene = true; //when client calls photnnetwork.load level, every client will load the new level toogether 

            //create and initialize lobby entries dictionary 
            lobbyEntries = new Dictionary<Player, PlayerLobbyEntry>(PhotonNetwork.CurrentRoom.MaxPlayers); //creating a dictionary of the entry size of our maxplayers
            foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                AddLobbyEntry(player);
            }
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            AddLobbyEntry(newPlayer);
            UpdateStartButton();
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Destroy(lobbyEntries[otherPlayer].gameObject);
            lobbyEntries.Remove(otherPlayer);

            UpdateStartButton();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            lobbyEntries[targetPlayer].UpdateVisuals();
            UpdateStartButton();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            UpdateStartButton(); //say a master client leaves and the master is switched to someone else 
        }

        private void UpdateStartButton() //we only want it to matter when all players are ready to the master client
        {
            // TODO: Show start button only to the master client and when all players are ready
            startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient && IsEveryPlayerReady);

        }

        private void OnStartButtonClicked()
        {
            // TODO: Load gameplay level for all clients
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("You Fool! Trying to start game while not MasterClient");
                return;
            }

            PhotonNetwork.CurrentRoom.IsOpen = false; //isopen = false mean no one can enter anymore 
            PhotonNetwork.LoadLevel("Gameplay");

        }

        private void OnCloseButtonClicked()
        {
            // TODO: Leave room & go to main menu
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("MainMenu");
        }

        private void DestroyHolderChildren()
        {
            for (var i = entriesHolder.childCount - 1; i >= 0; i--) {
                Destroy(entriesHolder.GetChild(i).gameObject);
            }
        }
    }
}