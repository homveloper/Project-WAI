using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLerp : MonoBehaviour
{
    Rigidbody rigidbody;

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
        rigidbody = GetComponent<Rigidbody>();

        position = transform.position;
        moveTo = new Vector3( Random.Range(-maxRange, maxRange), 0,Random.Range(-maxRange, maxRange));
        torqueTo = Vector3.up * Random.Range(1f,1.5f) * torqueSpeed * Time.deltaTime;
        rigidbody.AddForce(moveTo,ForceMode.VelocityChange);
        rigidbody.AddTorque(torqueTo, ForceMode.VelocityChange);
    }
}
