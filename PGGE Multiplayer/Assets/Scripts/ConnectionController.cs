using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PGGE.Multiplayer
{
    public class ConnectionController : MonoBehaviourPunCallbacks
    {
        //To keep track of the current version of the game so that
        //we don't clash with different versions of the game
        const string gameVersion = "1";

        public byte maxPlayersPerRoom = 4;

        public GameObject mConnectionProgress;
        public GameObject mBtnJoinRoom;
        public GameObject mInpPlayerName;
        bool isConnecting = false;

        private void Awake()
        {
            //This ensures we can use PhotonNetwork.LoadLevel() on 
            //the master client and all clients in the same 
            //room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        private void Start()
        {
            mConnectionProgress.SetActive(false);
        }

        //Checks if we are connected to the photon server or not
        //If yes, then join a random room
        //Otherwise, attempt to connect to the photon server
        public void Connect()
        {
            mBtnJoinRoom.SetActive(false); 
            mInpPlayerName.SetActive(false);
            mConnectionProgress.SetActive(true);

            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinRandomRoom();
            }

            else
            {
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }

        public override void OnConnectedToMaster()
        {
            //Join a random room when connected to the photon server
            if (isConnecting)
            {
                Debug.Log("OnConnectedToMaster() was called by PUN");
                PhotonNetwork.JoinRandomRoom();
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
            isConnecting = false;
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("OnJoinRandomFailed() was called by PUN." +
                "No random room available" +
                ", so we create by calling: " +
                "PhotonNetwork.CreateRoom()");

            //Failed to join a random room.
            //This may happen if no room exists or 
            //they are all full. In either case, we create a new room.
            PhotonNetwork.CreateRoom(null, new RoomOptions{MaxPlayers = maxPlayersPerRoom});
        }

        //Load scene named "MultiplayerMap00" for the player that joined the room
        public override void OnJoinedRoom()
        {
            Debug.Log("OnJoinedRoom() called by PUN. Client is in a room");

            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("We load the default room for multiplayer");
                PhotonNetwork.LoadLevel("MultiplayerMap00");
            }
        }
    }
}

