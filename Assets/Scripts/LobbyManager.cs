using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public bool DEBUG_LOBBY = true;

    FadeController fadeController;
    AlertController alertController;
    public PlayerColorPalette playerColors;
    public GameObject UI_Room_Palette_Prefab;

    GameObject[] UI_Room_Palettes;

    int menuCode = MENU_NONE;
    const int MENU_NONE = 0;
    const int MENU_INTRO = 1;
    const int MENU_MAINMENU = 11;
    const int MENU_ROOM = 21;

    int lobbyPage = 1;
    List<RoomInfo> lobbyRoom = null;
    RoomInfo selectedRoom;

    bool event_roomJoined = false;

    // ---------------------------------------------------------------------------------------------------
    // 공통
    // ---------------------------------------------------------------------------------------------------
    void Start()
    {
        alertController = GetComponent<AlertController>();
        fadeController = GetComponent<FadeController>();
        GameObject.Find("UI_Nickname_Input").GetComponent<InputField>().text = PhotonNetwork.NickName;

        if (PhotonNetwork.InRoom == true) // 게임이 종료되어 퇴장하여 신이 로드된 상황 (=이미 방에 포함된 경우)
        {
            PhotonNetwork.IsMessageQueueRunning = true;
            GameObject.Find("UI_Room_Ready").GetComponent<Button>().interactable = false;
            OnJoinedRoomCall();
        }
        else
        {
            menuCode = MENU_INTRO;

            fadeController.OnBlack();
            GameObject.Find("UI_Intro").GetComponent<Canvas>().enabled = true;
            GameObject.Find("UI_MainMenu").GetComponent<Canvas>().enabled = false;
            GameObject.Find("UI_Room").GetComponent<Canvas>().enabled = false;
            fadeController.OnFadeIn();
        }
    }

    void Update()
    {
        // [디버깅용] 디버그 모드 전환
        if (Input.GetKeyDown(KeyCode.F12) == true)
        {
            DEBUG_LOBBY = !DEBUG_LOBBY;
        }

        // 인트로에서 메인메뉴로 전환
        if (menuCode == MENU_INTRO && Input.anyKey)
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.NickName = "GUEST #" + UnityEngine.Random.Range(1, 9999);
                GameObject.Find("UI_Nickname_Input").GetComponent<InputField>().text = PhotonNetwork.NickName;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        if (menuCode == MENU_MAINMENU && (Input.GetKeyDown(KeyCode.Return) == true || Input.GetKeyDown(KeyCode.KeypadEnter) == true))
            OnChangeNickname();

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
        if (PhotonNetwork.InLobby == false) PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        if (menuCode != MENU_MAINMENU) fadeController.OnBlack();
        GameObject.Find("UI_Intro").GetComponent<Canvas>().enabled = false;
        GameObject.Find("UI_MainMenu").GetComponent<Canvas>().enabled = true;
        GameObject.Find("UI_Room").GetComponent<Canvas>().enabled = false;

        if (menuCode != MENU_MAINMENU) fadeController.OnFadeIn();

        menuCode = MENU_MAINMENU;
        PhotonNetwork.IsMessageQueueRunning = true;
    }
    // ---------------------------------------------------------------------------------------------------
    // 메인메뉴
    // ---------------------------------------------------------------------------------------------------
    public void OnMenuCreate() // 방 생성
    {
        GameObject.Find("UI_MainMenu_CreateRoom_RoomTitle_InputField").GetComponent<InputField>().text = "";
        GameObject.Find("UI_MainMenu_CreateRoom_Public_InputField").GetComponent<InputField>().text = "";
        GameObject.Find("UI_MainMenu_CreateRoom").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    public void OnMenuQuickPlay() // 빠른 참여
    {
        GameObject.Find("UI_MainMenu_Join").GetComponent<Button>().interactable = false;
        lobbyPage = 1;
        lobbyRoom = null;

        ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
        prop["isPrivate"] = false;

        PhotonNetwork.JoinRandomRoom(prop, 0);
    }

    public void OnMenuExplore()
    {
        GameObject.Find("UI_ExploreRoom").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    public void OnMenuOption() // 설정
    {
        alertController.OnEnableAlert("준비중", "현재 사용할 수 없는 메뉴입니다.");
    }

    public void OnMenuExit() // 종료 버튼 함수
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    public void OnClickNickName()
    {
        InputField field = GameObject.Find("UI_Nickname_Input").GetComponent<InputField>();

        field.text = "";
        field.interactable = true;
        field.ActivateInputField();
    }
    public void OnChangeNickname()
    {
        InputField field = GameObject.Find("UI_Nickname_Input").GetComponent<InputField>();

        if (field.text == "")
            return;

        field.interactable = false;
        PhotonNetwork.NickName = field.text;
        GetComponent<MiniAlertController>().OnEnableAlert("닉네임이 변경되었습니다.", field.text);
    }

    // ---------------------------------------------------------------------------------------------------
    // 메인메뉴 - 방생성
    // ---------------------------------------------------------------------------------------------------

    public void OnMenuCreateExit() // 종료
    {
        GameObject.Find("UI_MainMenu_CreateRoom").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
    }
    public void OnMenuCreatePublic() // 공개 여부 토글
    {
        Text publicText = GameObject.Find("UI_MainMenu_CreateRoom_Public_PublicButton_Text").GetComponent<Text>();
        InputField passText = GameObject.Find("UI_MainMenu_CreateRoom_Public_InputField").GetComponent<InputField>();

        if (publicText.text == "공개")
        {
            publicText.text = "비공개";
            passText.text = "";
            passText.interactable = true;
        }
        else
        {
            publicText.text = "공개";
            passText.text = "";
            passText.interactable = false;

        }
    }
    public void OnMenuCreateSubmit() // 제출
    {
        GameObject.Find("UI_MainMenu_CreateRoom").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        fadeController.OnBlack();

        string title = GameObject.Find("UI_MainMenu_CreateRoom_RoomTitle_InputField").GetComponent<InputField>().text;
        bool isPrivate = GameObject.Find("UI_MainMenu_CreateRoom_Public_PublicButton_Text").GetComponent<Text>().text == "비공개";
        string password = GameObject.Find("UI_MainMenu_CreateRoom_Public_InputField").GetComponent<InputField>().text;
        int countOfPlayers = GameObject.Find("UI_MainMenu_CreateRoom_Max_Dropdown").GetComponent<Dropdown>().value + 3;
        int countOfAliens = GameObject.Find("UI_MainMenu_CreateRoom_Alien_Dropdown").GetComponent<Dropdown>().value + 1;

        ExitGames.Client.Photon.Hashtable prop = new ExitGames.Client.Photon.Hashtable();
        prop["title"] = title;
        prop["isPrivate"] = isPrivate;
        prop["password"] = password;
        prop["countOfAliens"] = countOfAliens;

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = (byte)countOfPlayers, CustomRoomProperties = prop, CustomRoomPropertiesForLobby = new string[] { "title", "isPrivate", "password", "countOfAliens" }, BroadcastPropsChangeToAll = true }) ;
    }

    public override void OnCreateRoomFailed(short returnCode, string message) // 방 생성에 실패했을 때 호출되는 콜백 함수
    {
        base.OnCreateRoomFailed(returnCode, message);

        alertController.OnEnableAlert("방 생성 실패", message + " (" + returnCode + ")");
    }

    // ---------------------------------------------------------------------------------------------------
    // 메인메뉴 - 방탐색
    // ---------------------------------------------------------------------------------------------------
    public void OnMenuExploreExit() // 종료
    {
        GameObject.Find("UI_ExploreRoom").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
    }

    public void OnMenuExplorePage(int num) // 페이지 이동
    {
        lobbyPage += num;

        if (lobbyPage < 1) lobbyPage = 1;
        if (lobbyPage > ((int)(lobbyRoom.Count / 8) + 1)) lobbyPage = ((int)(lobbyRoom.Count / 8) + 1);
        RefreshRoomList();
    }

    public void OnMenuExploreSubmit(int num) // 방 선택
    {
        if (lobbyRoom.Count < num * lobbyPage)
            return;

        RoomInfo info = lobbyRoom[num * lobbyPage];
        ExitGames.Client.Photon.Hashtable roomProp = info.CustomProperties;

        if ((bool)roomProp["isPrivate"] == false) {
            PhotonNetwork.JoinRoom(info.Name);
        }
        else
        {
            selectedRoom = info;
            GameObject.Find("UI_PasswordAlert_InputField").GetComponent<InputField>().text = "";
            GameObject.Find("UI_PasswordAlert").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }       
    }

    public void OnPasswordExit() // (패스워드창) 종료
    {
        selectedRoom = null;
        GameObject.Find("UI_PasswordAlert").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
    }

    public void OnPasswordSubmit() // (패스워드창) 입장
    {
        ExitGames.Client.Photon.Hashtable roomProp = selectedRoom.CustomProperties;

        if (GameObject.Find("UI_PasswordAlert_InputField").GetComponent<InputField>().text == (string)roomProp["password"])
        {
            GameObject.Find("UI_PasswordAlert_InputField").GetComponent<InputField>().text = "";
            GameObject.Find("UI_PasswordAlert").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            GameObject.Find("UI_ExploreRoom").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            PhotonNetwork.JoinRoom(selectedRoom.Name);
            selectedRoom = null;
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);

        lobbyRoom = roomList;
        RefreshRoomList();
    }

    public void RefreshRoomList() // 방 갱신
    {
        if (lobbyRoom == null) return;
        if (lobbyRoom.Count <= 0) return;

        int i = 8 * (lobbyPage - 1);
        int max = lobbyRoom.Count < 8 * (lobbyPage) ? lobbyRoom.Count : 8 * (lobbyPage);

        // 존재하는 방 처리
        for (; i< max; i++)
        {
            RoomInfo info = lobbyRoom[i];
            GameObject unit = GameObject.Find("UI_ExploreRoom_Unit_" + i % 8);
            ExitGames.Client.Photon.Hashtable roomProp = info.CustomProperties;

            if (info.IsOpen == true)
                unit.GetComponent<Button>().interactable = true;
            else
                unit.GetComponent<Button>().interactable = false;

            if (roomProp.Count <= 0)
            {
                unit.GetComponent<Button>().interactable = false;
                unit.transform.Find("UI_ExploreRoom_Unit_Title").GetComponent<Text>().text = "";
                unit.transform.Find("UI_ExploreRoom_Unit_Info").GetComponent<Text>().text = "";
                unit.transform.Find("UI_ExploreRoom_Unit_Status").GetComponent<Text>().text = "";
                continue;
            }


            unit.transform.Find("UI_ExploreRoom_Unit_Title").GetComponent<Text>().text = (string)roomProp["title"];

            string topText = "";
            topText += (bool)roomProp["isPrivate"] == false ? "공개, " : "비공개, ";
            topText += "외계인 " + roomProp["countOfAliens"] + "명";
            unit.transform.Find("UI_ExploreRoom_Unit_Info").GetComponent<Text>().text = topText;

            string bottomText = "";
            bottomText += info.IsOpen == true ? "대기중" : "게임중";
            bottomText += "\n";
            bottomText += "(" + info.PlayerCount + "/" + info.MaxPlayers + ")";
            unit.transform.Find("UI_ExploreRoom_Unit_Status").GetComponent<Text>().text = bottomText;
        }

        // 방이 페이지 내 8번까지 채우지 못하면 나머지 처리
        for (; i < 8 * (lobbyPage); i++)
        {
            GameObject unit = GameObject.Find("UI_ExploreRoom_Unit_" + i % 8);

            unit.GetComponent<Button>().interactable = false;
            unit.transform.Find("UI_ExploreRoom_Unit_Title").GetComponent<Text>().text = "";
            unit.transform.Find("UI_ExploreRoom_Unit_Info").GetComponent<Text>().text = "";
            unit.transform.Find("UI_ExploreRoom_Unit_Status").GetComponent<Text>().text = "";
        }

        // 페이지 표시
        GameObject.Find("UI_ExploreRoom_Page").GetComponent<Text>().text = lobbyPage + " / " + ((int)(lobbyRoom.Count / 8) + 1);

    }

    // ---------------------------------------------------------------------------------------------------
    // 대기실 - 공통
    // ---------------------------------------------------------------------------------------------------
    public override void OnJoinedRoom() // 방 입장에 성공했을때 호출되는 콜백 함수
    {
        base.OnJoinedRoom();

        ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;

        localProp["color"] = 0;
        localProp["isReady"] = false;
        localProp["isStart"] = false;

        PhotonNetwork.LocalPlayer.SetCustomProperties(localProp);
        OnJoinedRoomCall();
    }
    public void OnJoinedRoomCall()
    {
        fadeController.OnBlack();
        GameObject.Find("UI_Intro").GetComponent<Canvas>().enabled = false;
        GameObject.Find("UI_MainMenu").GetComponent<Canvas>().enabled = false;
        GameObject.Find("UI_Room").GetComponent<Canvas>().enabled = true;

        menuCode = MENU_ROOM;
        fadeController.OnFadeIn();

        GameObject UI_Room_Palette = GameObject.Find("UI_Room_Palette");
        // GameObject UI_Room_Palette_Prefab = Resources.Load<GameObject>("Resources/UI/UI_Room_Palette.prefab");
        UI_Room_Palettes = new GameObject[playerColors.colors.Capacity];
        for (int i = 0; i < playerColors.colors.Capacity; i++)
        {
            UI_Room_Palettes[i] = Instantiate(UI_Room_Palette_Prefab, UI_Room_Palette_Prefab.transform.position + new Vector3(i * 50.0f, 0, 0), UI_Room_Palette.transform.rotation);
            UI_Room_Palettes[i].transform.SetParent(UI_Room_Palette.transform, false); // 부모에 상대적인 위치로 맞춤

            UI_Room_Palettes[i].name = "UI_Room_Palette_" + i;
            UI_Room_Palettes[i].GetComponent<Image>().color = playerColors.colors[i]; // 미리 지정된 Color Palette의 색상 부여
            int temp = i;
            UI_Room_Palettes[i].GetComponent<Button>().onClick.AddListener(delegate { OnChangeColor(temp); });
        }

        RefreshRoomUI();

        GameObject.Find("UI_Room_Ready").GetComponent<Button>().interactable = true;
        GameObject.Find("UI_Room_Exit").GetComponent<Button>().interactable = true;

        GameObject.Find("UI_Room_Chat_Text").GetComponent<Text>().text = "";
        GameObject.Find("UI_Room_Chat_Input").GetComponent<InputField>().ActivateInputField();

        GameObject.Find("UI_MainMenu_Join").GetComponent<Button>().interactable = true;
    }

    public override void OnJoinRandomFailed(short returnCode, string message) // 방 입장에 실패했을 때 호출되는 콜백 함수
    {
        base.OnCreateRoomFailed(returnCode, message);

        GameObject.Find("UI_MainMenu_Join").GetComponent<Button>().interactable = true;
        alertController.OnEnableAlert("방 입장 실패", message + " (" + returnCode + ")");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        GameObject.Find("UI_MainMenu_Join").GetComponent<Button>().interactable = true;
        alertController.OnEnableAlert("방 입장 실패", message + " (" + returnCode + ")");
    }

    public void OnRoomReady() // 게임 준비(시작) 버튼의 클릭 함수
    {
        if (GameObject.Find("UI_Room_Ready").GetComponent<Button>().interactable == false)
            return;

        if (PhotonNetwork.LocalPlayer.IsMasterClient == true)
        {
            ExitGames.Client.Photon.Hashtable roomProp = PhotonNetwork.CurrentRoom.CustomProperties;
            Photon.Realtime.Player[] player = PhotonNetwork.PlayerList;

            if (DEBUG_LOBBY == false && player.Length <= (int)roomProp["countOfAliens"])
            {
                alertController.OnEnableAlert("인원 부족", "참가 인원이 외계인 수보다 많아야 합니다.\n현재 인원으로는 시작할 수 없습니다.");
                return;
            }

            for (int i = 0; i < player.Length; i++)
            {
                ExitGames.Client.Photon.Hashtable prop = player[i].CustomProperties;
                prop["spawnIndex"] = (i + 1);
                player[i].SetCustomProperties(prop);
            }

            PhotonNetwork.CurrentRoom.IsOpen = false; // 난입 제한

            photonView.RPC("OnStart", RpcTarget.AllBuffered);
        }
        else
        {
            ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;
            if (localProp.ContainsKey("isReady") == false) localProp.Add("isReady", true);
            else if ((bool)localProp["isReady"] == true) localProp["isReady"] = false;
            else if ((bool)localProp["isReady"] == false) localProp["isReady"] = true;

            if (localProp.ContainsKey("isAlien") == true) localProp.Remove("isAlien");
            PhotonNetwork.LocalPlayer.SetCustomProperties(localProp);

            RefreshRoomUI();
        }
    }

    [PunRPC]
    public void OnStart() // 게임 시작 함수
    {
        GameObject.Find("UI_Room_Exit").GetComponent<Button>().interactable = false;

        // 채팅창 비활성화 (퇴장시 커서 관련 오류 발생 방지)
        GameObject.Find("UI_Room_Chat_Input").GetComponent<InputField>().DeactivateInputField();

        SceneManager.LoadScene("proto_field_ver2");
    }

    public void OnRoomExit() // 나가기 버튼의 클릭 함수
    {
        if (GameObject.Find("UI_Room_Exit").GetComponent<Button>().interactable == false)
            return;

        // 채팅창 비활성화 (퇴장시 커서 관련 오류 발생 방지)
        GameObject.Find("UI_Room_Chat_Input").GetComponent<InputField>().DeactivateInputField();

        // 플레이어 프롭 제거
        ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;
        localProp.Clear();
        PhotonNetwork.LocalPlayer.SetCustomProperties(localProp);

        PhotonNetwork.LeaveRoom();
    }

    public void OnRoomOption() // 게임 설정 버튼
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        ExitGames.Client.Photon.Hashtable prop = PhotonNetwork.CurrentRoom.CustomProperties;

        GameObject.Find("UI_Room_UpdateRoom_RoomTitle_InputField").GetComponent<InputField>().text = (string)prop["title"];
        GameObject.Find("UI_Room_UpdateRoom_Public_PublicButton_Text").GetComponent<Text>().text = (bool)prop["isPrivate"] == true ? "비공개" : "공개";
        GameObject.Find("UI_Room_UpdateRoom_Public_InputField").GetComponent<InputField>().interactable = (bool)prop["isPrivate"] == true ? true : false;
        GameObject.Find("UI_Room_UpdateRoom_Public_InputField").GetComponent<InputField>().text = (string)prop["password"];
        GameObject.Find("UI_Room_UpdateRoom_Max_Dropdown").GetComponent<Dropdown>().value = (int)PhotonNetwork.CurrentRoom.MaxPlayers - 3;
        GameObject.Find("UI_Room_UpdateRoom_Alien_Dropdown").GetComponent<Dropdown>().value = (int)prop["countOfAliens"] - 1;
        GameObject.Find("UI_Room_UpdateRoom").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }

    public void OnRoomOptionExit() // 게임 설정 - 종료
    {
        GameObject.Find("UI_Room_UpdateRoom").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
    }

    public void OnRoomOptionPublic() // 게임 설정 - 공개 여부 토글
    {
        Text publicText = GameObject.Find("UI_Room_UpdateRoom_Public_PublicButton_Text").GetComponent<Text>();
        InputField passText = GameObject.Find("UI_Room_UpdateRoom_Public_InputField").GetComponent<InputField>();

        if (publicText.text == "공개")
        {
            publicText.text = "비공개";
            passText.text = "";
            passText.interactable = true;
        }
        else
        {
            publicText.text = "공개";
            passText.text = "";
            passText.interactable = false;

        }
    }

    public void OnRoomOptionSubmit() // 게임 설정 - 수정
    {
        ExitGames.Client.Photon.Hashtable prop = PhotonNetwork.CurrentRoom.CustomProperties;

        prop["title"] = GameObject.Find("UI_Room_UpdateRoom_RoomTitle_InputField").GetComponent<InputField>().text;
        prop["isPrivate"] = GameObject.Find("UI_Room_UpdateRoom_Public_PublicButton_Text").GetComponent<Text>().text == "비공개" ? true : false;
        prop["password"] = GameObject.Find("UI_Room_UpdateRoom_Public_InputField").GetComponent<InputField>().text;
        PhotonNetwork.CurrentRoom.MaxPlayers = (byte)(GameObject.Find("UI_Room_UpdateRoom_Max_Dropdown").GetComponent<Dropdown>().value + 3);
        prop["countOfAliens"] = (int)(GameObject.Find("UI_Room_UpdateRoom_Alien_Dropdown").GetComponent<Dropdown>().value) + 1;
        PhotonNetwork.CurrentRoom.SetCustomProperties(prop);

        GameObject.Find("UI_Room_UpdateRoom").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        RefreshRoomUI();
    }

    public void RefreshRoomUI() // 대기실 UI를 갱신하는 함수
    {
        int countOfReady = 0; // 준비한 플레이어 수

        Photon.Realtime.Player[] player = PhotonNetwork.PlayerList;

        // 접속된 플레이어 칸 갱신
        for (int i = 0; i < player.Length; i++)
        {
            ExitGames.Client.Photon.Hashtable prop = player[i].CustomProperties;
            GameObject playerPanel = GameObject.Find("UI_Room_Player_" + i);

            playerPanel.GetComponent<Image>().enabled = true;
            playerPanel.transform.Find("UI_Room_Player_Nickname").gameObject.GetComponent<Text>().text = player[i].NickName;

            if (prop.ContainsKey("color"))
                playerPanel.transform.Find("UI_Room_Player_Color").gameObject.GetComponent<Image>().color = playerColors.colors[(int)prop["color"]];
            // playerPanel.transform.Find("UI_Room_Player_Color").gameObject.GetComponent<Image>().color = GameObject.Find("UI_Room_Palette_" + (int)prop["color"]).GetComponent<Image>().color;

            if (prop.ContainsKey("isStart") && (bool)prop["isStart"] == true)
            {
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Animation>().Stop();
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Image>().fillAmount = 1;
                playerPanel.transform.Find("UI_Room_Player_Status").gameObject.GetComponent<Text>().text = "시작";
            }
            else if (prop.ContainsKey("isReady") && (bool)prop["isReady"] == true)
            {
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Animation>().Stop();
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Image>().fillAmount = 1;
                playerPanel.transform.Find("UI_Room_Player_Status").gameObject.GetComponent<Text>().text = "준비";
                countOfReady++;
            }
            else if (player[i].IsMasterClient == true)
            {
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Animation>().Stop();
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Image>().fillAmount = 0;
                playerPanel.transform.Find("UI_Room_Player_Status").gameObject.GetComponent<Text>().text = "방장";
                countOfReady++;
            }
            else
            {
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Animation>().Stop();
                playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Image>().fillAmount = 0;
                playerPanel.transform.Find("UI_Room_Player_Status").gameObject.GetComponent<Text>().text = "";
            }
        }

        // 접속되지 않은 플레이어 칸 갱신
        for (int i = player.Length; i < 10; i++)
        {
            GameObject playerPanel = GameObject.Find("UI_Room_Player_" + i);

            playerPanel.GetComponent<Image>().enabled = false;
            playerPanel.transform.Find("UI_Room_Player_Nickname").gameObject.GetComponent<Text>().text = "";
            playerPanel.transform.Find("UI_Room_Player_Highlight").gameObject.GetComponent<Image>().fillAmount = 0;
            playerPanel.transform.Find("UI_Room_Player_Status").gameObject.GetComponent<Text>().text = "";
            playerPanel.transform.Find("UI_Room_Player_Color").gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        }

        // 방 정보 갱신
        ExitGames.Client.Photon.Hashtable roomProp = PhotonNetwork.CurrentRoom.CustomProperties;

        GameObject.Find("UI_Room_Title_Text").GetComponent<Text>().text = (string)roomProp["title"];
        GameObject.Find("UI_Room_Title_Count").GetComponent<Text>().text = PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;

        string topText = "";
        topText += (bool)roomProp["isPrivate"] == false ? "공개, " : "비공개 (패스워드 : " + roomProp["password"] + "), ";
        topText += (roomProp.ContainsKey("countOfAliens") == true) ? ("외계인 " + roomProp["countOfAliens"] + "명") : "";
        GameObject.Find("UI_Room_TopPanel_Text").GetComponent<Text>().text = topText;

        // 준비, 시작 갱신
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

        // 준비, 시작에 따른 내 플레이어 칸 갱신
        ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;
        if (localProp.ContainsKey("isReady") && (bool)localProp["isReady"] == true) GameObject.Find("UI_Room_Ready_Highlight").GetComponent<Image>().fillAmount = 1;
        else GameObject.Find("UI_Room_Ready_Highlight").GetComponent<Image>().fillAmount = 0;

        // 마스터클라이언트만 옵션 보이게
        if (PhotonNetwork.IsMasterClient == true)
            GameObject.Find("UI_Room_Option").GetComponent<Button>().interactable = true;
        else
            GameObject.Find("UI_Room_Option").GetComponent<Button>().interactable = false;
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
