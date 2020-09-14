using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using static System.Random;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        CreatePlayer();
        PhotonNetwork.IsMessageQueueRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreatePlayer()
    {
        Transform[] points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        int idx = Random.Range(1, points.Length);
        PhotonNetwork.Instantiate("Third Person Player", points[idx].position, Quaternion.identity);
    }
}
