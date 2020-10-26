using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using static System.Random;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    private const int TIME = 1800;
    public static bool DEBUG_GAME = true;
    private static GameManager instance = null;

    // 플레이어 객체
    public GameObject mPlayer; // 플레이어 객체 (런타임 중 자동 할당)
    public GameObject mCamera; // 카메라 객체 (런타임 중 자동 할당)
    public GameObject[] inGamePlayerList;
    GameObject[] inGameDeadPlayerList;
    PlayerColorPalette colorPalettte;

    // 벽 객체
    private List<Renderer> wallList;
    private List<Renderer> storeList;
    public string wallName;

    // 시간
    public float time = TIME;    // 시간
    public float timeMax = TIME; // 시간 (최대치)

    // 플래그
    public bool flag_start = false; // 인스턴스 생성 체크용 플래그. 첫 시작 시 true로 변경됨
    public bool flag_gameStart = false; // 모든 플레이어가 준비됨
    public bool flag_gameStartFinish = false; // 모든 플레이어가 준비되었고, 게임시작 애니메이션을 마침
    public bool flag_finish = false; // 게임이 종료되었음을 나타내는 플래그
    public bool flag_win = false; // 종료 플래그
    public bool flag_alertJob = false; // 역할 알림 완료 플래그

    private void Awake()
    {
        instance = this; // 최초 생성인 경우 해당 오브젝트를 계속 인스턴스로 가져감

        PhotonNetwork.IsMessageQueueRunning = true;

        wallList = new List<Renderer>();
        storeList = new List<Renderer>();
        wallName = null;
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    void Start()
    {
        GameObject.Find("UI_Game").GetComponent<Canvas>().enabled = false;
        time = TIME;
        timeMax = TIME;

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
        // [디버깅용] 디버그 모드 전환
        if (Input.GetKeyDown(KeyCode.F12) == true)
        {
            DEBUG_GAME = !DEBUG_GAME;
        }

        // [디버깅용] 강제 게임 종료
        if (DEBUG_GAME == true && Input.GetKeyDown(KeyCode.F11) == true)
        {
            SetFinish(true);
        }

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

        if (flag_gameStartFinish == false)
            return;

        // 게임 종료 애니메이션 처리
        if (flag_finish == true && flag_win == true)
        {
            Vector3 pos = GameObject.Find("CineMachine").transform.position;
            pos.y += 1;
            GameObject.Find("CineMachine").transform.position = pos;
        }

        // 시간 차감
        if (time > 0.0f) time -= Time.deltaTime;

        // 미션 디버그용 코드 시작
        GetComponent<MissionController>().OnModify("1분 대기하기", "(" + (int)(time - (TIME - 60)) + "초 남음)");

        if (time < (TIME - 59))
            GetComponent<MissionController>().OnClear("1분 대기하기");
        // 미션 디버그용 코드 종료

        // 게임 종료 후 퇴장
        if (flag_finish && Input.anyKey && time <= (TIME - 5))
        {
            exit();
        }

        // 외계인 선택 완료 이후, 연구원이 0명이 되면 게임이 종료
        if (DEBUG_GAME == false && flag_alertJob == true && time <= (TIME - 5))
            checkFinish();

        if (mPlayer == null)
            return;

        //FadeWall();
       // FadeInWall();
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

        // 아이템박스 생성
        GetComponent<ResponeItem>().CreateItem();

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

        // 외계인 선정
        if (PhotonNetwork.IsMasterClient == true)
        {
            ExitGames.Client.Photon.Hashtable roomProp = PhotonNetwork.CurrentRoom.CustomProperties;
            Photon.Realtime.Player[] player = PhotonNetwork.PlayerList;

            int[] pick = randomPick(0, player.Length, (int)roomProp["countOfAliens"]);

            for (int i = 0; i < player.Length; i++)
            {
                ExitGames.Client.Photon.Hashtable playerProp = player[i].CustomProperties;
                playerProp["isAlien"] = pick.Contains(i);
                player[i].SetCustomProperties(playerProp);
            }
        }

        // 플레이어 움직임 설정
        mPlayer.GetComponent<Player>().SetMove(true);

        // 타이머 시작
        checkTimer();
    }

    public int[] randomPick(int min, int max, int count)
    {
        // min 포함 ~ max 제외 범위의 숫자 중 count 개만큼을 뽑아서 출력
        
        HashSet<int> pick = new HashSet<int>();

        while (pick.Count < count)
        {
            pick.Add(Random.Range(min, max));
        }

        return pick.ToArray<int>();
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        if (flag_alertJob == false && targetPlayer.IsLocal == true && changedProps.ContainsKey("isAlien") == true)
        {
            Debug.Log(targetPlayer.NickName);
            Debug.Log((bool)changedProps["isAlien"]);

            if ((bool)changedProps["isAlien"] == false)
            {
                GetComponent<MiniAlertController>().OnEnableAlert("연구원", "당신은 연구원입니다.\n우주선을 고쳐 이곳을 탈출하세요.", new Color(0.06666667f, 0.2f, 0.8f));
                GetComponent<MissionController>().OnSetHeader("연구원 목표");
                GetComponent<MissionController>().OnAdd("우주선을 수리하고 탈출하기");
                GetComponent<MissionController>().OnAdd("1분 대기하기"); // 미션 디버그용 코드
            }
            else if ((bool)changedProps["isAlien"] == true)
            {
                GetComponent<MiniAlertController>().OnEnableAlert("외계인", "당신은 외계인입니다.\n연구원들을 방해하고 처치하세요.", new Color(0.8f, 0.2f, 0.06666667f));
                GetComponent<MissionController>().OnSetHeader("외계인 목표");
                GetComponent<MissionController>().OnAdd("연구원들을 방해하고 처치하기");
                GetComponent<MissionController>().OnAdd("1분 대기하기"); // 미션 디버그용 코드
            }

            flag_alertJob = true;
        }
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
        inGameDeadPlayerList = GameObject.FindGameObjectsWithTag("DeadPlayer");

        for (int i = 0; i < inGamePlayerList.Length; i++)
        {
            ExitGames.Client.Photon.Hashtable prop = inGamePlayerList[i].GetComponent<PhotonView>().Owner.CustomProperties;
            inGamePlayerList[i].transform.Find("spacesuit").Find("body").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalettte.colors[(int)prop["color"]]);
            inGamePlayerList[i].transform.Find("spacesuit").Find("head").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor", colorPalettte.colors[(int)prop["color"]]);
        }

        for (int i = 0; i < inGameDeadPlayerList.Length; i++)
        {
            ExitGames.Client.Photon.Hashtable prop = inGameDeadPlayerList[i].GetComponent<PhotonView>().Controller.CustomProperties;
            inGameDeadPlayerList[i].transform.Find("body").GetComponent<MeshRenderer>().material.SetColor("_MainColor", colorPalettte.colors[(int)prop["color"]]);
            inGameDeadPlayerList[i].transform.Find("head").GetComponent<MeshRenderer>().material.SetColor("_MainColor", colorPalettte.colors[(int)prop["color"]]);
        }
    }
    /*void FadeWall()
    {
        float Distance = Vector3.Distance(mCamera.transform.position , mPlayer.transform.position);
        Vector3 Direction = (mPlayer.transform.position - mCamera.transform.position).normalized;
        RaycastHit hit;
        if( Physics.Raycast(mCamera.transform.position, Direction , out hit, Distance) )
        {
            Renderer ObstacleRenderer = hit.transform.GetComponentInChildren<Renderer>();

            if(ObstacleRenderer == null)
                return ;
            if(ObstacleRenderer.name == "Spaceship")
                return ;
            if(ObstacleRenderer.name == "well1")
                return ;

                if( ObstacleRenderer  != null )
                {
                    foreach(Renderer r in mPlayer.GetComponentsInChildren<Renderer>())
                    {
                        if(ObstacleRenderer == r)
                            return;
                    }

                    Material Mat = ObstacleRenderer.material;
                    
                    if (Mat.HasProperty("_Color") == true)
                    {
                        Color matColor = Mat.color;
                        matColor =  new Color(matColor.r , matColor.g,matColor.b, 0.5f);
                        Mat.color = matColor;
                    }
                    if(wallList.Count == 0 || wallList[0].name != ObstacleRenderer.name)
                        wallList.Add(ObstacleRenderer);

                    wallName = ObstacleRenderer.name;
                }
        }
    }
    void FadeInWall()
    {
        if(wallList.Count > 0)
        {
            foreach(Renderer Rname in wallList)
            {
                if(Rname.name == wallName )
                    continue;
                else
                {
                    Renderer ObstacleRenderer = Rname;
                
                        if( ObstacleRenderer  != null )
                        {
                            Material Mat = ObstacleRenderer.material;
                            if(Mat.HasProperty("_Color") == true)
                            {
                                Color matColor = Mat.color;
                                matColor =  new Color(matColor.r , matColor.g,matColor.b, 1f);
                                Mat.color = matColor;
                            }
                        }
                        storeList.Add(Rname);
                }
            }
        }
        foreach(Renderer Dname in storeList)
        {
            wallList.Remove(Dname);
        }
        
        storeList.Clear();
        wallName = "";
    }*/

    [PunRPC]
    public void OnTime(float time) // RPC로 시간을 수신하는 함수
    {
        this.time = time;
    }

    // ---------------------------------------------------------------------------------------------------
    // 게임 종료 처리
    // ---------------------------------------------------------------------------------------------------
    public void checkFinish() // 종료 조건 체크
    {
        if (GetComponent<FadeController>().IsPlaying() == true)
            return;

        if (flag_finish == true)
            return;

        // 연구원 수가 0명이 되면 패배 처리
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        int countOfResearcher = player.Length;

        for (int i = 0; i < player.Length; i++)
        {
            ExitGames.Client.Photon.Hashtable prop = player[i].GetComponent<PhotonView>().Owner.CustomProperties;
            if (prop.ContainsKey("isAlien") == true && (bool)prop["isAlien"] == true)
                countOfResearcher--;
        }

        if (countOfResearcher <= 0)
            SetFinish(false);
    }

    public void SetFinish(bool win)
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        photonView.RPC("OnFinish", RpcTarget.AllBuffered, win);
    }

    [PunRPC]
    public void OnFinish(bool win) // 게임 종료 수신 함수
    {
        flag_win = win;
        GetComponent<FadeController>().OnFadeOut();
        Invoke("OnFinishCallback", 0.9f);
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

        // 룸 프로퍼티 초기화
        if (PhotonNetwork.IsMasterClient == true)
            PhotonNetwork.CurrentRoom.IsOpen = true;

        // 관련 오브젝트 제거
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);

        // 미션 오브젝트 숨김
        GetComponent<MissionController>().OnHide();

        // 플래그 활성화
        flag_finish = true;

        if (flag_win == true)
        {
            GameObject.Find("CineMachine").GetComponent<CinemachineFreeLook>().Follow = null;
            GameObject.Find("CineMachine").GetComponent<CinemachineFreeLook>().LookAt = GameObject.Find("CenterStone").transform;
            GameObject.Find("UI_Result_Text").GetComponent<Text>().text = "연구원 승리";
        }
        else
        {
            GameObject.Find("UI_Result_Text").GetComponent<Text>().text = "외계인 승리";
        }

        // 화면 걷어내기
        GetComponent<FadeController>().OnFadeIn();

        Invoke("exit", 5.0f);
    }

    public void exit()
    {
        SceneManager.LoadScene("proto_main");
    }
}
