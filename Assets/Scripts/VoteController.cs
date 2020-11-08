using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.UI;

public class VoteController : MonoBehaviourPunCallbacks
{
    public GameObject votePanel;
    private GameObject[] color;
    private GameObject[] nickname;
    private GameObject[] up;
    private GameObject[] up_value;
    private GameObject[] down;
    private GameObject[] down_value;

    private GameObject[] player;

    void Start()
    {
        color = new GameObject[10];
        nickname = new GameObject[10];
        up = new GameObject[10];
        up_value = new GameObject[10];
        down = new GameObject[10];
        down_value = new GameObject[10];

        GameObject panel = votePanel.transform.Find("UI_Vote_Panel").gameObject;
        for (int i = 0; i <= 9; i++)
        {
            GameObject unit = panel.transform.Find("UI_Vote_Unit_" + i).gameObject;
            color[i] = unit.transform.Find("UI_Vote_Unit_Color").gameObject;
            nickname[i] = unit.transform.Find("UI_Vote_Unit_Nickname").gameObject;
            up[i] = unit.transform.Find("UI_Vote_Unit_Up").gameObject;
            up_value[i] = up[i].transform.Find("UI_Vote_Unit_Up_Value").gameObject;
            down[i] = unit.transform.Find("UI_Vote_Unit_Down").gameObject;
            down_value[i] = down[i].transform.Find("UI_Vote_Unit_Down_Value").gameObject;
        }

        votePanel.SetActive(false);
    }
    void Update()
    {
        // 5분 주기로 진행되며
        // 우주선에서 투표가 가능하며
        // 가장 연구원같은 사람과 외계인같은 사람을 투표하며
        // 5분마다 해당 결과를 취합해서 결과를 줘야하고
        // 공개 투표 (내가 누굴 찍고 남이 누굴 찍었는지 확인 가능) << 현황을 보여주는걸로 >>
        // 1분전에 알람
        // 마감되고 알람

    }
    public void SetSwitchVote() // 투표 창 출력 메소드
    {
        SetSwitchVote(!votePanel.activeSelf);
    }
    public void SetSwitchVote(bool val)
    {
        votePanel.SetActive(val);

        if (val == false)
            return;

        player = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i <=9; i++)
        {
            color[i].GetComponent<Image>().color = new Color(0, 0, 0, 0);
            nickname[i].GetComponent<Text>().text = "";
            up[i].GetComponent<Button>().interactable = false;
            up_value[i].GetComponent<Text>().text = "-";
            down[i].GetComponent<Button>().interactable = false;
            down_value[i].GetComponent<Text>().text = "-";

            if (i >= player.Length)
                continue;

            color[i].GetComponent<Image>().color = player[i].GetComponent<Player>().GetColor();
            nickname[i].GetComponent<Text>().text = player[i].GetComponent<PhotonView>().Owner.NickName;
            up[i].GetComponent<Button>().interactable = true;
            up_value[i].GetComponent<Text>().text = "0";
            down[i].GetComponent<Button>().interactable = true;
            down_value[i].GetComponent<Text>().text = "0";
        }
    }
    public void SetVoteUp(int num) // 투표 (신뢰)
    {
        photonView.RPC("OnVote", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, player[num].GetComponent<PhotonView>().Owner.ActorNumber, true);
    }
    public void SetVoteDown(int num) // 투표 (의심)
    {
        photonView.RPC("OnVote", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, player[num].GetComponent<PhotonView>().Owner.ActorNumber, false);
    }
    public void SetSendResult() // 결과 출력 메소드
    {
        // 결과 발표하고, 자원을 이동하는 내용
    }
    // ---------------------------------------------------------------------------------------------------
    // # 트리거 메소드
    // ---------------------------------------------------------------------------------------------------
    [PunRPC]
    void OnVote(int sender, int target, bool up) // d
    {
        GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert("테스트 투표 (" + sender + " > " + target + ")", up ? "신뢰함" : "의심함");
    }
    // ---------------------------------------------------------------------------------------------------
    // # 트리거 메소드
    // ---------------------------------------------------------------------------------------------------
    void OnTriggerStay(Collider other) // 우주선 근처에서 상호작용
    {
        if (!other.CompareTag("Player") || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            SetSwitchVote();
    }
}