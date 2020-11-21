using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Linq.Expressions;

public class VoteController : MonoBehaviourPunCallbacks
{
    private float REFRESH_TIME = 1.0f;

    public GameObject voteObj_panel;
    private GameObject[] voteObj_color;
    private GameObject[] voteObj_nickname;
    private GameObject[] voteObj_up;
    private GameObject[] voteObj_up_value;
    private GameObject[] voteObj_down;
    private GameObject[] voteObj_down_value;
    private GameObject voteObj_time;
    private GameObject voteObj_text;

    private GameObject[] playerList;
    private Dictionary<int, int> actorList; // actorNumber > playerList Index
    private Dictionary<string, int> nickList; // nickname > actorNumber (먹혔다면 변신한 외계인의 번호)
    private Dictionary<string, Color> colorList; // nickname > color
    private Dictionary<int, string> upVoteList; // actorNumber(sender) > nickname(target)
    private Dictionary<int, string> downVoteList; // actorNumber(sender) > nickname(target)

    private List<String> nickKey; // nickList의 정렬된 키

    bool voting;
    bool isActive;

    void Start()
    {
        voteObj_color = new GameObject[10];
        voteObj_nickname = new GameObject[10];
        voteObj_up = new GameObject[10];
        voteObj_up_value = new GameObject[10];
        voteObj_down = new GameObject[10];
        voteObj_down_value = new GameObject[10];

        actorList = new Dictionary<int, int>();
        nickList = new Dictionary<string, int>();
        colorList = new Dictionary<string, Color>();
        upVoteList = new Dictionary<int, string>();
        downVoteList = new Dictionary<int, string>();

        GameObject panel = voteObj_panel.transform.Find("UI_Vote_Panel").gameObject;
        voteObj_time = panel.transform.Find("UI_Vote_Time").gameObject;
        voteObj_text = panel.transform.Find("UI_Vote_Text").gameObject;
        for (int i = 0; i <= 9; i++)
        {
            GameObject unit = panel.transform.Find("UI_Vote_Unit_" + i).gameObject;
            voteObj_color[i] = unit.transform.Find("UI_Vote_Unit_Color").gameObject;
            voteObj_nickname[i] = unit.transform.Find("UI_Vote_Unit_Nickname").gameObject;
            voteObj_up[i] = unit.transform.Find("UI_Vote_Unit_Up").gameObject;
            voteObj_up_value[i] = voteObj_up[i].transform.Find("UI_Vote_Unit_Up_Value").gameObject;
            voteObj_down[i] = unit.transform.Find("UI_Vote_Unit_Down").gameObject;
            voteObj_down_value[i] = voteObj_down[i].transform.Find("UI_Vote_Unit_Down_Value").gameObject;
        }

        StartCoroutine(OnRefresh());

        OnStatus(true);
        voteObj_panel.SetActive(false);        
    }
    void Update()
    {
        if (!isActive)
            SetSwitchVote(false);
        else if (!GameManager.GetInstance().mPlayer.GetComponent<Player>().IsControllable())
            SetSwitchVote(false);
        else if (isActive && Input.GetKeyDown(KeyCode.W) && !GameManager.GetInstance().clear)
            SetSwitchVote();

        float t = GameManager.GetInstance().time;

        if (voting)
        {
            voteObj_text.GetComponent<Text>().text = "투표가 진행중입니다.";
            voteObj_time.GetComponent<Text>().text = "투표 종료까지 " + (Math.Truncate(t % 300) - 60) + "초 남음";
        }
            
        else
        {
            voteObj_text.GetComponent<Text>().text = "투표가 종료되었습니다.\n가장 의심받은 사람의 모든 재료가 가장 신뢰받은 사람에게로 넘어갑니다.";
            voteObj_time.GetComponent<Text>().text = "다음 투표까지 " + Math.Truncate(t % 300) + "초 남음";
        }
            
        if (!PhotonNetwork.IsMasterClient)
            return;

        if (GameManager.GetInstance().clear)
            return;
        
        if (t % 300 >= 59 && t % 300 < 60 && voting == true)
            photonView.RPC("OnStatus", RpcTarget.AllBuffered, false);
        else if (t % 300 >= 299 && t % 300 < 300 && voting == false)
            photonView.RPC("OnStatus", RpcTarget.AllBuffered, true);

    }
    public void SetSwitchVote() // 투표 창 출력 (스위칭)
    {
        SetSwitchVote(!voteObj_panel.activeSelf);
    }
    public void SetSwitchVote(bool val) // 투표 창 출력 (매뉴얼)
    {
        voteObj_panel.SetActive(val);

        if (val == false)
            return;

        OnRefresh();
    }
    IEnumerator OnRefresh() // 시스템 갱신 루틴
    {
        while (true)
        {
            Refresh();

            yield return new WaitForSeconds(REFRESH_TIME);
        }
    }

