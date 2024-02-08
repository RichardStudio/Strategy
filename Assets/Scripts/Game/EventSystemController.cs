using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EventSystemController : MonoBehaviourPunCallbacks
{

    public GameObject MasterPanel;
    public GameObject OpponentPanel;

    // Start is called before the first frame update
    void Start()
    {
        MasterPanel.SetActive(false);
        OpponentPanel.SetActive(false);
        if (PhotonNetwork.IsMasterClient)
        {
            MasterPanel.SetActive(true);
        }
        else
        {
            OpponentPanel.SetActive(true);
        }
    }

    public void ChooseBase(int id)
    {
        if (id == 0 && PhotonNetwork.IsMasterClient)
        {
            MasterPanel.SetActive(true);

        }
        if (id == 1 && !PhotonNetwork.IsMasterClient)
        {
            OpponentPanel.SetActive(true);
        }
    }

    public void BackToLobby()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Lobby");
    }
}
