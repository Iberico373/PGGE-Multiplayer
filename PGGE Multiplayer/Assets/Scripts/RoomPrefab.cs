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
    public TextMeshProUGUI txtJoin;
    public GameObject btnJoin;

    private string mRoomName;
    private Color greenColor = new Color(92, 255, 101, 255);
    private Color redColor = new Color(255, 92, 102, 255);

    public void Start()
    {
        btnJoin.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (PhotonNetwork.InLobby)
            {
                PhotonNetwork.LeaveLobby();
            }

            PhotonNetwork.JoinRoom(mRoomName);
        });
    }

    private void Update()
    {
        
    }

    //Initialize prefab with appropriate room information
    public void Initialize(string name, byte currentPlayers, byte maxPlayers)
    {
        mRoomName = name;

        txtRoomName.text = name;
        txtTotalPlayers.text = currentPlayers + " / " + maxPlayers;

        //If room is full set button text to "Full", and set button color to red
        if (currentPlayers >= maxPlayers)
        {
            txtJoin.text = "Full";
            btnJoin.GetComponent<Image>().color = redColor;
        }

        //Otherwise, set button text to "Join", and set button color to green
        else
        {
            txtJoin.text = "Join";
            btnJoin.GetComponent<Image>().color = greenColor;
        }
    }
}