    public void Refresh() // 시스템 갱신
    {
        // 초기화
        actorList.Clear();
        nickList.Clear();

        // 플레이어 목록 구성
        playerList = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < playerList.Length; i++)
        {
            int actorNumber = playerList[i].GetComponent<PhotonView>().Owner.ActorNumber;
            Color color = playerList[i].GetComponent<Player>().GetColor(true);
            string nick_in = playerList[i].GetComponent<Player>().GetNickname(true);
            string nick_out = playerList[i].GetComponent<Player>().GetNickname(false);
            bool isAlien = playerList[i].GetComponent<Player>().IsAlienPlayer();

            if (!actorList.ContainsKey(actorNumber))
                actorList.Add(actorNumber, i);

            if (!nickList.ContainsKey(nick_in))
                nickList.Add(nick_in, actorNumber);

            if (isAlien && !nickList.ContainsKey(nick_out))
                nickList.Add(nick_out, actorNumber);
            else if (isAlien && nickList.ContainsKey(nick_out))
                nickList[nick_out] = actorNumber;

            if (!colorList.ContainsKey(nick_in))
                colorList.Add(nick_in, color);
        }

        // 투표수 구성
        Dictionary<string, int> upCountList = upVoteList.GroupBy(r => r.Value).ToDictionary(grp => grp.Key, grp => grp.Count());
        Dictionary<string, int> downCountList = downVoteList.GroupBy(r => r.Value).ToDictionary(grp => grp.Key, grp => grp.Count());

