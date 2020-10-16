﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLerp : MonoBehaviour
{
    public Rigidbody rigidbody;

    [SerializeField]
    [Range(0f, 4f)]
    float lerpTime = 2f;

    [SerializeField]
    int minRange = 3;

    [SerializeField]
    int maxRange = 5;

    [SerializeField]
    int torqueSpeed = 300;

    Vector3 moveTo;
    Vector3 torqueTo;
    Vector3 position;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        moveTo = new Vector3(minRange + Random.Range(-maxRange, maxRange), 0, minRange + Random.Range(-maxRange, maxRange));
        torqueTo = Vector3.up * Random.Range(1f,1.5f) * torqueSpeed * Time.deltaTime;
        rigidbody.AddForce(moveTo,ForceMode.VelocityChange);
        rigidbody.AddTorque(torqueTo, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = Vector3.Lerp(position, position + moveTo, lerpTime * Time.deltaTime);
    }
}