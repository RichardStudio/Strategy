using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviourPunCallbacks
{

    public TMP_Text TimerText;
    public float TimerTime;
    private float startTime;
    private bool allReady = false;
    public Information info;
    // Start is called before the first frame update
    void Start()
    {
        photonView.RPC("SendData", RpcTarget.OthersBuffered, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!allReady || info.endGame) return;

        TimerTime = Time.time - startTime;

        string minutes = ((int)TimerTime / 60).ToString();
        string seconds = (TimerTime % 60).ToString("f2");

        TimerText.text = minutes + ":" + seconds;
    }

    [PunRPC]
    private void SendData(bool ready)
    {
        if (PhotonNetwork.IsConnectedAndReady && ready)
        {
            StartTimer();
            allReady = true;
        }
    }

    void StartTimer()
    {
        startTime = Time.time;
    }
}
