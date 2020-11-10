using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.UI;
using System.Linq;
using System;

public class VoteController : MonoBehaviourPunCallbacks
{
    public GameObject votePanel;
    private GameObject[] color;
    private GameObject[] nickname;
    private GameObject[] up;
    private GameObject[] up_value;
    private GameObject[] down;
    private GameObject[] down_value;
    private GameObject time;
    private GameObject text;

    private GameObject[] player;
    private Dictionary<int, string> nick;
    private KeyValuePair<int, int> maxUpPlayer;
    private KeyValuePair<int, int> maxDownPlayer;
    private Dictionary<int, int> upVote;
    private Dictionary<int, int> downVote;

    
    bool voting; 

    void Start()
    {
        color = new GameObject[10];
        nickname = new GameObject[10];
        up = new GameObject[10];
        up_value = new GameObject[10];
        down = new GameObject[10];
        down_value = new GameObject[10];

        nick = new Dictionary<int, string>();
        upVote = new Dictionary<int, int>();
        downVote = new Dictionary<int, int>();

        GameObject panel = votePanel.transform.Find("UI_Vote_Panel").gameObject;
        time = panel.transform.Find("UI_Vote_Time").gameObject;
        text = panel.transform.Find("UI_Vote_Text").gameObject;
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

        SetStatus(true);
        votePanel.SetActive(false);
    }
    void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        float t = GameManager.GetInstance().time;

        if (voting)
            time.GetComponent<Text>().text = "투표 종료까지 " + Math.Truncate(t % 60) + "초 남음";
        else
            time.GetComponent<Text>().text = "다음 투표까지 " + Math.Truncate(t % 60) + "초 남음";

        if (t % 120 >= 59 && t % 120 < 60 && voting == true)
            SetStatus(false);
        else if (t % 120 >= 119 && t % 120 < 120 && voting == false)
            SetStatus(true);

