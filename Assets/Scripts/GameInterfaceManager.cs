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
    public GameObject playerObject;

    public GameObject statHpObject;
    public GameObject statO2Object;

    public GameObject meterialWoodObject;
    public GameObject meterialIronObject;
    public GameObject meterialPartObject;

    public GameObject talkPanelObject;
    public GameObject talkInputFieldObject;
    public GameObject talkTextObject;

    public GameObject timerObject;
    public GameObject timerTextObject;

    public GameObject SystemMsgBar;
    public float time;

    bool flag_chat = false; // 채팅모드 체크용 플래그. 채팅이 켜져있다면 true로 변경됨

    void Start()
    {
        
    }

    void Update()
    {
        Player player = playerObject.GetComponent<Player>();

        statHpObject.GetComponent<Image>().fillAmount = (float)player.statHp / (float)player.statHpMax;
        statO2Object.GetComponent<Image>().fillAmount = (float)player.statO2 / (float)player.statO2Max;

        meterialWoodObject.GetComponent<Text>().text = player.meterialWood.ToString();
        meterialIronObject.GetComponent<Text>().text = player.meterialIron.ToString();
        meterialPartObject.GetComponent<Text>().text = player.meterialPart.ToString();

        timerObject.GetComponent<Image>().fillAmount = time / 1800.0f;
        timerTextObject.GetComponent<Text>().text = Math.Truncate(time / 60.0f).ToString() + " : " + Math.Truncate(time % 60.0f);

        // 채팅모드 전환 (탭)
        if (Input.GetKeyDown(KeyCode.Tab) == true)
        {
            OnSwitchChat();
        }
        // 채팅 (엔터)
        else if (flag_chat == true && (Input.GetKeyDown(KeyCode.Return) == true || Input.GetKeyDown(KeyCode.KeypadEnter) == true))
        {
            OnSendChat();
        }
    }

    // ---------------------------------------------------------------------------------------------------
    // 채팅 관련
    // ---------------------------------------------------------------------------------------------------
    public void OnSwitchChat() // 채팅 화면을 켜거나 끄는 스위치 함수
    {
        if (flag_chat == false)
        {
            flag_chat = true;
            talkPanelObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

            InputField field = talkInputFieldObject.GetComponent<InputField>();
            field.ActivateInputField();
        }
        else
        {
            flag_chat = false;
            talkPanelObject.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        }
    }

    public void OnSendChat() // 입력한 채팅을 송신하는 함수
    {
        InputField field = talkInputFieldObject.GetComponent<InputField>();

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
        Text chat = talkTextObject.GetComponent<Text>();
        chat.text = chat.text + "\n" + message;
    }

}
