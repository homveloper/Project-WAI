using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using static System.Random;
using UnityEngine.SceneManagement;
using Cinemachine;
using System;

public class GameInterfaceManager : MonoBehaviourPunCallbacks
{
    bool mode_result = false; // 결과창
    bool mode_chat = false; // 채팅창
    bool mode_watching = false; // 관전창

    int mode_watching_index = 0; // 관전 모드 캐릭터 인덱스

    float fps = 0.0f; // fps 체크

    void Update()
    {
        if (GameManager.GetInstance() == null)
            return;

        // fps 체크
        fps += (Time.deltaTime - fps) * 0.1f;

        // 게임이 끝나면 결과창 출력
        if (GameManager.GetInstance().flag_finish == true && mode_result == false)
            OnSwitchResult();

        // 게임 매니저가 없는 상황, 플레이어 데이터가 없는 상황, 게임이 끝난 상황에는 갱신하지 않음
        if (GameManager.GetInstance().mPlayer == null || mode_result == true)
            return;

        // UI 갱신
        refresh();

        // 관전모드 전환
        if (GameManager.GetInstance().mPlayer.GetComponent<Player>().IsDead() == true && mode_watching == false)
        {
            OnSwitchWatching();
        }
        else if (GameManager.GetInstance().mPlayer.GetComponent<Player>().IsDead() == false && mode_watching == true)
        {
            OnSwitchWatching();
        }

        // 관전모드 - 관전 대상 전환
        if (mode_watching == true && Input.GetKeyDown(KeyCode.LeftArrow) == true)
        {
            OnMoveWatching(-1);
        }
        else if (mode_watching == true && Input.GetKeyDown(KeyCode.RightArrow) == true)
        {
            OnMoveWatching(1);
        }

        // 나가기 (F4) ** 디버깅용 **
        if (Input.GetKeyDown(KeyCode.F4) == true)
        {
            ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;
            localProp.Clear();
            PhotonNetwork.LocalPlayer.SetCustomProperties(localProp);

            PhotonNetwork.LeaveRoom();
        }

        // 캐릭터가 죽었다면 채팅 모드는 동작하지 않음
        if (GameManager.GetInstance().mPlayer.GetComponent<Player>().IsDead() == true)
            return;

        // 채팅모드 전환 (탭)
        if (Input.GetKeyDown(KeyCode.Tab) == true)
        {
            OnSwitchChat();
        }

        // 채팅모드 - 채팅 (엔터)
        if (mode_chat == true && (Input.GetKeyDown(KeyCode.Return) == true || Input.GetKeyDown(KeyCode.KeypadEnter) == true))
        {
            // 화면이 완전 표시된 후에 채팅 사용 가능
            Animator animator = GameObject.Find("UI_Panel_Talk").gameObject.GetComponent<Animator>();
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Talk_show") == false)
                return;

            OnSendChat();
        }
    }