        // 출력 구성
        nickKey = nickList.Keys.ToList<String>();
        nickKey.Sort();
        for (int i = 0; i <= 9; i++)
        {
            voteObj_color[i].GetComponent<Image>().color = new Color(0, 0, 0, 0);
            voteObj_nickname[i].GetComponent<Text>().text = "";
            voteObj_up[i].GetComponent<Button>().interactable = false;
            voteObj_up_value[i].GetComponent<Text>().text = "-";
            voteObj_down[i].GetComponent<Button>().interactable = false;
            voteObj_down_value[i].GetComponent<Text>().text = "-";

            if (nickList == null || i >= nickList.Count)
                continue;

            voteObj_color[i].GetComponent<Image>().color = colorList.ContainsKey(nickKey[i]) ? colorList[nickKey[i]] : new Color (0,0,0,0);
            voteObj_nickname[i].GetComponent<Text>().text = nickKey[i];
            voteObj_up[i].GetComponent<Button>().interactable = voting;
            voteObj_up_value[i].GetComponent<Text>().text = (upCountList.ContainsKey(nickKey[i]) ? upCountList[nickKey[i]] : 0).ToString();
            voteObj_down[i].GetComponent<Button>().interactable = voting;
            voteObj_down_value[i].GetComponent<Text>().text = (downCountList.ContainsKey(nickKey[i]) ? downCountList[nickKey[i]] : 0).ToString();
        }
    }
    public void SetVoteUp(int num) // 투표 (신뢰)
    {
        photonView.RPC("OnVote", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, nickKey[num], true);
    }
    public void SetVoteDown(int num) // 투표 (의심)
    {
        photonView.RPC("OnVote", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, nickKey[num], false);
    }
    public void Finish()
    {
        // 투표수 구성
        Dictionary<string, int> upCountList = upVoteList.GroupBy(r => r.Value).ToDictionary(grp => grp.Key, grp => grp.Count());
        Dictionary<string, int> downCountList = downVoteList.GroupBy(r => r.Value).ToDictionary(grp => grp.Key, grp => grp.Count());

        string maxUpNick = "", maxDownNick = "";
        int maxUp = 0, maxUpCount = 0, maxDown = 0, maxDownCount = 0;

        List<String> upKey = upCountList.Keys.ToList<String>();
        for (int i = 0; i < upKey.Count; i++)
        {
            if (upCountList[upKey[i]] > maxUp)
            {
                maxUpNick = upKey[i];
                maxUp = upCountList[upKey[i]];
                maxUpCount = 1;
            }
            else if (upCountList[upKey[i]] == maxUp)
            {
                maxUpCount += 1;
            }
        }

        List<String> downKey = downCountList.Keys.ToList<String>();
        for (int i = 0; i < downKey.Count; i++)
        {
            if (downCountList[downKey[i]] > maxDown)
            {
                maxDownNick = downKey[i];
                maxDown = downCountList[downKey[i]];
                maxDownCount = 1;
            }
            else if (downCountList[downKey[i]] == maxDown)
            {
                maxDownCount += 1;
            }
        }

        if (maxUp == 0 && maxDown == 0)
        {
            photonView.RPC("OnAlert", RpcTarget.AllBuffered, "투표 결과", "아무도 투표받지 않았습니다.\n재료 전송이 발생하지 않습니다.");
            return;
        }
        else if (maxUp == 0)
        {
            photonView.RPC("OnAlert", RpcTarget.AllBuffered, "투표 결과", "아무도 신뢰받지 못했습니다.\n재료 전송이 발생하지 않습니다.");
            return;
        }
        else if (maxDown == 0)
        {
            photonView.RPC("OnAlert", RpcTarget.AllBuffered, "투표 결과", "아무도 의심받지 않았습니다.\n재료 전송이 발생하지 않습니다.");
            return;
        }
        else if (maxUpCount >= 2)
        {
            photonView.RPC("OnAlert", RpcTarget.AllBuffered, "투표 결과", "신뢰 투표가 동수입니다.\n재료 전송이 발생하지 않습니다.");
            return;
        }
        else if (maxDownCount >= 2)
        {
            photonView.RPC("OnAlert", RpcTarget.AllBuffered, "투표 결과", "의심 투표가 동수입니다.\n재료 전송이 발생하지 않습니다.");
            return;
        }

        GameObject maxUpPlayerObject = playerList[actorList[nickList[maxUpNick]]];
        GameObject maxDownPlayerObject = playerList[actorList[nickList[maxDownNick]]];

        int wood = maxDownPlayerObject.GetComponent<Player>().GetWood();
        int iron = maxDownPlayerObject.GetComponent<Player>().GetIron();
        int part = maxDownPlayerObject.GetComponent<Player>().GetPart();

        maxDownPlayerObject.GetComponent<Player>().SetTransformMeterial(-wood, -iron, -part);
        maxUpPlayerObject.GetComponent<Player>().SetTransformMeterial(wood, iron, part);

        photonView.RPC("OnAlert", RpcTarget.AllBuffered, "투표 결과", "최고 신뢰자: " + maxUpNick + " (" + upCountList[maxUpNick] + "표)\n" + "최고 의심자: " + maxDownNick + " (" + downCountList[maxDownNick] + "표)");
    }
    public void Clear() // 투표 초기화
    {
        upVoteList.Clear();
        downVoteList.Clear();
    }
    // ---------------------------------------------------------------------------------------------------
    // # 네트워크 메소드
    // ---------------------------------------------------------------------------------------------------
    [PunRPC]
    public void OnStatus(bool val) // 투표 상태 설정
    {
        voting = val;

        if (val == true)
            Clear();
        else if (val == false)
            Finish();

        Refresh();
    }
    [PunRPC]
    public void OnAlert(string title, string text)
    {
        GameManager.GetInstance().GetComponent<MiniAlertController>().OnEnableAlert(title, text, new Color(0.2666667f, 0.5333334f, 0.7333333f));
    }
    [PunRPC]
    void OnVote(int sender, string targetNick, bool up) // 투표 수신
    {
        if (up)
            if (upVoteList.ContainsKey(sender)) upVoteList[sender] = targetNick;
            else upVoteList.Add(sender, targetNick);
        else
            if (downVoteList.ContainsKey(sender)) downVoteList[sender] = targetNick;
            else downVoteList.Add(sender, targetNick);

        Refresh();
    }
    // ---------------------------------------------------------------------------------------------------
    // # 트리거 메소드
    // ---------------------------------------------------------------------------------------------------
    void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || !other.GetComponent<PhotonView>().IsMine)
            return;

        isActive = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player") || !other.GetComponent<PhotonView>().IsMine)
            return;

        isActive = false;
    }
}
