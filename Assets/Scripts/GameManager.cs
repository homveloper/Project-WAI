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
    GameObject mPlayer;
    GameObject mCamera;

    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;

        CreatePlayer();
    }

    void Update()
    {
        
    }

    void CreatePlayer()
    {
        Transform[] points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        mPlayer = PhotonNetwork.Instantiate("Third Person Player", points[idx].position, Quaternion.identity);
        mCamera = GameObject.Find("CineMacine");

        mCamera.GetComponent<CinemachineFreeLook>().Follow = mPlayer.transform;
        mCamera.GetComponent<CinemachineFreeLook>().LookAt = mPlayer.transform;
    }
}
