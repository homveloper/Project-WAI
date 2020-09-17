using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    Canvas introCanvas;
    Canvas mainMenuCanvas;
    Canvas roomCanvas;

    FadeController fadeController;

    int menuCode = MENU_INTRO;
    const int MENU_INTRO = 0;
    const int MENU_MAINMENU = 10;
    const int MENU_ROOM = 20;

    bool event_roomJoined = false;

    void Start()
    {
        fadeController = GetComponent<FadeController>();
        fadeController.OnBlack();

        introCanvas = GameObject.Find("UI_Intro").GetComponent<Canvas>();
        mainMenuCanvas = GameObject.Find("UI_MainMenu").GetComponent<Canvas>();
        roomCanvas = GameObject.Find("UI_Room").GetComponent<Canvas>();

        PhotonNetwork.ConnectUsingSettings();

        if (PhotonNetwork.IsConnected)
        {
            introCanvas.enabled = true;
            fadeController.OnFadeIn();
        }
    }

    void Update()
    {
        // 인트로에서 키를 누르면 메인 메뉴로 전환
        if (menuCode == MENU_INTRO && PhotonNetwork.IsConnected && Input.anyKey)
        {
            menuCode = MENU_MAINMENU;
            introCanvas.enabled = false;
            mainMenuCanvas.enabled = true;

            fadeController.OnWhite();
        }

        // 메인메뉴에서 방에 입장하면 로비로 전환
        if (menuCode == MENU_MAINMENU && event_roomJoined == true && fadeController.IsPlaying() == false)
        {
            menuCode = MENU_ROOM;
            mainMenuCanvas.enabled = false;
            roomCanvas.enabled = true;

            fadeController.OnFadeIn();
        }
    }

    public void OnCreate()
    {
        fadeController.OnBlack();
        PhotonNetwork.CreateRoom("test", new RoomOptions { MaxPlayers = 100 });
    }

    public override void OnCreatedRoom()
    {

    }

    public void OnJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        fadeController.OnBlack();
        event_roomJoined = true;
        PhotonNetwork.IsMessageQueueRunning = false;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        GetComponent<AlertController>().OnEnableAlert("방 없음", "현재 방이 없어 랜덤 참가가 불가능합니다.");
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
}
