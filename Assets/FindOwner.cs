using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FindOwner : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for(int i=0; i<players.Length; i++){
            if(players[i].GetComponent<PhotonView>().OwnerActorNr == photonView.OwnerActorNr){
                Transform rightHand = TransformExtention.FirstOrDefault(players[i].transform ,x => x.name == "mixamorig:RightHand");
                transform.SetParent(rightHand);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                break;
            }
        }
    }
}
