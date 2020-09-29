using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniAlertController : MonoBehaviour
{
    public GameObject parentObject = null;
    GameObject alert = null;

    float time = 0.0f; // 타이머
    int flag = 0;      // 플래그 - 0(알림 없음) 1(알림 뜨는 중)

    void Awake()
    {
        UnityEngine.Object prefab = Resources.Load("UI/UI_MiniAlert");
        alert = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);

        alert.transform.parent = parentObject.transform;
        alert.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 60, 0);
    }

    void Update()
    {
        if (flag == 1)
        {
            time += Time.deltaTime;
            alert.transform.Find("UI_MiniAlert_Bar").gameObject.GetComponent<Image>().fillAmount = time / 5.0f;
        }
            

        if (flag == 1 && time >= 5.0f)
            OnDisableAlert();
    }
    public void OnEnableAlert(string title, string text)
    {
        alert.transform.Find("UI_MiniAlert_TopText").gameObject.GetComponent<Text>().text = title;
        alert.transform.Find("UI_MiniAlert_BottomText").gameObject.GetComponent<Text>().text = text;

        alert.GetComponent<Animation>().Play("MiniAlert_show");

        time = 0;
        flag = 1;
    }
    public void OnEnableAlert(string title, string text, Color color, Sprite sprite)
    {
        alert.GetComponent<Image>().color = color;
        alert.transform.Find("UI_MiniAlert_Image").gameObject.GetComponent<Image>().sprite = sprite;

        OnEnableAlert(title, text);
    }

    public void OnDisableAlert()
    {
        alert.GetComponent<Animation>().Play("MiniAlert_hide");

        time = 0;
        flag = 0;
    }
}
