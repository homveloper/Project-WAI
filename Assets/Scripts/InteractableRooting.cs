﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Photon.Pun;

[Serializable]
public class InteractableRooting : MonoBehaviourPun
{
    [SerializeField]
    protected GameObject buttonHint;

    [SerializeField]
    String key = "Showing Key when you approache example 'E'";

    [SerializeField]
    KeyCode keyCode;

    [SerializeField]
    string description;
    bool inTrigger;
    bool hasInteracted = false;

    [SerializeField]
    bool isScreenSpace = false;

    [SerializeField]
    Color color;
    Text UI_Key;
    Text UI_Description;
    Transform camTransform;
    Quaternion originalRotation;

    protected Player target;

    public void Rooting()
    {
        target.SetRooting(this.GetComponent<Player>());
    }
    void Start()
    {
        // UI의 초기 회전값
        originalRotation = buttonHint.transform.rotation;

        // 초기화
        UI_Key = buttonHint.transform.Find("Key").GetComponent<Text>();
        buttonHint.transform.Find("Background").GetComponent<Image>().color = color;
        UI_Description = buttonHint.transform.Find("Description").GetComponent<Text>();

        UI_Key.text = key;
        UI_Description.text = description;
    }
    void Update()
    {
        if (inTrigger)
            buttonHint.transform.rotation = camTransform.rotation * originalRotation;

        if (Input.GetKeyDown(_KeyCode) && inTrigger)
            Rooting();
    }
    private void OnTriggerStay(Collider other)
    {
        // 트리거 대상이 내가 아니면 미반응
        if (!other.CompareTag("Player") || other.GetComponent<PhotonView>() == null || other.GetComponent<PhotonView>().IsMine == false)
            OnButtonHint(false);

        // 해당 객체가 죽지 않았으면 미반응
        else if (!GetComponent<Player>().IsDead())
            OnButtonHint(false);

        // 내가 외계인 역할이 아니거나, 외계인 상태가 아니면 미반응
        else if (!other.GetComponent<Player>().IsAlienPlayer() || !other.GetComponent<Player>().IsAlienObject())
            OnButtonHint(false);

        // 내가 조작 제한 상태면 미반응
        else if (!other.GetComponent<Player>().IsControllable())
            OnButtonHint(false);

        // 해당 객체가 소멸했다면 미반응
        else if (GetComponent<Player>().IsTakeOvered())
            OnButtonHint(false);

        // 모든 제한 조건을 통과했으면 출력
        else
            OnButtonHint(true, Camera.main.transform, other.GetComponent<Player>());
    }
    private void OnTriggerExit(Collider other)
    {
        OnButtonHint(false);
    }
    void OnButtonHint(bool active)
    {
        OnButtonHint(active, null, null);
    }
    void OnButtonHint(bool active, Transform transform, Player player)
    {
        if (buttonHint == null)
            return;

        inTrigger = active;
        camTransform = active ? transform : null;
        target = active ? player : null;
        buttonHint.SetActive(active);
    }
    public KeyCode _KeyCode
    {
        get => keyCode;
    }
    public GameObject ButtonHint
    {
        get => buttonHint;
    }
    public string Key
    {
        get => key;
    }
}