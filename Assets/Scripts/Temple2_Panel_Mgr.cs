using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Temple2_Panel_Mgr : MonoBehaviourPun
{
    public GameObject stage;
    public GameObject mgr;
    public GameObject door;

    public bool isPlay = false;
    public int tmp = 1;
    // Start is called before the first frame update
    void Start()
    {
        door.SetActive(false);
    }
    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            return;

        if (other.gameObject.tag != "Player")
            return;

        if (Input.GetKeyDown(KeyCode.E) && tmp == 1 && isPlay == false)
        {
            GameManager.GetInstance().mPlayer.GetComponent<Player>().SetMove(false);
            GameManager.GetInstance().GetComponent<FadeController>().OnFadeOut();
            StartCoroutine(this.OnStartMinigame());
        }
    }

    IEnumerator OnStartMinigame()
    {
        photonView.RPC("SetPlayState", RpcTarget.AllBuffered, true);

        yield return new WaitForSeconds(1);

        stage.SetActive(true);
        MiniGame_enemy[] elist = stage.transform.Find("Enemies").GetComponentsInChildren<MiniGame_enemy>();

        foreach (MiniGame_enemy e in elist)
        {
            e.GetComponent<Animator>().SetInteger("EnemyType", (int)e.type);
        }
        GameInterfaceManager.GetInstance().OnSwitchHide(true);
        photonView.RPC("OnActiveDoor", RpcTarget.AllBuffered);
        GameManager.GetInstance().GetComponent<FadeController>().OnFadeIn();
    }
    // Update is called once per frame
    void Update()
    {
    }

    [PunRPC]
    public void OnActiveDoor()
    {
        door.SetActive(true);
    }

    [PunRPC]
    public void OnDisableDoor()
    {
        door.SetActive(false);
    }

    [PunRPC]
    public void SetClearMsg()
    {
        mgr.GetComponent<Temple2_Mgr>().cnt--;
        tmp = 2;
    }

    [PunRPC]
    public void SetPlayState(bool state)
    {
        isPlay = state;
    }
}
