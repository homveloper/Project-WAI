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

    GameObject mPlayer;
    GameObject mCamera;

    float time = 1800.0f; // 시간
    float deltaTime = 0.0f; // fps 체크용

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
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        time -= Time.deltaTime;

        GameObject.Find("FPS").GetComponent<Text>().text = (int)(1.0f / deltaTime) + " FPS";
        GameObject.Find("Ping").GetComponent<Text>().text = PhotonNetwork.GetPing().ToString() + " ms";
    }

    void GameReady() // 모든 유저의 이동이 끝날떄까지 대기하는 함수
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
            Invoke("GameReady", 1.0f);
    }

    void GameStart() // 게임 시작 함수
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

        // 타이머 처리
        time = 1800.0f;

        checkTimer();
    }

    void checkTimer()
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
        GetComponent<GameInterfaceManager>().time = time;
    }
}
