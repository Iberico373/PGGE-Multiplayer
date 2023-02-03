using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

namespace PGGE.Multiplayer
{
    public class ConnectionController : MonoBehaviourPunCallbacks
    {
        public GameObject mLoginPanel;

        public GameObject mSelectionPanel;
        public GameObject mRoomListContent;
        public GameObject mRoomListPrefab;        

        private Dictionary<string, RoomInfo> mCachedRoomList;
        private Dictionary<string, GameObject> mRoomListEntries;

        //To keep track of the current version of the game so that
        //we don't clash with different versions of the game
        const string gameVersion = "1";

        public byte maxPlayersPerRoom = 4;

        public GameObject mConnectionProgress;
        bool isConnecting = false;

        private void Awake()
        {
            //This ensures we can use PhotonNetwork.LoadLevel() on 
            //the master client and all clients in the same 
            //room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
            mCachedRoomList = new Dictionary<string, RoomInfo>();
            mRoomListEntries = new Dictionary<string, GameObject>();

            ActivatePanel(mLoginPanel.name);
            mConnectionProgress.SetActive(false);
        }

        //Attempt to connect to photon's server
        public void Connect()
        {
            if (PhotonNetwork.IsConnected)
            {
                OnConnectedToMaster();
                return;
            }

            PhotonNetwork.ConnectUsingSettings();
            ActivatePanel();
            mConnectionProgress.SetActive(true);            
        }

        //Join a random open room
        public void JoinRandomRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        //Return back to main menu scene when called
        public void BackToMainMenu()
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            SceneManager.LoadScene("Menu");
        }

        //Contains all PUN related callbacks
        #region PUN Callbacks
        //Displays a list of available rooms when connected to proton's server
        public override void OnConnectedToMaster()
        {            
            ActivatePanel(mSelectionPanel.name);
            mConnectionProgress.SetActive(false);

            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
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

        //Called whenever any room gets updated in the lobby
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            ClearRoomListView();

            UpdateRoomList(roomList);
            UpdateRoomListView();
        }

        //Whenever the player joins a new lobby, clear any previous room lists
        public override void OnJoinedLobby()
        {
            mCachedRoomList.Clear();
            ClearRoomListView();
        }

        public override void OnLeftLobby()
        {
            mCachedRoomList.Clear();
            ClearRoomListView();
        }
        #endregion

        //Activates panel requested in the argument and deactivates
        //all other active panels
        public void ActivatePanel(string panelName = "None")
        {
            mLoginPanel.SetActive(panelName.Equals(mLoginPanel.name));
            mSelectionPanel.SetActive(panelName.Equals(mSelectionPanel.name));
        }

        //Clears room list
        void ClearRoomListView()
        {
            foreach (GameObject entry in mRoomListEntries.Values)
            {
                Destroy(entry.gameObject);
            }

            mRoomListEntries.Clear();
        }

        //Update cached room list
        void UpdateRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                //Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
                {
                    if (mCachedRoomList.ContainsKey(info.Name))
                    {
                        mCachedRoomList.Remove(info.Name);
                    }

                    continue;
                }

                //Update current room info 
                if (mCachedRoomList.ContainsKey(info.Name))
                {
                    mCachedRoomList[info.Name] = info;
                }
                
                else
                {
                    mCachedRoomList.Add(info.Name, info);
                }
            }
        }

        //Updates room list UI element in order to reflect
        //the cached room list
        void UpdateRoomListView()
        {
            foreach (RoomInfo info in mCachedRoomList.Values)
            {
                GameObject entry = Instantiate(mRoomListPrefab);
                entry.transform.SetParent(mRoomListContent.transform);
                entry.transform.localScale = Vector3.one;
                entry.GetComponent<RoomPrefab>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);

                mRoomListEntries.Add(info.Name, entry);
            }
        }
    }
}

