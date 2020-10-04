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

    bool flag_chat = false; // 채팅모드 체크용 플래그. 채팅이 켜져있다면 true로 변경됨
    float fps = 0.0f;       // fps 체크

    void Update()
    {
        if (managerObject == null)
            return;

        if (playerObject == null)
            playerObject = managerObject.GetComponent<GameManager>().mPlayer;

        // 채팅모드 전환 (탭)
        if (Input.GetKeyDown(KeyCode.Tab) == true)
        {
            OnSwitchChat();
        }
        // 채팅 (엔터)
        else if (flag_chat == true && (Input.GetKeyDown(KeyCode.Return) == true || Input.GetKeyDown(KeyCode.KeypadEnter) == true))
        {
            // 화면이 완전 표시된 후에 채팅 사용 가능
            Animator animator = GameObject.Find("UI_Panel_Talk").gameObject.GetComponent<Animator>();
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Talk_show") == false)
                return;

            OnSendChat();
        }

        fps += (Time.deltaTime - fps) * 0.1f;
        refresh();
    }

    void refresh()
    {
        Player player = playerObject.GetComponent<Player>();

        GameObject.Find("UI_Stat_HP_Bar").gameObject.GetComponent<Image>().fillAmount = (float)player.GetHP() / (float)player.GetHPMax();
        GameObject.Find("UI_Stat_O2_Bar").gameObject.GetComponent<Image>().fillAmount = (float)player.GetO2() / (float)player.GetO2Max();

        GameObject.Find("UI_Meterial_Wood_Text").GetComponent<Text>().text = player.GetWood().ToString();
        GameObject.Find("UI_Meterial_Iron_Text").gameObject.GetComponent<Text>().text = player.GetIron().ToString();
        GameObject.Find("UI_Meterial_Part_Text").gameObject.GetComponent<Text>().text = player.GetPart().ToString();

        GameObject.Find("UI_Timer_Bar").gameObject.GetComponent<Image>().fillAmount = managerObject.GetComponent<GameManager>().time / managerObject.GetComponent<GameManager>().timeMax;
        GameObject.Find("UI_Timer_Text").gameObject.GetComponent<Text>().text = Math.Truncate(managerObject.GetComponent<GameManager>().time / 60.0f).ToString() + ":" + Math.Truncate(managerObject.GetComponent<GameManager>().time % 60.0f);

        GameObject.Find("FPS").GetComponent<Text>().text = (int)(1.0f / fps) + " FPS";
        GameObject.Find("Ping").GetComponent<Text>().text = PhotonNetwork.GetPing().ToString() + " ms";
    }

    // ---------------------------------------------------------------------------------------------------
    // 채팅 관련
    // ---------------------------------------------------------------------------------------------------
    public void OnSwitchChat() // 채팅 화면을 켜거나 끄는 스위치 함수
    {
        if (flag_chat == false)
        {
            flag_chat = true;
            playerObject.GetComponent<Player>().SetMove(false);
            GameObject.Find("UI_Panel_Talk").gameObject.GetComponent<Animator>().Play("Talk_load");

            InputField field = GameObject.Find("UI_Panel_Talk_Input").gameObject.GetComponent<InputField>();
            field.DeactivateInputField();
        }
        else
        {
            Animator animator = GameObject.Find("UI_Panel_Talk").gameObject.GetComponent<Animator>();
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Talk_show") == false)
                return;

            flag_chat = false;
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

}