    void refresh() // UI 갱신
    {
        GameObject.Find("UI_Timer_Bar").gameObject.GetComponent<Image>().fillAmount = GameManager.GetInstance().GetComponent<GameManager>().time / GameManager.GetInstance().GetComponent<GameManager>().timeMax;
        GameObject.Find("UI_Timer_Text").gameObject.GetComponent<Text>().text = Math.Truncate(GameManager.GetInstance().GetComponent<GameManager>().time / 60.0f).ToString() + ":" + Math.Truncate(GameManager.GetInstance().GetComponent<GameManager>().time % 60.0f);

        GameObject.Find("FPS").GetComponent<Text>().text = (int)(1.0f / fps) + " FPS";
        GameObject.Find("Ping").GetComponent<Text>().text = PhotonNetwork.GetPing().ToString() + " ms";

        if (GameManager.GetInstance().mPlayer == null)
            return;

        Player player = GameManager.GetInstance().mPlayer.GetComponent<Player>();

        GameObject.Find("UI_Stat_HP_Bar").gameObject.GetComponent<Image>().fillAmount = (float)player.GetHP() / (float)player.GetHPMax();
        GameObject.Find("UI_Stat_O2_Bar").gameObject.GetComponent<Image>().fillAmount = (float)player.GetO2() / (float)player.GetO2Max();
        GameObject.Find("UI_Stat_Bt_Bar").gameObject.GetComponent<Image>().fillAmount = (float)player.GetBt() / (float)player.GetBtMax();

        GameObject.Find("UI_Meterial_Wood_Text").GetComponent<Text>().text = player.GetWood().ToString();
        GameObject.Find("UI_Meterial_Iron_Text").gameObject.GetComponent<Text>().text = player.GetIron().ToString();
        GameObject.Find("UI_Meterial_Part_Text").gameObject.GetComponent<Text>().text = player.GetPart().ToString();

        GameObject[] playerObj = GameObject.FindGameObjectsWithTag("Player");
        GameObject[] nickObj = GameObject.FindGameObjectsWithTag("Nickname");

        for (int i = 0; i < nickObj.Length; i++)
        {
            nickObj[i].GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        }

        for (int i = 0; i < playerObj.Length; i++)
        {
            if (playerObj[i].GetComponent<PhotonView>().IsMine == true)
                continue;

            Vector3 pos = playerObj[i].transform.position;
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(pos);

            //Debug.Log(i + "번째: " + viewportPoint.x + "/" + viewportPoint.y);

            viewportPoint.x *= Screen.width;
            viewportPoint.y = (viewportPoint.y * Screen.height) + 125;

            nickObj[i].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            nickObj[i].GetComponent<RectTransform>().position = viewportPoint;
            nickObj[i].GetComponent<Text>().text = playerObj[i].GetComponent<PhotonView>().Owner.NickName;

            ExitGames.Client.Photon.Hashtable myProp = PhotonNetwork.LocalPlayer.CustomProperties;
            ExitGames.Client.Photon.Hashtable playerProp = playerObj[i].GetComponent<PhotonView>().Owner.CustomProperties;

            if (myProp.ContainsKey("isAlien") == true && playerProp.ContainsKey("isAlien") == true && (bool)myProp["isAlien"] == true && (bool)playerProp["isAlien"] == true)
                nickObj[i].GetComponent<Outline>().effectColor = new Color(1, 0, 0);
            else
                nickObj[i].GetComponent<Outline>().effectColor = new Color(0, 0, 0);
        }   
    }

    // ---------------------------------------------------------------------------------------------------
    // 채팅 모드
    // ---------------------------------------------------------------------------------------------------
    public void OnSwitchChat() // 채팅 모드 스위칭 함수
    {
        if (mode_chat == false) // 채팅 모드 ON
        {
            mode_chat = true;
            GameManager.GetInstance().mPlayer.GetComponent<Player>().SetMove(false);
            GameObject.Find("UI_Panel_Talk").gameObject.GetComponent<Animator>().Play("Talk_load");

            InputField field = GameObject.Find("UI_Panel_Talk_Input").gameObject.GetComponent<InputField>();
            field.DeactivateInputField();
            GameManager.GetInstance().GetComponent<MissionController>().OnHide();
        }
        else // 채팅 모드 OFF
        {
            Animator animator = GameObject.Find("UI_Panel_Talk").gameObject.GetComponent<Animator>();
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Talk_show") == false)
                return;

            mode_chat = false;
            GameManager.GetInstance().mPlayer.GetComponent<Player>().SetMove(true);
            GameObject.Find("UI_Talk_Active").gameObject.GetComponent<Image>().enabled = false;
            GameObject.Find("UI_Panel_Talk").gameObject.GetComponent<Animator>().Play("Talk_hide");

            InputField field = GameObject.Find("UI_Panel_Talk_Input").gameObject.GetComponent<InputField>();
            field.DeactivateInputField();
            GameManager.GetInstance().GetComponent<MissionController>().OnShow();
        }
    }

