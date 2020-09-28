using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using static System.Random;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviourPunCallbacks
{
    static GameManager instance = null;

    // 객체
    GameObject mPlayer;
    GameObject mCamera;

    // 시간
    public float time;    // 시간
    public float timeMax; // 시간 (최대치)

    // 플래그
    bool flag_start = false; // 인스턴스 생성 체크용 플래그. 첫 시작 시 true로 변경됨

    private void Awake()
    {
        if (instance == null)
            instance = this; // 최초 생성인 경우 해당 오브젝트를 계속 인스턴스로 가져감
        else if (instance != this)
            Destroy(gameObject); // 이후 게임 매니저를 포함한 오브젝트는 삭제

        
        PhotonNetwork.IsMessageQueueRunning = true;

        DontDestroyOnLoad(gameObject); // 최초 생성된 오브젝트를 유지
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        if (flag_start == false)
        {
            ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;
            if (localProp.ContainsKey("isStart") == false || (bool)localProp["isStart"] == false)
            {
                localProp["isStart"] = true;
                PhotonNetwork.LocalPlayer.SetCustomProperties(localProp);
            }

            flag_start = true;

            GameReady();
        }
    }

    void Update()
    {
        // 시간 차감
        if (time > 0.0f) time -= Time.deltaTime;


        // 산소 차감
        mPlayer.GetComponent<Player>().statO2 -= Time.deltaTime;

        // 체력 차감
        if (mPlayer.GetComponent<Player>().statO2 <= 0)
            mPlayer.GetComponent<Player>().statHp -= (Time.deltaTime * 5);
    }

    void GameReady() // 모든 유저의 이동이 끝날 때까지 대기하는 함수
    {
        int countOfStart = 0; // 필드로 이동 완료한 플레이어 수

        Photon.Realtime.Player[] player = PhotonNetwork.PlayerList;

        for (int i = 0; i < player.Length; i++)
        {
            ExitGames.Client.Photon.Hashtable prop = player[i].CustomProperties;

            if (prop.ContainsKey("isStart") && (bool)prop["isStart"] == true)
                countOfStart++;
        }

        if (countOfStart >= PhotonNetwork.CurrentRoom.PlayerCount)
            GameStart();
        else
            Invoke("GameReady", 1.0f); // 모두 이동이 끝나지 않았다면 대기
    }

    void GameStart() // 전체 유저의 게임 시작 함수
    {
        // 스폰
        Transform[] points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        mPlayer = PhotonNetwork.Instantiate("Third Person Player", points[idx].position, Quaternion.identity);
        GetComponent<GameInterfaceManager>().playerObject = mPlayer;

        mCamera = GameObject.Find("CineMachine");
        mCamera.GetComponent<CinemachineFreeLook>().Follow = mPlayer.transform;
        mCamera.GetComponent<CinemachineFreeLook>().LookAt = mPlayer.transform;

        DontDestroyOnLoad(mPlayer);

        // 타이머 시작
        time = 1800.0f;
        timeMax = 1800.0f;

        checkTimer();
    }

    void checkTimer() // 마스터 클라이언트의 시간으로 나머지 플레이어의 시간을 동기화하는 함수
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            photonView.RPC("OnTime", RpcTarget.AllBuffered, time);
        }
        
        Invoke("checkTimer", 1.0f);
    }

    [PunRPC]
    public void OnTime(float time) // RPC로 시간을 수신하는 함수
    {
        this.time = time;
    }
}
