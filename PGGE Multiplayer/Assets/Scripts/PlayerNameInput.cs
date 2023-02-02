using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    //PlayerPrefs key for the player's name
    const string playerNamePrefKey = "PlayerName";
    private TMP_InputField mInputField;

    private void Start()
    {
        //Set player name to be the same as playerNamePrefKey
        //If playerNamePrefKey is empty, set name as empty
        string defaultName = string.Empty;
        mInputField = GetComponent<TMP_InputField>();

        if (mInputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                mInputField.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;
    }

    //Take in the value from the input field as the player's name and save it into player prefs
    public void SetPlayerName()
    {
        string value = mInputField.text; 

        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player name is null or empty");
            return;
        }

        PhotonNetwork.NickName = value;
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }
}
