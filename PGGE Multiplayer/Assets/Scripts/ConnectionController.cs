using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

namespace PGGE.Multiplayer
{
    enum FilterType
    {
        UNFILTERED,
        NAME,
    }

    public class ConnectionController : MonoBehaviourPunCallbacks
    {
        public GameObject mLoginPanel;

        public GameObject mSelectionPanel;
        public GameObject mRoomListContent;
        public GameObject mRoomListPrefab;
        public TMP_InputField mNameSearchInput;

        public GameObject mCreateRoomPanel;
        public TMP_InputField mRoomNameInput;
        public TMP_InputField mMaxPlayerInput;

        //Dictionary containing the room info list of every room on the 
        //proton server 
        private Dictionary<string, RoomInfo> mCachedRoomList;
        //Dicitonary containing the room info list once it's filtered 
        private Dictionary<string, RoomInfo> mFilteredCachedRoomList;
        //Dictionary containing the UI element of the room list
        private Dictionary<string, GameObject> mRoomListEntries;

        //To keep track of the current version of the game so that
        //we don't clash with different versions of the game
        const string gameVersion = "1";

        public byte maxPlayersPerRoom = 4;

        public GameObject mConnectionProgress;
        FilterType mCurrentFilter = FilterType.UNFILTERED;
        bool isConnecting = false;

        private void Awake()
        {
            //This ensures we can use PhotonNetwork.LoadLevel() on 
            //the master client and all clients in the same 
            //room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;
            mCachedRoomList = new Dictionary<string, RoomInfo>();
            mFilteredCachedRoomList = new Dictionary<string, RoomInfo>();
            mRoomListEntries = new Dictionary<string, GameObject>();

            ActivatePanel(mLoginPanel.name);
            mConnectionProgress.SetActive(false);
        }

        //Contains all functions called by buttons
        #region Button Functions
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
        #endregion 

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
            PhotonNetwork.CreateRoom("Room " + Random.Range(1, 1000).ToString(), new RoomOptions { MaxPlayers = maxPlayersPerRoom });
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

        //Reconnects player back to lobby if joining a room failed
        //(e.g., when a room is full, the player got disconnected, etc.)
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
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
            mCreateRoomPanel.SetActive(panelName.Equals(mCreateRoomPanel.name));
        }

        //Filters room list by name
        public void FilterRoomByName()
        {
            //Clear the filtered dictionary in order to prevent duplicates 
            //from being added
            mFilteredCachedRoomList.Clear();
            string roomName = mNameSearchInput.text;

            //If player empties the name search, set list display to be unfiltered
            //and return from function
            if (roomName.Equals(string.Empty))
            {
                mCurrentFilter = FilterType.UNFILTERED;
                ClearRoomListView();
                UpdateRoomListView();

                return;
            }

            //Otherwise set filter type to be based on the room's name
            mCurrentFilter = FilterType.NAME;

            //Compare every room name in the cached room list to the one 
            //inputted by the player
            foreach (string name in mCachedRoomList.Keys)
            {
                //If room name contains characters from the player inputted name
                //add that room to the filtered dictionary
                if (name.Contains(roomName))
                {
                    mFilteredCachedRoomList.Add(name, mCachedRoomList[name]);
                }
            }

            ClearRoomListView();
            UpdateRoomListView();
        }

        //Create a room in the PUN lobby
        public void CreateRoom()
        {
            //Set room name according to what the player inputted
            //If room name is left empty set room name to a default name
            string roomName = mRoomNameInput.text;
            roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1, 1000) : roomName;

            //Set room's max player based on player's input
            byte maxPlayers;
            byte.TryParse(mMaxPlayerInput.text, out maxPlayers);
            //Clamp players input between 2 to 8, meaning that
            //total number of players cannot go below 2 or exceed 8 
            maxPlayers = (byte)Mathf.Clamp(maxPlayers, 2, 8);

            RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };

            PhotonNetwork.CreateRoom(roomName, options);
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
            Debug.Log("Current filter type: " + mCurrentFilter);

            foreach (RoomInfo info in roomList)
            {
                //Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (!info.IsVisible || info.RemovedFromList)
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

            if (mCurrentFilter == FilterType.NAME)
            {
                FilterRoomByName();
            }
        }

        //Updates room list UI element in order to reflect
        //the cached room list
        void UpdateRoomListView()
        {
            //Display unfiltered list when current filter type is unfiltered
            if (mCurrentFilter == FilterType.UNFILTERED)
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

            //Otherwise display the filtered list
            else
            {
                foreach (RoomInfo info in mFilteredCachedRoomList.Values)
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
}

