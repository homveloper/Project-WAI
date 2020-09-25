using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertController : MonoBehaviour
{
    GameObject alert = null;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnEnableAlert(string title, string text)
    {
        UnityEngine.Object prefab = Resources.Load("UI/UI_Alert");
        alert = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);

        alert.transform.Find("UI_Alert_Title").gameObject.GetComponent<Text>().text = title;
        alert.transform.Find("UI_Alert_Text").gameObject.GetComponent<Text>().text = text;

        alert.transform.Find("UI_Alert_Button").gameObject.GetComponent<Button>().onClick.AddListener(OnDisableAlert);
    }

    public void OnDisableAlert()
    {
        if (alert != null)
        {
            Destroy(alert);
        }
    }
}
