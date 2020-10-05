using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    public GameObject mPlayer; // 플레이어 객체 (런타임 중 자동 할당)
    public Material mMaterial;

    // Start is called before the first frame update
    void Start()
    {
        mMaterial = mPlayer.transform.Find("spacesuit").Find("body").GetComponent<SkinnedMeshRenderer>().material;
        print(mMaterial);
    }

    // Update is called once per frame
    void Update()
    {
        mMaterial.SetColor("_MainColor",Color.blue);
        // mPlayer.<Renderer>().material.SetColor("Main Color",Color.red);
    }
}
