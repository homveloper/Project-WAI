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

    float deltaTime = 0.0f; // fps 체크용

    bool event_start = false;

    private void Awake()
    {
        if (instance == null)
            instance = this; // 최초 생성인 경우 해당 오브젝트를 계속 인스턴스로 가져감
        else if (instance != this)
            Destroy(gameObject); // 이후 게임 매니저를 포함한 오브젝트는 삭제

        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.IsMessageQueueRunning = true;

        DontDestroyOnLoad(gameObject); // 최초 생성된 오브젝트를 유지
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    void Start()
    {
        if (event_start == false)
        {
            GameStart();
            event_start = true;
        }
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        GameObject.Find("FPS").GetComponent<Text>().text = (int)(1.0f / deltaTime) + " FPS";
        GameObject.Find("Ping").GetComponent<Text>().text = PhotonNetwork.GetPing().ToString() + " ms";
    }

    void GameStart() // 최초 시작
    {
        Transform[] points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        mPlayer = PhotonNetwork.Instantiate("Third Person Player", points[idx].position, Quaternion.identity);
        mCamera = GameObject.Find("CineMacine");

        mCamera.GetComponent<CinemachineFreeLook>().Follow = mPlayer.transform;
        mCamera.GetComponent<CinemachineFreeLook>().LookAt = mPlayer.transform;

        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(player);
        for (int i = 0; i<player.Length; i++)
        {
            Debug.Log(player[i]);
            DontDestroyOnLoad(player[i]);
        }
            
    }

    public void ChangeMap(string map)
    {
        mPlayer.transform.position = new Vector3(60, 2, 60);
        SceneManager.LoadScene(map);
    }

    void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Debug.Log(this.gameObject);
        info.Sender.TagObject = this.gameObject;
        DontDestroyOnLoad(this.gameObject);
    }
}
