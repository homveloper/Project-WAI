using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InteractableBox : Interactable
{
    [SerializeField]
    private int minMaterialWood;
    [SerializeField]
    private int maxMaterialWood;

    [SerializeField]
    private int minmaterialIron;
    [SerializeField]
    private int maxmaterialIron;
    
    [SerializeField]
    private int minmaterialPart;
    [SerializeField]
    private int maxmaterialPart;

    [SerializeField]
    private Animator animator;

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void Interact(){

        int materialWood = Random.Range(minMaterialWood,maxMaterialWood);
        int materialPart = Random.Range(minmaterialPart,maxmaterialPart);
        int materialIron = Random.Range(minmaterialIron,maxmaterialIron);

        if(target != null){
            target.SetWood(target.GetWood() + materialWood);
            target.SetIron(target.GetIron() + materialIron);
            target.SetPart(target.GetPart() + materialPart);

            Debug.Log("상자를 열었습니다.");
            photonView.RPC("DestroyItem", RpcTarget.AllBuffered);
        }

    }

    [PunRPC]
    void DestroyItem() => Destroy(gameObject);
}
