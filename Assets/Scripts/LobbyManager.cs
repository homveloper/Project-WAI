using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExitGames.Client.Photon;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    FadeController fadeController;
    AlertController alertController;

    int menuCode = MENU_NONE;
    const int MENU_NONE = 0;
    const int MENU_INTRO = 1;
    const int MENU_MAINMENU = 11;
    const int MENU_ROOM = 21;

    bool event_roomJoined = false;

    // ---------------------------------------------------------------------------------------------------
    // 공통
    // ---------------------------------------------------------------------------------------------------
    void Start()
    {
        alertController = GetComponent<AlertController>();
        fadeController = GetComponent<FadeController>();

        menuCode = MENU_INTRO;

        fadeController.OnBlack();
        GameObject.Find("UI_Intro").GetComponent<Canvas>().enabled = true;
        GameObject.Find("UI_MainMenu").GetComponent<Canvas>().enabled = false;
        GameObject.Find("UI_Room").GetComponent<Canvas>().enabled = false;
      
        fadeController.OnFadeIn();
    }

    void Update()
    {
        // 인트로에서 메인메뉴로 전환
        if (menuCode == MENU_INTRO && Input.anyKey)
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.NickName = "GUEST #" + Random.Range(1, 9999);
                PhotonNetwork.ConnectUsingSettings();
            }
            else
            {
                PhotonNetwork.Reconnect();
            }
        }

        // 메인메뉴에서 대기실로 전환
        else if (menuCode == MENU_MAINMENU && event_roomJoined == true && fadeController.IsPlaying() == false)
        {
            menuCode = MENU_ROOM;
            GameObject.Find("UI_MainMenu").GetComponent<Canvas>().enabled = false;
            GameObject.Find("UI_Room").GetComponent<Canvas>().enabled = true;

            fadeController.OnFadeIn();
        }

        // 대기실 마우스
        if (menuCode == MENU_ROOM)
        {
            //Vector2 mousePosition = Input.mousePosition.normalized * 10;

            //GameObject.Find("UI_Room_Back").GetComponent<RectTransform>().anchoredPosition = mousePosition;
        }

        // 대기실 게임 준비 (F5)
        if (menuCode == MENU_ROOM && Input.GetKeyDown(KeyCode.F5) == true)
        {
            OnRoomReady();
        }

        // 대기실 퇴장 (F4)
        else if (menuCode == MENU_ROOM && Input.GetKeyDown(KeyCode.F4) == true)
        {
            OnRoomExit();
        }

        // 대기실 채팅 (엔터)
        else if (menuCode == MENU_ROOM && (Input.GetKeyDown(KeyCode.Return) == true || Input.GetKeyDown(KeyCode.KeypadEnter) == true))
        {
            OnSendChat();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        alertController.OnEnableAlert("연결 끊어짐", "...");

        GameObject.Find("UI_Intro").GetComponent<Canvas>().enabled = true;
        GameObject.Find("UI_MainMenu").GetComponent<Canvas>().enabled = false;
        GameObject.Find("UI_Room").GetComponent<Canvas>().enabled = false;
    }
    // ---------------------------------------------------------------------------------------------------
    // 인트로
    // ---------------------------------------------------------------------------------------------------
    public override void OnConnectedToMaster() // 포톤 서버와 연결됬을 때 호출되는 콜백 함수
    {
        base.OnConnectedToMaster();

        if (menuCode != MENU_INTRO) fadeController.OnBlack();
        GameObject.Find("UI_Intro").GetComponent<Canvas>().enabled = false;
        GameObject.Find("UI_MainMenu").GetComponent<Canvas>().enabled = true;
        GameObject.Find("UI_Room").GetComponent<Canvas>().enabled = false;

        if (menuCode != MENU_INTRO) fadeController.OnFadeIn();

        menuCode = MENU_MAINMENU;
        PhotonNetwork.IsMessageQueueRunning = true;
    }

    // ---------------------------------------------------------------------------------------------------
    // 메인메뉴
    // ---------------------------------------------------------------------------------------------------
    public void OnCreate() // 방 생성 버튼 함수
    {
        fadeController.OnBlack();
        PhotonNetwork.CreateRoom("테스트 대기실 #" + Random.Range(1, 9999), new RoomOptions { MaxPlayers = 10, BroadcastPropsChangeToAll = true });
    }
    public void OnJoin() // 참여 버튼 함수
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void OnOption() // 설정 버튼 함수
    {
        alertController.OnEnableAlert("준비중", "현재 사용할 수 없는 메뉴입니다.");
    }

    public void OnExit() // 종료 버튼 함수
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public override void OnCreateRoomFailed(short returnCode, string message) // 방 생성에 실패했을 때 호출되는 콜백 함수
    {
        base.OnCreateRoomFailed(returnCode, message);

        alertController.OnEnableAlert("방 생성 실패", message + " (" + returnCode + ")");
    }

    public override void OnJoinRandomFailed(short returnCode, string message) // 방 입장에 실패했을 때 호출되는 콜백 함수
    {
        base.OnCreateRoomFailed(returnCode, message);

        alertController.OnEnableAlert("방 입장 실패", message + " (" + returnCode + ")");
    }


    // ---------------------------------------------------------------------------------------------------
    // 대기실 - 공통
    // ---------------------------------------------------------------------------------------------------
    public override void OnJoinedRoom() // 방 입장에 성공했을때 호출되는 콜백 함수
    {
        base.OnJoinedRoom();

        fadeController.OnBlack();
        GameObject.Find("UI_MainMenu").GetComponent<Canvas>().enabled = false;
        GameObject.Find("UI_Room").GetComponent<Canvas>().enabled = true;

        menuCode = MENU_ROOM;
        fadeController.OnFadeIn();

        ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;

        localProp["color"] = 1;
        localProp["isReady"] = false;
        localProp["isStart"] = false;

        PhotonNetwork.LocalPlayer.SetCustomProperties(localProp);

        RefreshRoomUI();

        GameObject.Find("UI_Room_Ready").GetComponent<Button>().interactable = true;
        GameObject.Find("UI_Room_Exit").GetComponent<Button>().interactable = true;

        GameObject.Find("UI_Room_Chat_Text").GetComponent<Text>().text = "";
        GameObject.Find("UI_Room_Chat_Input").GetComponent<InputField>().ActivateInputField();
    }

    public void OnRoomReady() // 게임 준비(시작) 버튼의 클릭 함수
    {
        if (GameObject.Find("UI_Room_Ready").GetComponent<Button>().interactable == false)
            return;

        if (PhotonNetwork.LocalPlayer.IsMasterClient == true)
        {
            photonView.RPC("OnStart", RpcTarget.AllBuffered);
        }
        else
        {
            ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;
            if (localProp.ContainsKey("isReady") == false) localProp.Add("isReady", true);
            else if ((bool)localProp["isReady"] == true) localProp["isReady"] = false;
            else if ((bool)localProp["isReady"] == false) localProp["isReady"] = true;

            PhotonNetwork.LocalPlayer.SetCustomProperties(localProp);

            RefreshRoomUI();
        }
    }

    [PunRPC]
    public void OnStart() // 게임 시작 함수
    {
        GameObject.Find("UI_Room_Exit").GetComponent<Button>().interactable = false;

        SceneManager.LoadScene("proto_field_ver2");
    }

    public void OnRoomExit() // 나가기 버튼의 클릭 함수
    {
        if (GameObject.Find("UI_Room_Exit").GetComponent<Button>().interactable == false)
            return;

        ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;
        localProp.Remove("isReady");
        PhotonNetwork.LocalPlayer.SetCustomProperties(localProp);

        PhotonNetwork.LeaveRoom();
    }

    public void RefreshRoomUI() // 대기실 UI를 갱신하는 함수
    {
        int countOfReady = 0; // 준비한 플레이어 수

        Photon.Realtime.Player[] player = PhotonNetwork.PlayerList;

        for (int i = 0; i < player.Length; i++)
        {
            ExitGames.Client.Photon.Hashtable prop = player[i].CustomProperties;
            GameObject playerPanel = GameObject.Find("UI_Room_Player_" + i);

            playerPanel.GetComponent<Image>().enabled = true;
            playerPanel.transform.Find("UI_Room_Player_Nickname").gameObject.GetComponent<Text>().text = player[i].NickName;

            if (prop.ContainsKey("color"))
                playerPanel.transform.Find("UI_Room_Player_Color").gameObject.GetComponent<Image>().color = GameObject.Find("UI_Room_Palette_" + (int)prop["color"]).GetComponent<Image>().color;

            if (player[i].IsMasterClient == true)
            {
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Animation>().Stop();
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Image>().fillAmount = 0;
                playerPanel.transform.Find("UI_Room_Player_Status").gameObject.GetComponent<Text>().text = "방장";
                countOfReady++;
            }
            else if (prop.ContainsKey("isReady") && (bool)prop["isReady"] == true)
            {
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Animation>().Stop();
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Image>().fillAmount = 1;
                playerPanel.transform.Find("UI_Room_Player_Status").gameObject.GetComponent<Text>().text = "준비";
                countOfReady++;
            }
            else
            {
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Animation>().Stop();
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Image>().fillAmount = 0;
                playerPanel.transform.Find("UI_Room_Player_Status").gameObject.GetComponent<Text>().text = "";
            }
        }

        for (int i = player.Length; i < 10; i++)
        {
            GameObject playerPanel = GameObject.Find("UI_Room_Player_" + i);

            playerPanel.GetComponent<Image>().enabled = false;
            playerPanel.transform.Find("UI_Room_Player_Nickname").gameObject.GetComponent<Text>().text = "";
            playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Image>().fillAmount = 0;
            playerPanel.transform.Find("UI_Room_Player_Status").gameObject.GetComponent<Text>().text = "";
            playerPanel.transform.Find("UI_Room_Player_Color").gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }

        GameObject.Find("UI_Room_Title_Text").GetComponent<Text>().text = PhotonNetwork.CurrentRoom.Name;
        GameObject.Find("UI_Room_Title_Count").GetComponent<Text>().text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        if (PhotonNetwork.LocalPlayer.IsMasterClient == true)
        {
            GameObject.Find("UI_Room_Ready_Text").GetComponent<Text>().text = "게임 시작";

            if (countOfReady >= PhotonNetwork.CurrentRoom.PlayerCount) GameObject.Find("UI_Room_Ready").GetComponent<Button>().interactable = true;
            else GameObject.Find("UI_Room_Ready").GetComponent<Button>().interactable = false;
        }
        else
        {
            GameObject.Find("UI_Room_Ready_Text").GetComponent<Text>().text = "게임 준비";
            GameObject.Find("UI_Room_Ready").GetComponent<Button>().interactable = true;
        }

        ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;

        if (localProp.ContainsKey("isReady") && (bool)localProp["isReady"] == true) GameObject.Find("UI_Room_Ready_Highlight").GetComponent<Image>().fillAmount = 1;
        else GameObject.Find("UI_Room_Ready_Highlight").GetComponent<Image>().fillAmount = 0;
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) // 방에 사람이 들어왔을 때의 콜백 함수
    {
        base.OnPlayerEnteredRoom(newPlayer);

        RefreshRoomUI();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) // 방에 사람이 나갔을 때의 콜백 함수
    {
        base.OnPlayerLeftRoom(otherPlayer);

        RefreshRoomUI();
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps) // 플레이어의 프로퍼티가 변경됬을 때의 콜백 함수
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        RefreshRoomUI();

        Photon.Realtime.Player[] player = PhotonNetwork.PlayerList;

        for (int i = 0; i < player.Length; i++)
        {
            if (player[i] != targetPlayer) continue;

            ExitGames.Client.Photon.Hashtable prop = player[i].CustomProperties;
            GameObject playerPanel = GameObject.Find("UI_Room_Player_" + i);

            if (prop.ContainsKey("isReady") && (bool)prop["isReady"] == true)
            {
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Animation>().Play("Player_ready");
            }
        }
    }

    public override void OnLeftRoom() // 방에서 나갔을때 호출되는 콜백 함수
    {
        base.OnLeftRoom();

        fadeController.OnBlack();
        GameObject.Find("UI_Room").GetComponent<Canvas>().enabled = false;
        GameObject.Find("UI_MainMenu").GetComponent<Canvas>().enabled = true;

        menuCode = MENU_MAINMENU;
        fadeController.OnFadeIn();
    }

    // ---------------------------------------------------------------------------------------------------
    // 대기실 - 채팅
    // ---------------------------------------------------------------------------------------------------
    public void OnSendChat() // 입력한 채팅을 송신하는 함수
    {
        InputField field = GameObject.Find("UI_Room_Chat_Input").GetComponent<InputField>();

        field.ActivateInputField();

        if (field.text != "")
        {
            photonView.RPC("OnChat", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName + " : " + field.text);
            field.text = "";
            field.ActivateInputField();
        }
    }

    [PunRPC]
    public void OnChat(string message) // RPC로 채팅을 수신하는 함수
    {
        Text chat = GameObject.Find("UI_Room_Chat_Text").GetComponent<Text>();
        chat.text = chat.text + "\n" + message;
    }

    // ---------------------------------------------------------------------------------------------------
    // 대기실 - 팔레트
    // ---------------------------------------------------------------------------------------------------
    public void OnChangeColor(int colorIndex) // 선택한 색으로 변경
    {
        if (GameObject.Find("UI_Room_Palette_" + colorIndex).GetComponent<Button>().interactable == false)
            return;

        ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;

        if (localProp.ContainsKey("isReady") && (bool)localProp["isReady"] == true)
            return;

        localProp["color"] = colorIndex;

        PhotonNetwork.LocalPlayer.SetCustomProperties(localProp);

        RefreshRoomUI();
    }
}
