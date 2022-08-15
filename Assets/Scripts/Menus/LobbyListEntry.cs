using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

namespace Tanks
{
    public class LobbyListEntry : MonoBehaviour
    {
        [SerializeField] private Button enterButton;
        [SerializeField] private TMP_Text lobbyNameText;
        [SerializeField] private TMP_Text lobbyPlayerCountText;

        private RoomInfo roomInfo;

        private void OnEnterButtonClick()
        {
            LoadingGraphics.Enable();
            PhotonNetwork.JoinRoom(roomInfo.Name); //targets rooms using their name

            // TODO: Join target room
        }

        public void Setup(RoomInfo info)
        {
            // TODO: Store and update room information
            roomInfo = info;

            lobbyNameText.text = info.Name;
            lobbyPlayerCountText.text = $"{info.PlayerCount}/{info.MaxPlayers}";
        }

        private void Start()
        {
            enterButton.onClick.AddListener(OnEnterButtonClick);
        }
    }
}