using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnItem : MonoBehaviour
{
    public GameObject item;

    // Start is called before the first frame update
    void Start()
    {
        //for (int i = 0; i < 5; i++)
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(item, transform.position + new Vector3(0, 2, 0), Quaternion.identity);
        }
    }
}
