using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("연결됨");
        }
    }

    public void OnCreate()
    {
        PhotonNetwork.CreateRoom("test", new RoomOptions { MaxPlayers = 100 });
    }

    public void OnJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(" joined room");

        PhotonNetwork.IsMessageQueueRunning = false;
        SceneManager.LoadScene("test_ingame");
    }
}
