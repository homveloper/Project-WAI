using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlayer : MonoBehaviour
{
    public PlayerColorPalette colorPalettte;
    public GameObject defaultCharacter;
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i<colorPalettte.colors.Capacity; i++){
            GameObject character = Instantiate(defaultCharacter,new Vector3((i-5.0f) * 5.0f, 0, 0), Quaternion.identity);
            character.transform.Find("spacesuit").Find("body").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor",colorPalettte.colors[i]);
            character.transform.Find("spacesuit").Find("head").GetComponent<SkinnedMeshRenderer>().material.SetColor("_MainColor",colorPalettte.colors[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
