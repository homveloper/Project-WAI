using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Photon.Pun;

public class CameraChanger : MonoBehaviourPun
{
    [SerializeField]
    private CinemachineFreeLook vcam;

    [SerializeField]
    private CinemachineFollowZoom followZoom;

    [SerializeField]
    private Volume globalVolume;

    Vignette vignette;
    LiftGammaGain liftGammaGain;

    [SerializeField]
    Light directionalLight;


    [SerializeField]
    private Player player;

    public GameObject cam;
    ZoomOut zoom;

    [SerializeField]
    float origin = 27;

    [SerializeField]
    float change = 100;

    void Start()
    {
        cam = GameObject.Find("CineMachine");
        directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
        directionalLight.color = new Color(255/255f,124/255f,124/255f);
        vcam = cam.GetComponent<CinemachineFreeLook>();
        followZoom = cam.GetComponent<CinemachineFollowZoom>();
        player = GetComponent<Player>();
        globalVolume = GameObject.Find("Global Volume").GetComponent<Volume>();

        if(globalVolume != null){
            globalVolume.profile.TryGet(out vignette);
            globalVolume.profile.TryGet(out liftGammaGain);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsConnected)
            if (!photonView.IsMine)
                return;

        if (player.IsAlienObject())
        {
            vignette.active = true;
            liftGammaGain.active = true;
            directionalLight.enabled = true;
            ZoomTo(change, 2);
        }
        else
        {
            vignette.active = false;
            liftGammaGain.active = false;
            directionalLight.enabled = false;
            ZoomTo(origin, 2);
        }
    }

    public void ZoomTo(float destSize, float duration = 0)
    {
        followZoom.m_Damping = duration;
        followZoom.m_Width = destSize;
    }
}
