using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RobbyManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();

        if (PhotonNetwork.IsConnected)
        {
            GameObject.Find("UI_Intro_FadeIn_Panel").GetComponent<Animation>().Play();
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsConnected && Input.anyKey)
        {
            Canvas introCanvas = GameObject.Find("UI_Intro").GetComponent<Canvas>();
            Canvas mainMenuCanvas = GameObject.Find("UI_MainMenu").GetComponent<Canvas>();

            introCanvas.enabled = false;
            mainMenuCanvas.enabled = true;
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

    public void OnOption()
    {
        GetComponent<AlertController>().OnEnableAlert("준비중", "현재 사용할 수 없는 메뉴입니다.");
    }

    public void OnExit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public override void OnJoinedRoom()
    {
        Debug.Log(" joined room");

        PhotonNetwork.IsMessageQueueRunning = false;
        SceneManager.LoadScene("test_ingame");
    }
}
