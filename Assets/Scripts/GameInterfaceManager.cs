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
    public GameObject managerObject; // 게임 매니저 객체 (미지정시 미동작)
    public GameObject playerObject;   // 플레이어 객체 (런타임 중 자동 할당)

    bool mode_result = false; // 결과창 모드
    bool mode_chat = false; // 채팅 모드 (채팅이 켜져있다면 true)
    bool mode_watching = false; // 관전 모드 (관전창이 켜져있다면 true)
    int mode_watching_index = 0; // 관전 모드 캐릭터 인덱스

    float fps = 0.0f; // fps 체크

    void Update()
    {
        if (managerObject == null)
            return;

        // fps 체크
        fps += (Time.deltaTime - fps) * 0.1f;

        // 게임이 끝나면 결과창 출력
        if (GameManager.GetInstance().flag_finish == true && mode_result == false)
            OnSwitchResult();

        // 게임이 끝나면 다른 갱신은 중단
        if (mode_result == true)
            return;

        // UI 갱신
        refresh();

        // 관전모드 전환
        if (playerObject == null && mode_watching == false)
        {
            OnSwitchWatching();
        }
        else if (playerObject != null && mode_watching == true)
        {
            OnSwitchWatching();
        }

        if (playerObject == null)
        {
            playerObject = managerObject.GetComponent<GameManager>().mPlayer;
            return;
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

        // 나가기 (F4) ** 디버깅용 **
        if (Input.GetKeyDown(KeyCode.F4) == true)
        {
            ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;
            localProp.Clear();
            PhotonNetwork.LocalPlayer.SetCustomProperties(localProp);

            PhotonNetwork.LeaveRoom();
        }
    }

    void refresh() // UI 갱신
    {
        GameObject.Find("UI_Timer_Bar").gameObject.GetComponent<Image>().fillAmount = managerObject.GetComponent<GameManager>().time / managerObject.GetComponent<GameManager>().timeMax;
        GameObject.Find("UI_Timer_Text").gameObject.GetComponent<Text>().text = Math.Truncate(managerObject.GetComponent<GameManager>().time / 60.0f).ToString() + ":" + Math.Truncate(managerObject.GetComponent<GameManager>().time % 60.0f);

        GameObject.Find("FPS").GetComponent<Text>().text = (int)(1.0f / fps) + " FPS";
        GameObject.Find("Ping").GetComponent<Text>().text = PhotonNetwork.GetPing().ToString() + " ms";

        if (playerObject == null)
            return;

        Player player = playerObject.GetComponent<Player>();

        GameObject.Find("UI_Stat_HP_Bar").gameObject.GetComponent<Image>().fillAmount = (float)player.GetHP() / (float)player.GetHPMax();
        GameObject.Find("UI_Stat_O2_Bar").gameObject.GetComponent<Image>().fillAmount = (float)player.GetO2() / (float)player.GetO2Max();

        GameObject.Find("UI_Meterial_Wood_Text").GetComponent<Text>().text = player.GetWood().ToString();
        GameObject.Find("UI_Meterial_Iron_Text").gameObject.GetComponent<Text>().text = player.GetIron().ToString();
        GameObject.Find("UI_Meterial_Part_Text").gameObject.GetComponent<Text>().text = player.GetPart().ToString();
    }

    // ---------------------------------------------------------------------------------------------------
    // 채팅 모드
    // ---------------------------------------------------------------------------------------------------
    public void OnSwitchChat() // 채팅 모드 스위칭 함수
    {
        if (mode_chat == false) // 채팅 모드 ON
        {
            mode_chat = true;
            playerObject.GetComponent<Player>().SetMove(false);
            GameObject.Find("UI_Panel_Talk").gameObject.GetComponent<Animator>().Play("Talk_load");

            InputField field = GameObject.Find("UI_Panel_Talk_Input").gameObject.GetComponent<InputField>();
            field.DeactivateInputField();
        }
        else // 채팅 모드 OFF
        {
            Animator animator = GameObject.Find("UI_Panel_Talk").gameObject.GetComponent<Animator>();
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Talk_show") == false)
                return;

            mode_chat = false;
            playerObject.GetComponent<Player>().SetMove(true);
            GameObject.Find("UI_Talk_Active").gameObject.GetComponent<Image>().enabled = false;
            GameObject.Find("UI_Panel_Talk").gameObject.GetComponent<Animator>().Play("Talk_hide");

            InputField field = GameObject.Find("UI_Panel_Talk_Input").gameObject.GetComponent<InputField>();
            field.DeactivateInputField();
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

            OnMoveWatching(0);

            GameObject.Find("UI_Stats").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            GameObject.Find("UI_Inventory").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            GameObject.Find("UI_Watching").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        }
        else // 관전 모드 OFF
        {
            mode_watching = false;

            GameObject.Find("UI_Stats").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            GameObject.Find("UI_Inventory").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            GameObject.Find("UI_Watching").GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        }
    }

    public void OnMoveWatching(int num) // 관전 대상 변경 함수
    {
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");

        if (player.Length <= 0)
        {
            GameObject.Find("UI_Watching_Nickname").GetComponent<Text>().text = "";
            return;
        }
            
        mode_watching_index += num;
        if (mode_watching_index >= player.Length) mode_watching_index = 0;
        if (mode_watching_index < 0) mode_watching_index = player.Length - 1;

        GameManager.GetInstance().mCamera.GetComponent<CinemachineFreeLook>().Follow = player[mode_watching_index].transform;
        GameManager.GetInstance().mCamera.GetComponent<CinemachineFreeLook>().LookAt = player[mode_watching_index].transform;
        GameObject.Find("UI_Watching_Nickname").GetComponent<Text>().text = player[mode_watching_index].GetComponent<PhotonView>().Owner.NickName;
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
        GameObject.Find("UI_Result").GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
    }
}