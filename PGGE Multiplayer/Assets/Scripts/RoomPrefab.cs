using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;


public class RoomPrefab : MonoBehaviour
{
    public TextMeshProUGUI txtRoomName;
    public TextMeshProUGUI txtTotalPlayers;
    public Button btnJoin;

    private string mRoomName;

    public void Start()
    {
        btnJoin.onClick.AddListener(() =>
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(mRoomName);
        });
    }

    public void Initialize(string name, byte currentPlayers, byte maxPlayers)
    {
        mRoomName = name;

        txtRoomName.text = name;
        txtTotalPlayers.text = currentPlayers + " / " + maxPlayers;
    }
}
