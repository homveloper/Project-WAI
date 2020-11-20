using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    // 공통
    private static GameManager instance = null;
    public static bool NORMAL_START = false;
    public static bool DEBUG_GAME = true;
    private const int TIME = 1800;

    // 플레이어 객체
    public GameObject mPlayer; // 플레이어 객체 (런타임 중 자동 할당)
    public GameObject mCamera; // 카메라 객체 (런타임 중 자동 할당)

    // 시간
    public float time = TIME;    // 시간
    public float timeMax = TIME; // 시간 (최대치)

    // 기타
    public bool clear;

    private void Awake()
    {
        instance = this;
        PhotonNetwork.IsMessageQueueRunning = true;
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    void Start()
    {
        if (NORMAL_START == false)
        {
            SceneManager.LoadScene("proto_main");
            return;
        }

        ExitGames.Client.Photon.Hashtable ppp = PhotonNetwork.LocalPlayer.CustomProperties;
        Debug.Log(ppp.Keys.Count);

        GetComponent<FadeController>().OnBlack();
        GameObject.Find("UI_Game").GetComponent<Canvas>().enabled = false;
        time = TIME;
        timeMax = TIME;

        PhotonNetwork.IsMessageQueueRunning = true;

        ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;
        localProp.Remove("isAlien");
        localProp["isStart"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(localProp);

        if (PhotonNetwork.IsMasterClient)
            StartCoroutine(GameReadyByMasterClient());

        StartCoroutine(GameReadyByPlayer());
    }

    void Update()
    {
        // 디버그 모드 전환
        if (Input.GetKeyDown(KeyCode.F12) == true)
            DEBUG_GAME = !DEBUG_GAME;

        // 강제 게임 종료 (* 디버그 모드 전용)
        if (DEBUG_GAME == true && Input.GetKeyDown(KeyCode.F11) == true)
            SetFinish(true);

        // 시간 차감
        if (time > 0.0f) time -= Time.deltaTime;
    }

    // ---------------------------------------------------------------------------------------------------
    // 게임 루틴 메소드
    // ---------------------------------------------------------------------------------------------------
    // GameReadyMasterClient : 모든 플레이어의 Scene 이동이 끝나면 역할 배분 시작 (* 마스터 클라이언트용)
    IEnumerator GameReadyByMasterClient()
    {
        while (true)
        {
            int countOfReady = 0; // 필드로 이동 완료한 플레이어 수

            Photon.Realtime.Player[] player = PhotonNetwork.PlayerList;

            for (int i = 0; i < player.Length; i++)
            {
                ExitGames.Client.Photon.Hashtable prop = player[i].CustomProperties;

                if (prop.ContainsKey("isStart") && (bool)prop["isStart"] == true)
                    countOfReady++;
            }

            if (countOfReady >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                ExitGames.Client.Photon.Hashtable roomProp = PhotonNetwork.CurrentRoom.CustomProperties;
                int[] pick = randomPick(0, player.Length, (int)roomProp["countOfAliens"]);

                for (int i = 0; i < player.Length; i++)
                {
                    ExitGames.Client.Photon.Hashtable playerProp = player[i].CustomProperties;

                    playerProp["spawnIndex"] = (i + 1);
                    playerProp["isAlien"] = pick.Contains(i);
                    player[i].SetCustomProperties(playerProp);
                }
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
    // GameReadyPlayer : 마스터 클라이언트의 역할 배분이 끝나면 게임 시작 (* 모든 플레이어용)
    IEnumerator GameReadyByPlayer()
    {
        while (true)
        {
            int countOfReady = 0; // 필드로 이동 완료한 플레이어 수

            Photon.Realtime.Player[] player = PhotonNetwork.PlayerList;

            for (int i = 0; i < player.Length; i++)
            {
                ExitGames.Client.Photon.Hashtable prop = player[i].CustomProperties;

                if (prop.ContainsKey("isAlien"))
                    countOfReady++;
            }

            if (countOfReady >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                ExitGames.Client.Photon.Hashtable localProp = PhotonNetwork.LocalPlayer.CustomProperties;

                // 캐릭터 스폰
                Transform[] points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
                int idx = localProp.ContainsKey("spawnIndex") == true ? (int)localProp["spawnIndex"] : 1;
                mPlayer = PhotonNetwork.Instantiate("Third Person Player", points[idx].position, Quaternion.identity);
                mPlayer.GetComponent<Player>().SetMove(false);

                // 미션 부여
                if ((bool)localProp["isAlien"] == false)
                {
                    GetComponent<MiniAlertController>().OnEnableAlert("연구원", "당신은 연구원입니다.\n우주선을 고쳐 이곳을 탈출하세요.", new Color(0.06666667f, 0.2f, 0.8f));
                    GetComponent<MissionController>().OnSetHeader("연구원 목표");
                    GetComponent<MissionController>().OnAdd("우주선을 수리하고 탈출하기");
                    GetComponent<MissionController>().OnAdd("1분 대기하기"); // 미션 디버그용 코드
                }
                else if ((bool)localProp["isAlien"] == true)
                {
                    GetComponent<MiniAlertController>().OnEnableAlert("외계인", "당신은 외계인입니다.\n연구원들을 방해하고 처치하세요.", new Color(0.8f, 0.2f, 0.06666667f));
                    GetComponent<MissionController>().OnSetHeader("외계인 목표");
                    GetComponent<MissionController>().OnAdd("연구원들을 방해하고 처치하기");
                    GetComponent<MissionController>().OnAdd("1분 대기하기"); // 미션 디버그용 코드
                }

                // 카메라 설정
                mCamera = GameObject.Find("CineMachine");
                mCamera.GetComponent<CinemachineFreeLook>().Follow = mPlayer.transform;
                mCamera.GetComponent<CinemachineFreeLook>().LookAt = mPlayer.transform;
                mCamera.GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Height = 27.5f;
                mCamera.GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Radius = 200.0f;

                // 아이템 박스 생성
                GetComponent<ResponeItem>().CreateItem();

                // 라이트 켜기
                GameObject stone = GameObject.Find("CenterStone");
                stone.transform.Find("Light1").GetComponent<Light>().enabled = true;
                stone.transform.Find("Light2").GetComponent<Light>().enabled = true;

                // 기본 설정
                GameObject.Find("UI_Game").GetComponent<Canvas>().enabled = true;
                GameObject.Find("Main Camera").GetComponent<AudioSource>().Play();

                // 플레이어 움직임 설정
                mPlayer.GetComponent<Player>().SetMove(true);

                // 페이드 인
                GetComponent<FadeController>().OnFadeIn();

                // 카메라 이동
                while (true)
                {
                    float radius = GameObject.Find("CineMachine").GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Radius;
                    GameObject.Find("CineMachine").GetComponent<CinemachineFreeLook>().m_Orbits[1].m_Radius -= (Time.deltaTime * 75);

                    if (radius <= 24.0f)
                        break;
                }

                // 게임 체커 루틴 시작 (* 마스터 클라이언트용)
                StartCoroutine(GameChecker());
                break;
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
    // GameReadyPlayer : 게임 조건을 지속 확인 (* 마스터 클라이언트용)
    IEnumerator GameChecker()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (!PhotonNetwork.IsMasterClient)
                continue;

            // 시간 동기화
            photonView.RPC("OnTime", RpcTarget.AllBuffered, time);

            // 미션 달성 조건 확인 (* 디버깅용)
            GetComponent<MissionController>().OnModify("1분 대기하기", "(" + (int)(time - (TIME - 60)) + "초 남음)");

            if (time < (TIME - 59))
                GetComponent<MissionController>().OnClear("1분 대기하기");

            // 연구원 수 계산
            GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
            int countOfResearcher = player.Length;

            for (int i = 0; i < player.Length; i++)
            {
                ExitGames.Client.Photon.Hashtable prop = player[i].GetComponent<PhotonView>().Owner.CustomProperties;
                if (prop.ContainsKey("isAlien") == true && (bool)prop["isAlien"] == true)
                    countOfResearcher--;
            }

            // 게임 패배 조건 : 연구원 수 = 0 (디버그 모드 상태에서는 발동하지 않음)
            if (!DEBUG_GAME && countOfResearcher <= 0)
            {
                SetFinish(false);
                break;
            }

            // 게임 승리 조건 : 목적 달성
            if (clear && time <= 0)
            {
                SetFinish(true);
                break;
            }
        }
    }

    // ---------------------------------------------------------------------------------------------------
    // 메소드
    // ---------------------------------------------------------------------------------------------------
    public int[] randomPick(int min, int max, int count) // 숫자 뽑기 (min 포함 ~ max 제외 범위)
    {
        HashSet<int> pick = new HashSet<int>();

        while (pick.Count < count)
        {
            pick.Add(Random.Range(min, max));
        }

        return pick.ToArray<int>();
    }
    public void SetFinish(bool win) // 게임 종료
    {
        if (PhotonNetwork.IsMasterClient == false)
            return;

        photonView.RPC("OnFinish", RpcTarget.AllBuffered, win);
    }
    // ---------------------------------------------------------------------------------------------------
    // RPC 메소드
    // ---------------------------------------------------------------------------------------------------
    [PunRPC]
    public void OnTime(float time) // 시간 동기화
    {
        this.time = time;
    }
    [PunRPC]
    public IEnumerator OnFinish(bool win) // 게임 종료 동기화
    {
        mPlayer.GetComponent<Player>().SetMove(false);

        GetComponent<FadeController>().OnFadeOut();

        // 결과창 - 외계인 정보
        Photon.Realtime.Player[] player = PhotonNetwork.PlayerList;
        List<string> alien = new List<string>();
        for (int i = 0; i < player.Length; i++)
        {
            ExitGames.Client.Photon.Hashtable playerProp = player[i].CustomProperties;

            if (playerProp.ContainsKey("isAlien") == true && (bool)playerProp["isAlien"] == true)
                alien.Add(player[i].NickName);
        }
        GameObject.Find("UI_ResultList_Text").GetComponent<Text>().text = string.Join(", ", alien);

        yield return new WaitForSeconds(0.9f);

        // 플레이어 프로퍼티 초기화
        ExitGames.Client.Photon.Hashtable prop = PhotonNetwork.LocalPlayer.CustomProperties;
        int color = (int)prop["color"];

        prop.Clear();
        prop["color"] = color;

        PhotonNetwork.LocalPlayer.SetCustomProperties(prop);

        // 룸 프로퍼티 설정
        if (PhotonNetwork.IsMasterClient == true)
            PhotonNetwork.CurrentRoom.IsOpen = true;

        // 미션 오브젝트 숨김
        GetComponent<MissionController>().OnHide();

        // 결과창 켜기
        GameInterfaceManager.GetInstance().OnSwitchEnd();

        // 카메라
        mCamera.GetComponent<CinemachineFreeLook>().Follow = GameObject.Find("Spaceship").transform;
        mCamera.GetComponent<CinemachineFreeLook>().LookAt = GameObject.Find("CenterStone").transform;

        if (win)
            GameObject.Find("UI_Result_Text").GetComponent<Text>().text = "연구원 승리";
        else
            GameObject.Find("UI_Result_Text").GetComponent<Text>().text = "외계인 승리";

        // 화면 페이드 인
        GetComponent<FadeController>().OnFadeIn();

        // 대기
        yield return new WaitForSeconds(5.0f);

        // 관련 오브젝트 제거
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);

        SceneManager.LoadScene("proto_main");
    }
}

// 벽 객체
// private List<Renderer> wallList;
// private List<Renderer> storeList;
// public string wallName;

//wallList = new List<Renderer>();
//storeList = new List<Renderer>();
//wallName = null;

//FadeWall();
// FadeInWall();

//
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