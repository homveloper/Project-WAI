using System;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class UIManager : MonoBehaviourPunCallbacks
{
    Text _meterial_wood_text;
    Text _meterial_iron_text;
    Text _meterial_part_text;

    Image _stat_hp_bar;
    int _stat_hp_max;

    Image _stat_o2_bar;

    Image _timer_bar;
    Text _timer_text;
    float _timer_time;

    Image _talk_back;
    Image _talk_count;
    Text _talk_count_text;

    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        _meterial_wood_text = GameObject.Find("UI_Meterial_Wood_Text").GetComponent<Text>();
        _meterial_iron_text = GameObject.Find("UI_Meterial_Iron_Text").GetComponent<Text>();
        _meterial_part_text = GameObject.Find("UI_Meterial_Part_Text").GetComponent<Text>();

        _stat_hp_bar = GameObject.Find("UI_HP_Bar").GetComponent<Image>();
        _stat_hp_max = 100;

        _stat_o2_bar = GameObject.Find("UI_O2_Bar").GetComponent<Image>();

        _timer_bar = GameObject.Find("UI_Timer_Bar").GetComponent<Image>();
        _timer_text = GameObject.Find("UI_Timer_Text").GetComponent<Text>();
        _timer_time = 70.0f;

        _talk_back = GameObject.Find("UI_Talk_Button").GetComponent<Image>();
        _talk_count = GameObject.Find("UI_Talk_Count").GetComponent<Image>();
        _talk_count_text = GameObject.Find("UI_Talk_Count_Text").GetComponent<Text>();

        _stat_hp_bar.fillAmount = 1;
        _stat_o2_bar.fillAmount = 1;
        _timer_bar.fillAmount = 1;
        _timer_text.text = string.Format("{0}:{1}", Math.Truncate(_timer_time / 60), Math.Truncate(_timer_time % 60));
    }

    void Update()
    {
        _stat_o2_bar.fillAmount -= (Time.deltaTime / 70);

        _timer_time -= Time.deltaTime;
        _timer_text.text = string.Format("{0:N0}:{1:N0}", (_timer_time / 60), (_timer_time % 60));
        //_timer_bar.fillAmount -= (_timer_time / 70.0f);
    }

    public void OnDebug()
    {
        photonView.RPC("SetDebug", RpcTarget.OthersBuffered);
        SetDebug();
    }

    [PunRPC]
    public void SetDebug()
    {
        System.Random rand = new System.Random();
        _meterial_wood_text.text = rand.Next(1, 200).ToString();
        _meterial_iron_text.text = rand.Next(1, 200).ToString();
        _meterial_part_text.text = rand.Next(1, 200).ToString();
        _stat_hp_bar.fillAmount = rand.Next();

        int count = rand.Next(0, 2);

        if (count == 0)
        {
            _talk_back.sprite = Resources.Load<Sprite>("UI/talk");
            _talk_count.sprite = null;
            _talk_count_text.text = count.ToString();
        }

        else if (count == 1)
        {
            _talk_back.sprite = Resources.Load<Sprite>("UI/talk_active");
            _talk_count.sprite = Resources.Load<Sprite>("UI/talkCount");
            _talk_count_text.text = count.ToString();
        }
    }
}
