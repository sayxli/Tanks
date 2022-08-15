using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Tanks
{
    public class MainMenuController : MonoBehaviourPunCallbacks //also provides you with some photon callbacks and events in PU that we can call 
                                                                //we are going to implement & override some methods
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button lobbyButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private SettingsController settingsPopup;
        private Action pendingAction;

        private void Start()
        {
            // TODO: Connect to photon server
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                PhotonNetwork.ConnectUsingSettings(); //user settings are the ones that we jjust configured
            }

            //playButton.onClick.AddListener(JoinRandomRoom);
            playButton.onClick.AddListener(() => OnConnectionDependentActionClicked(JoinRandomRoom)); //telling this button, we are adding OCDAC and giving it a parameter // => makes it a single line for a return (LAMBDA)
            lobbyButton.onClick.AddListener(GoToLobbyList);
            settingsButton.onClick.AddListener(OnSettingsButtonClicked);

            settingsPopup.gameObject.SetActive(false);
            settingsPopup.Setup();

            if (!PlayerPrefs.HasKey("PlayerName"))
                PlayerPrefs.SetString("PlayerName", "Player #" + Random.Range(0, 9999));
        }

        public override void OnConnectedToMaster() //called when client is connected to master server and is ready for matchmaking
        {
            base.OnConnectedToMaster();
            Debug.Log("Connected to Master");
            pendingAction?.Invoke();
            PhotonNetwork.AutomaticallySyncScene = false; //if we are connected to master, master will dictate if everyone moves on to next scene when ready
        }

        private void OnSettingsButtonClicked()
        {
            settingsPopup.gameObject.SetActive(true);
        }

        public void JoinRandomRoom() //connecting to game server
        {
            // TODO: Connect to a random room
            RoomOptions roomOptions = new RoomOptions { IsOpen = true, MaxPlayers = 4 };
            PhotonNetwork.JoinRandomOrCreateRoom(roomOptions : roomOptions); //if a room is not found, one is created 
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            SceneManager.LoadScene("RoomLobby");
        }

        private void OnConnectionDependentActionClicked(Action action)
        {
            if(pendingAction != null)
            {
                return;
            }
            pendingAction = action;

            if (PhotonNetwork.IsConnectedAndReady)
            {
                action();
            }
        }

        private void GoToLobbyList()
        {
            SceneManager.LoadSceneAsync("LobbyList");
        }
    }
}