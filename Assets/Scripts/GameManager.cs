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
    public GameObject mPlayer; // 플레이어 객체 (런타임 중 자동 할당)

    public GameObject[] inGamePlayerList;
    public GameObject mCamera; // 카메라 객체 (런타임 중 자동 할당)

    PlayerColorPalette colorPalettte;

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
        colorPalettte = Instantiate(Resources.Load<PlayerColorPalette>("PlayerColorPalette"));

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

        // 미션 디버그용 코드 시작
        GetComponent<MissionController>().OnModify("29분 30초까지 대기하기", "(" + (int)(time - 1770) + "초 남음)");

        if (time < 1771.0f)
            GetComponent<MissionController>().OnClear("29분 30초까지 대기하기");
        // 미션 디버그용 코드 종료
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
        ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;
        Transform[] points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        int idx = localProp.ContainsKey("spawnIndex") == true ? (int)localProp["spawnIndex"] : 1;

        mPlayer = PhotonNetwork.Instantiate("Third Person Player", points[idx].position, Quaternion.identity);
        mPlayer.transform.Find("spacesuit").Find("body").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalettte.colors[(int)localProp["color"]]);
        mPlayer.transform.Find("spacesuit").Find("head").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalettte.colors[(int)localProp["color"]]);

        mCamera = GameObject.Find("CineMachine");
        mCamera.GetComponent<CinemachineFreeLook>().Follow = mPlayer.transform;
        mCamera.GetComponent<CinemachineFreeLook>().LookAt = mPlayer.transform;

        DontDestroyOnLoad(mPlayer);

        // 타이머 시작
        time = 1800.0f;
        timeMax = 1800.0f;

        checkTimer();

        GetComponent<MiniAlertController>().OnEnableAlert("연구원", "당신은 연구원입니다.\n우주선을 고쳐 이곳을 탈출하세요.");
        GetComponent<MissionController>().OnSetHeader("연구원 목표");
        GetComponent<MissionController>().OnAdd("우주선을 수리하고 탈출하기");
        GetComponent<MissionController>().OnAdd("29분 30초까지 대기하기"); // 미션 디버그용 코드

    }

    void checkTimer() // 마스터 클라이언트의 시간으로 나머지 플레이어의 시간을 동기화하는 함수
    {
        if (PhotonNetwork.IsMasterClient == true)
        {
            photonView.RPC("OnTime", RpcTarget.AllBuffered, time);
        }

        Invoke("checkTimer", 1.0f);

        inGamePlayerList = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < inGamePlayerList.Length; i++)
        {   
            ExitGames.Client.Photon.Hashtable prop = inGamePlayerList[i].GetComponent<PhotonView>().Owner.CustomProperties;
            
            prop["player"] = mPlayer;
            inGamePlayerList[i].transform.Find("spacesuit").Find("body").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalettte.colors[(int)prop["color"]]);
            inGamePlayerList[i].transform.Find("spacesuit").Find("head").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalettte.colors[(int)prop["color"]]);
        }
    }

    void syncCharacter(){

    }

    [PunRPC]
    public void OnTime(float time) // RPC로 시간을 수신하는 함수
    {
        this.time = time;
    }
}