        // 5분 주기로 진행되며
        // 우주선에서 투표가 가능하며
        // 가장 연구원같은 사람과 외계인같은 사람을 투표하며
        // 5분마다 해당 결과를 취합해서 결과를 줘야하고
        // 공개 투표 (내가 누굴 찍고 남이 누굴 찍었는지 확인 가능) << 현황을 보여주는걸로 >>
        // 1분전에 알람
        // 마감되고 알람

    }
    public void SetSwitchVote() // 투표 창 출력 (스위칭)
    {
        SetSwitchVote(!votePanel.activeSelf);
    }
    public void SetSwitchVote(bool val) // 투표 창 출력 (매뉴얼)
    {
        GameManager.GetInstance().mPlayer.GetComponent<Player>().SetMove(!val);
        votePanel.SetActive(val);

        if (val == false)
            return;

        player = GameObject.FindGameObjectsWithTag("Player");
        Refresh();
    }
    public void Clear() // 투표 초기화
    {
        upVote.Clear();
        downVote.Clear();

        maxUpPlayer = new KeyValuePair<int, int>(-1, 0);
        maxDownPlayer = new KeyValuePair<int, int>(-1, 0);

        upVote.Add(-1, -1);
        downVote.Add(-1, -1);
    }
    public void SetStatus(bool val) // 투표 상태 설정
    {
        voting = val;
        
        if (val == true)
            Clear();

        Refresh();

        if (val == false)
        {
            GameObject maxUpPlayerObject = null;
            GameObject maxDownPlayerObject = null;

            for (int i = 0; i<player.Length; i++)
            {
                if (player[i].GetComponent<PhotonView>().Owner.ActorNumber == maxUpPlayer.Key) maxUpPlayerObject = player[i];
                if (player[i].GetComponent<PhotonView>().Owner.ActorNumber == maxDownPlayer.Key) maxDownPlayerObject = player[i];
            }

            GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert("투표 결과", "최고 신뢰자: " + (maxUpPlayerObject != null ? maxUpPlayerObject.GetComponent<PhotonView>().Owner.NickName : "없음") + " (" + maxUpPlayer.Value + "표)\n" + "최고 의심자: " + (maxDownPlayerObject != null ? maxDownPlayerObject.GetComponent<PhotonView>().Owner.NickName : "없음") + " (" + maxDownPlayer.Value + "표)");

            if (maxUpPlayerObject == null || maxDownPlayerObject == null)
                return;

            int wood = maxDownPlayerObject.GetComponent<Player>().GetWood();
            int iron = maxDownPlayerObject.GetComponent<Player>().GetIron();
            int part = maxDownPlayerObject.GetComponent<Player>().GetPart();

            maxDownPlayerObject.GetComponent<Player>().SetTransformMeterial(-wood, -iron, -part);
            maxUpPlayerObject.GetComponent<Player>().SetTransformMeterial(wood, iron, part);
        }
    }
    public void Refresh() // 투표창 갱신
    {

        Dictionary<int, int> upCount = upVote.GroupBy(r => r.Value).ToDictionary(grp => grp.Key, grp => grp.Count());
        Dictionary<int, int> downCount = downVote.GroupBy(r => r.Value).ToDictionary(grp => grp.Key, grp => grp.Count());

        for (int i = 0; i <= 9; i++)
        {
            color[i].GetComponent<Image>().color = new Color(0, 0, 0, 0);
            nickname[i].GetComponent<Text>().text = "";
            up[i].GetComponent<Button>().interactable = false;
            up_value[i].GetComponent<Text>().text = "-";
            down[i].GetComponent<Button>().interactable = false;
            down_value[i].GetComponent<Text>().text = "-";

            if (player == null || i >= player.Length)
                continue;

            int actorNumber = player[i].GetComponent<PhotonView>().Owner.ActorNumber;

            if (nick.ContainsKey(actorNumber))
                nick[actorNumber] = player[i].GetComponent<Player>().GetNickname();
            else
                nick.Add(actorNumber, player[i].GetComponent<Player>().GetNickname());

            if ((upCount.ContainsKey(actorNumber) ? upCount[actorNumber] : 0) > maxUpPlayer.Value)
                maxUpPlayer = new KeyValuePair<int, int>(actorNumber, upCount[actorNumber]);
            if ((downCount.ContainsKey(actorNumber) ? downCount[actorNumber] : 0) > maxDownPlayer.Value)
                maxDownPlayer = new KeyValuePair<int, int>(actorNumber, downCount[actorNumber]);


            color[i].GetComponent<Image>().color = player[i].GetComponent<Player>().GetColor();
            nickname[i].GetComponent<Text>().text = player[i].GetComponent<Player>().GetNickname();
            up[i].GetComponent<Button>().interactable = voting;
            up_value[i].GetComponent<Text>().text = (upCount.ContainsKey(actorNumber) ? upCount[actorNumber] : 0).ToString();
            down[i].GetComponent<Button>().interactable = voting;
            down_value[i].GetComponent<Text>().text = (downCount.ContainsKey(actorNumber) ? downCount[actorNumber] : 0).ToString();

            if (voting)
                text.GetComponent<Text>().text = "투표가 진행중입니다.";
            else
                text.GetComponent<Text>().text = "투표가 종료되었습니다.\n가장 의심받은 사람의 모든 재료가 가장 신뢰받은 사람에게로 넘어갑니다.";
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
    // ---------------------------------------------------------------------------------------------------
    // # 트리거 메소드
    // ---------------------------------------------------------------------------------------------------
    [PunRPC]
    void OnVote(int sender, int target, bool up) // 투표 수신
    {
        try
        {
            if (up)
                if (upVote.ContainsKey(sender)) upVote[sender] = target;
                else upVote.Add(sender, target);
            else
                if (downVote.ContainsKey(sender)) downVote[sender] = target;
                else downVote.Add(sender, target);

            if (nick.ContainsKey(target))
                GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert("투표 진행중", nick[target] + " (이)가 " + (up ? "신뢰" : "의심") + "받고 있습니다.");
            Refresh();
        }
        catch
        {
            Debug.Log("투표 오류");
        }
    }
    // ---------------------------------------------------------------------------------------------------
    // # 트리거 메소드
    // ---------------------------------------------------------------------------------------------------
    void OnTriggerStay(Collider other) // 상호작용
    {
        if (!other.CompareTag("Player") || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if (Input.GetKeyDown(KeyCode.E))
            SetSwitchVote();
    }
}