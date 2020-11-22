using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class ZoomOut : MonoBehaviour
{
    [SerializeField]
    private CinemachineFreeLook vcam;
    [SerializeField]
    private CinemachineFollowZoom followZoom;

    public GameObject cam;
    ZoomOut zoom;

    float origin = 27;
    float chg = 80;

    void Start()
    {
       cam = GameObject.Find("CineMachine");
       vcam = cam.GetComponent<CinemachineFreeLook>();
       followZoom = cam.GetComponent<CinemachineFollowZoom>();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("c in1");
        if (!other.CompareTag("Player") || !other.GetComponent<PhotonView>().IsMine)
            return;
        Debug.Log("c in2");
        ZoomTo(chg,2);
    }
    void OnTriggerExit(Collider other)
    {
        Debug.Log("c out1");
        if (!other.CompareTag("Player") || !other.GetComponent<PhotonView>().IsMine)
            return;
        Debug.Log("c out2");
        ZoomTo(origin,2);
    }
 
    private void InitializeFromGameObject(GameObject camObj)
    {
        if (camObj != null) 
        {
            Debug.Log("in");
            vcam = camObj.GetComponent<CinemachineFreeLook>();
            followZoom= camObj.GetComponent<CinemachineFollowZoom>();
        }
 
    }
    public void ZoomTo(float destSize, float duration = 0)
    {
        Debug.Log("in");
        followZoom.m_Damping = duration;
        followZoom.m_Width = destSize;
    }
}
