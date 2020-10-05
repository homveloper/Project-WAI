using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionController : MonoBehaviour
{
    public GameObject parentObject = null;
    GameObject missionObject = null;
    Dictionary<string, Mission> dic;

    void Awake()
    {
        UnityEngine.Object prefab = Resources.Load("UI/UI_Mission");
        missionObject = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);

        missionObject.transform.parent = parentObject.transform;
        missionObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -150, 0);
        missionObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        dic = new Dictionary<string, Mission>();
    }

    public void OnRefresh()
    {
        int index = 0;
        foreach (KeyValuePair<string, Mission> item in dic)
        {
            item.Value.GetObject().GetComponent<RectTransform>().anchoredPosition = new Vector3(275, -24 * index++, 0);
            item.Value.GetObject().transform.Find("UI_Mission_Unit_Text").GetComponent<Text>().text = item.Value.GetTitle() + " " + item.Value.GetExtra();
            if (item.Value.IsCleared() == true)
                item.Value.GetObject().transform.Find("UI_Mission_Unit_Check").GetComponent<Image>().enabled = true;
            else
                item.Value.GetObject().transform.Find("UI_Mission_Unit_Check").GetComponent<Image>().enabled = false;
        }

        if (index == 0)
            GameObject.Find("UI_Mission_Title").GetComponent<Text>().enabled = false;
        else
            GameObject.Find("UI_Mission_Title").GetComponent<Text>().enabled = true;
    }

    public void OnSetHeader(string header)
    {
        GameObject.Find("UI_Mission_Title").GetComponent<Text>().text = header;
    }
    public void OnAdd(string title, string extra = "")
    {
        UnityEngine.Object prefab = Resources.Load("UI/UI_Mission_Unit");
        GameObject obj = (GameObject)GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity,missionObject.transform);

        obj.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        Mission mission = new Mission(title, extra, obj, false);
        dic.Add(title, mission);

        OnRefresh();
    }

    public void OnModify(string title, string extra)
    {
        Mission mission;

        if (!dic.TryGetValue(title, out mission))
            return;

        if (mission.IsCleared() == true)
            return;

        mission.SetExtra(extra);

        dic[title] = mission;

        OnRefresh();
    }

    public void OnClear(string title)
    {
        Mission mission;

        if (!dic.TryGetValue(title, out mission))
            return;

        mission.SetExtra("");
        mission.SetCleared(true);

        dic[title] = mission;

        OnRefresh();
    }

    public void OnRemove(string title)
    {
        Mission mission;

        if (!dic.TryGetValue(title, out mission))
            return;

        Destroy(mission.GetObject());

        dic.Remove(title);

        OnRefresh();
    }

    public class Mission
    {
        private string title;
        private string ext;
        private GameObject obj;
        private bool cleared;

        public Mission(string title, string ext = "", GameObject obj = null, bool cleared = false)
        {
            this.title = title;
            this.ext = ext;
            this.obj = obj;
            this.cleared = cleared;
        }

        public string GetTitle()
        {
            return this.title;
        }

        public string GetExtra()
        {
            return this.ext;
        }

        public GameObject GetObject()
        {
            return this.obj;
        }

        public bool IsCleared()
        {
            return this.cleared;
        }

        public void SetTitle(string title)
        {
            this.title = title;
        }

        public void SetExtra(string ext)
        {
            this.ext = ext;
        }

        public void SetObject(GameObject obj)
        {
            this.obj = obj;
        }

        public void SetCleared(bool cleared)
        {
            this.cleared = cleared;
        }
    }
}