    public void OnSendChat() // 입력한 채팅을 송신하는 함수
    {
        InputField field = GameObject.Find("UI_Panel_Talk_Input").gameObject.GetComponent<InputField>();

        field.ActivateInputField();

        if (field.text != "")
        {
            photonView.RPC("OnChat", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.NickName + " : " + field.text);
            field.text = "";
            field.ActivateInputField();
        }
    }

    [PunRPC]
    public void OnChat(string message) // RPC로 채팅을 수신하는 함수
    {
        GameObject.Find("UI_Talk_Active").gameObject.GetComponent<Image>().enabled = true;
        Text chat = GameObject.Find("UI_Panel_Talk_Panel_Text").gameObject.GetComponent<Text>();
        chat.text = chat.text + "\n" + message;
    }

    // ---------------------------------------------------------------------------------------------------
    // 관전 모드
    // ---------------------------------------------------------------------------------------------------
    public void OnSwitchWatching() // 관전 모드 스위칭 함수
    {
        if (mode_watching == false) // 관전모드 ON
        {
            mode_watching = true;
            
            if (mode_chat == true) OnSwitchChat(); // 채팅 창이 켜져있으면 끄기

            OnMoveWatching(0);

            GameObject.Find("UI_Stats").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            GameObject.Find("UI_Inventory").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            GameObject.Find("UI_Watching").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            GameManager.GetInstance().GetComponent<MissionController>().OnHide();
            GameManager.GetInstance().mPlayer.GetComponent<Player>().SetMove(false);
        }
        else // 관전 모드 OFF
        {
            mode_watching = false;

            GameObject.Find("UI_Stats").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            GameObject.Find("UI_Inventory").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            GameObject.Find("UI_Watching").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            GameManager.GetInstance().GetComponent<MissionController>().OnShow();
            GameManager.GetInstance().mPlayer.GetComponent<Player>().SetMove(true);
        }
    }

    public void OnMoveWatching(int num) // 관전 대상 변경 함수
    {
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        int cnt = 1;
        
        while (cnt >= 1 && cnt <= 20)
        {
            mode_watching_index += num;
            if (mode_watching_index >= player.Length) mode_watching_index = 0;
            if (mode_watching_index < 0) mode_watching_index = player.Length - 1;

            if (player[mode_watching_index].GetComponent<Player>().IsDead() == false)
                cnt = -1;
            else
                cnt++;
        }

        if (cnt > 20)
        {
            GameObject.Find("UI_Watching").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            GameObject.Find("UI_Watching_Nickname").GetComponent<Text>().text = "";
        }
        else if (cnt < 0)
        {
            GameObject.Find("UI_Watching").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            GameManager.GetInstance().mCamera.GetComponent<CinemachineFreeLook>().Follow = player[mode_watching_index].transform;
            GameManager.GetInstance().mCamera.GetComponent<CinemachineFreeLook>().LookAt = player[mode_watching_index].transform;
            GameObject.Find("UI_Watching_Nickname").GetComponent<Text>().text = player[mode_watching_index].GetComponent<PhotonView>().Owner.NickName;
        }
    }

    // ---------------------------------------------------------------------------------------------------
    // 관전 모드
    // ---------------------------------------------------------------------------------------------------
    public override void OnLeftRoom() // 방에서 나갔을때 호출되는 콜백 함수
    {
        base.OnLeftRoom();

        SceneManager.LoadScene("proto_main");
    }

    // ---------------------------------------------------------------------------------------------------
    // 게임 종료 모드
    // ---------------------------------------------------------------------------------------------------
    public void OnSwitchResult() // 결과창 출력 (켜는 것만 있음)
    {
        mode_result = true;
        GameObject.Find("UI_Game").GetComponent<Canvas>().enabled = false;
        GameObject.Find("UI_Result").GetComponent<Canvas>().enabled = true;
    }
}