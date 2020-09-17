using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    GameObject canvas = null;
    GameObject panel = null;
    void Awake()
    {
        UnityEngine.Object prefab = Resources.Load("Common/UI_Fade");
        canvas = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        canvas.GetComponent<RectTransform>().SetAsLastSibling();
        panel = canvas.transform.Find("UI_Fade_Panel").gameObject;
        panel.GetComponent<RectTransform>().SetAsLastSibling();
        canvas.GetComponent<Canvas>().sortingOrder = 99;
        canvas.GetComponent<Canvas>().enabled = false;
    }

    public bool IsPlaying()
    {
        return panel.GetComponent<Animation>().isPlaying;
    }
    public void OnBlack()
    {
        canvas.GetComponent<Canvas>().enabled = true;
        panel.GetComponent<Image>().color = new Color(0, 0, 0, 1);
    }

    public void OnWhite()
    {
        canvas.GetComponent<Canvas>().enabled = false;
    }

    public void OnFadeIn()
    {
        canvas.GetComponent<Canvas>().enabled = true;
        panel.GetComponent<Animation>().Play("Panel_FadeIn");
    }

    public void OnFadeOut()
    {
        canvas.GetComponent<Canvas>().enabled = true;
        panel.GetComponent<Animation>().Play("Panel_FadeOut");
    }
}
