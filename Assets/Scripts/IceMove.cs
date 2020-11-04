using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceMove : MonoBehaviour
{
    // Start is called before the first frame updatepublic float speed = 5f;
    public float speed;
    public Rigidbody characterRigidbody;
 
    void Start()
    {
        //characterRigidbody = GetComponent<Rigidbody>();
    }
 
    void FixedUpdate()
    {
        // if (Input.GetKey(KeyCode.LeftArrow)){
        //     characterRigidbody.AddForce(-speed, 0, 0);
        // }
        // else if (Input.GetKey(KeyCode.RightArrow)){
        //    characterRigidbody.AddForce(speed, 0, 0);
        // }
        // else if (Input.GetKey(KeyCode.UpArrow)){
        //     characterRigidbody.AddForce(0, 0, speed);
        // }
        // else if (Input.GetKey(KeyCode.DownArrow)){
        //     characterRigidbody.AddForce(0, 0, -speed);
        // }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            characterRigidbody.velocity = new Vector3(-speed, 0, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
           characterRigidbody.velocity = new Vector3(speed, 0, 0);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            characterRigidbody.velocity = new Vector3(0, 0, speed);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            characterRigidbody.velocity = new Vector3(0, 0, -speed);
        }
    }
}
