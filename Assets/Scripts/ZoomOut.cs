using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class ZoomOut : MonoBehaviour
{
    [SerializeField]
    private CinemachineFreeLook vcam;
    [SerializeField]
    private CinemachineFollowZoom followZoom;
    // Start is called before the first frame update
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
    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player")
            return ;
        ZoomTo(chg,2);
    }
    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag != "Player")
            return ;
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
        followZoom.m_Damping = duration;
        followZoom.m_Width = destSize;
    }
}
