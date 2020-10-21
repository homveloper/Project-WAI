﻿using System.Collections;
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

    static GameObject[] mTransparentWalls;
    static GameObject[] newAddedWall;
    public string wallName = "";
    // 객체
    public GameObject mPlayer; // 플레이어 객체 (런타임 중 자동 할당)

    public GameObject[] inGamePlayerList;
    public GameObject mCamera; // 카메라 객체 (런타임 중 자동 할당)

    PlayerColorPalette colorPalettte;

    //Renderer ObstacleRenderer;
    // 시간
    public float time;    // 시간
    public float timeMax; // 시간 (최대치)

    // 플래그
    public bool flag_start = false; // 인스턴스 생성 체크용 플래그. 첫 시작 시 true로 변경됨
    public bool flag_gameStart = false; // 모든 플레이어가 준비됨
    public bool flag_gameStartFinish = false; // 모든 플레이어가 준비되었고, 게임시작 애니메이션을 마침
    public bool flag_finish = false; // 게임이 종료되었음을 나타내는 플래그


    private void Awake()
    {
        if (instance == null)
            instance = this; // 최초 생성인 경우 해당 오브젝트를 계속 인스턴스로 가져감
        else if (instance != this)
            Destroy(gameObject); // 이후 게임 매니저를 포함한 오브젝트는 삭제


        PhotonNetwork.IsMessageQueueRunning = true;
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    void Start()
    {
        colorPalettte = Instantiate(Resources.Load<PlayerColorPalette>("PlayerColorPalette"));

        PhotonNetwork.IsMessageQueueRunning = true;

        // 게임 시작 코드
        // Scene 이동이 완료되면 네트워크에 알림
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

        // 게임 시작 애니메이션 처리
        if (flag_gameStart == true && flag_gameStartFinish == false)
        {
            float radius = GameObject.Find("CineMachine").GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Radius;
            if (radius > 24.0f)
            {
                GameObject.Find("CineMachine").GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Radius -= (Time.deltaTime * 75);
            }
            else
            {
                GameStartFinish();
            }
        }

        // 게임 종료 후 퇴장
        if (flag_finish && Input.anyKey)
        {
            SceneManager.LoadScene("proto_main");
        }

        // 강제 게임 종료 ** 디버깅용 **
        if (Input.GetKeyDown(KeyCode.F12) == true)
        {
            SetFinish(true);
        }

        if (mPlayer == null)
            return;

        FadeWall();
    }

    // ---------------------------------------------------------------------------------------------------
    // 게임 시작
    // ---------------------------------------------------------------------------------------------------
    void GameReady() // 모든 유저의 Scene 이동이 끝날 때까지 대기하는 함수
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

    void GameStart() // 모든 유저의 Scene 이동이 끝난 후 진행되는 게임 시작 함수
    {
        GetComponent<FadeController>().OnBlack();
        // 스폰
        ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;
        Transform[] points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        int idx = localProp.ContainsKey("spawnIndex") == true ? (int)localProp["spawnIndex"] : 1;

        mPlayer = PhotonNetwork.Instantiate("Third Person Player", points[idx].position, Quaternion.identity);
        mPlayer.transform.Find("spacesuit").Find("body").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalettte.colors[(int)localProp["color"]]);
        mPlayer.transform.Find("spacesuit").Find("head").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalettte.colors[(int)localProp["color"]]);
        mPlayer.GetComponent<Player>().SetMove(false);

        mCamera = GameObject.Find("CineMachine");
        mCamera.GetComponent<CinemachineFreeLook>().Follow = mPlayer.transform;
        mCamera.GetComponent<CinemachineFreeLook>().LookAt = mPlayer.transform;
        mCamera.GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Height = 31.0f;
        mCamera.GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Radius = 200.0f;

        GetComponent<FadeController>().OnFadeIn();
        flag_gameStart = true;
    }

    void GameStartFinish() // 게임 시작 애니메이션이 끝난 후 마무리 처리
    {
        if (flag_gameStartFinish == false)
        {
            flag_gameStartFinish = true;
            Invoke("GameStartFinish", 1.0f);
            return;
        }

        GameObject stone = GameObject.Find("CenterStone");
        stone.transform.Find("Light1").GetComponent<Light>().enabled = true;
        stone.transform.Find("Light2").GetComponent<Light>().enabled = true;

        // 기본 설정
        GameObject.Find("UI_Game").GetComponent<Canvas>().enabled = true;
        GameObject.Find("Main Camera").GetComponent<AudioSource>().Play();

        // 플레이어 움직임 설정
        mPlayer.GetComponent<Player>().SetMove(true);

        // 타이머 시작
        time = 1800.0f;
        timeMax = 1800.0f;
        checkTimer();

        // 미션 시작
        GetComponent<MiniAlertController>().OnEnableAlert("연구원", "당신은 연구원입니다.\n우주선을 고쳐 이곳을 탈출하세요.");
        GetComponent<MissionController>().OnSetHeader("연구원 목표");
        GetComponent<MissionController>().OnAdd("우주선을 수리하고 탈출하기");
        GetComponent<MissionController>().OnAdd("29분 30초까지 대기하기"); // 미션 디버그용 코드

    }

    // ---------------------------------------------------------------------------------------------------
    // 시간 동기화
    // ---------------------------------------------------------------------------------------------------
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
            inGamePlayerList[i].transform.Find("spacesuit").Find("body").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalettte.colors[(int)prop["color"]]);
            inGamePlayerList[i].transform.Find("spacesuit").Find("head").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalettte.colors[(int)prop["color"]]);
        }
    }
    void FadeWall()
    {
        float Distance = Vector3.Distance(mCamera.transform.position , mPlayer.transform.position);
        Vector3 Direction = (mPlayer.transform.position - mCamera.transform.position).normalized;
        RaycastHit hit;
        
        if( Physics.Raycast(mCamera.transform.position, Direction , out hit, Distance) )
        {
            Renderer ObstacleRenderer = hit.transform.GetComponentInChildren<Renderer>();
            if(ObstacleRenderer == null)
                return ;
            if(ObstacleRenderer.name == wallName || wallName == "")
            {
                if( ObstacleRenderer  != null )
                {
                    foreach(Renderer r in mPlayer.GetComponentsInChildren<Renderer>()){
                        if(ObstacleRenderer == r)
                            return;
                    }
                    wallName = ObstacleRenderer.name;
                    Material Mat = ObstacleRenderer.material;
                    
                    if (Mat.HasProperty("_Color") == true)
                    {
                        Color matColor = Mat.color;
                        matColor =  new Color(matColor.r , matColor.g,matColor.b, 0.5f);
                        Mat.color = matColor;
                        wallName = ObstacleRenderer.name;
                    }
                    
                    
                }
            }
            else
            {   
                ObstacleRenderer = GameObject.Find(wallName).GetComponentInChildren<Renderer>();
                Debug.Log(ObstacleRenderer.name+" else");
                if( ObstacleRenderer  != null )
                {
                    foreach(Renderer r in mPlayer.GetComponentsInChildren<Renderer>())
                    {
                        if(ObstacleRenderer == r)
                            return;
                    }

                    Material Mat = ObstacleRenderer.material;
                    if(Mat.HasProperty("_Color") == true)
                    {
                        Color matColor = Mat.color;
                        matColor =  new Color(matColor.r , matColor.g,matColor.b, 1f);
                        Mat.color = matColor;
                        wallName = "";
                    }

                    
                   
                }
            }
        }
    }

    [PunRPC]
    public void OnTime(float time) // RPC로 시간을 수신하는 함수
    {
        this.time = time;
    }

    // ---------------------------------------------------------------------------------------------------
    // 게임 종료 처리
    // ---------------------------------------------------------------------------------------------------
    public void SetFinish(bool win)
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        photonView.RPC("OnFinish", RpcTarget.AllBuffered, win);
    }

    [PunRPC]
    public void OnFinish(bool win) // 게임 종료 수신 함수
    {
        GetComponent<FadeController>().OnFadeOut();
        Invoke("OnFinishCallback", 1.0f);
    }

    public void OnFinishCallback() // 종료 애니메이션 완료 후 처리
    {
        // 프로퍼티 초기화
        ExitGames.Client.Photon.Hashtable prop = PhotonNetwork.LocalPlayer.CustomProperties;
        int color = (int)prop["color"];

        prop["color"] = color;
        prop["isReady"] = false;
        prop["isStart"] = false;

        PhotonNetwork.LocalPlayer.SetCustomProperties(prop);

        // 관련 오브젝트 제거
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);

        // 미션 오브젝터 숨김
        GetComponent<MissionController>().OnHide();

        // 플래그 활성화
        flag_finish = true;

        // 화면 걷어내기
        GetComponent<FadeController>().OnFadeIn();
    }
}
