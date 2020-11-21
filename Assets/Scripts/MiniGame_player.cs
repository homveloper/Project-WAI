using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame_player : MonoBehaviour
{
    public float moveSpeed = 4;
    private MiniGame_item holdItem;
    private Animator animator;
    public GameObject pmgr;
    public GameObject stage;
    public Vector3 tmp;
    private Vector3 itempos;
    public List<MiniGame_enemy> enemylist;
    public GameObject items;
    public List<MiniGame_item> itemlist;
    void Start()
    {
        tmp = transform.position;
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDir += Vector3.up;
            animator.Play("Hero_Up");
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDir += Vector3.down;
            animator.Play("Hero_Down");
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDir += Vector3.left;
            animator.Play("Hero_Left");
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDir += Vector3.right;
            animator.Play("Hero_Right");
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.GetInstance().GetComponent<FadeController>().OnFadeOut();
            stage.SetActive(false);
            invoke("OnFinishMinigame", 1.0f);
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
           for(int i=0; i< itemlist.Count; i++)
            {
                itemlist[i].gameObject.SetActive(true);
                itemlist[i].GetComponent<BoxCollider2D>().enabled = true;
                
            }
            itemlist.Clear();
            
            
            for (int i = 0; i < enemylist.Count; i++)
            {
                enemylist[i].gameObject.SetActive(true);
                Animator animator = enemylist[i].GetComponent<Animator>();
                animator.SetInteger("EnemyType", (int)enemylist[i].type);
            }
            enemylist.Clear();
            if (holdItem != null)
            {
                holdItem.transform.parent = null;
                holdItem.transform.position = itempos;
                holdItem.GetComponent<BoxCollider2D>().enabled = true;
                holdItem = null;
                itempos = Vector3.zero;
            }

            transform.position = tmp;
        }

        transform.position += moveDir * Time.deltaTime * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var item = collision.gameObject.GetComponent<MiniGame_item>();
        if (item != null)
        {
            ProcessItem(item);
        }

        var enemy = collision.gameObject.GetComponent<MiniGame_enemy>();
        if (enemy != null)
        {
            ProcessEnemy(enemy);
        }
    }

    void ProcessItem(MiniGame_item item)
    {
        if (holdItem == null)
        {
            holdItem = item;
            itempos = holdItem.transform.position;
            holdItem.transform.parent = transform;
            holdItem.transform.localPosition = Vector3.zero;

            holdItem.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    void ProcessEnemy(MiniGame_enemy enemy)
    {
        if (holdItem != null)
        {
            var item = holdItem.GetComponent<MiniGame_item>();

            int itemType = (int)item.type;
            int enemyType = (int)enemy.type;

            if (itemType == enemyType)
            {
                holdItem.transform.parent = items.transform;
                holdItem.transform.position = itempos;
                holdItem.gameObject.SetActive(false);
                itemlist.Add(holdItem);
                holdItem = null;
                itempos = Vector3.zero;

                enemy.gameObject.SetActive(false);
                enemylist.Add(enemy);
                if(itemType == 3) //아이템이 키였고, 상자를 열었을때
                {
                    pmgr.GetComponent<Temple2_Panel_Mgr>().tmp--;
                    GameManager.GetInstance().GetComponent<FadeController>().OnFadeOut();
                    stage.SetActive(false);
                    invoke("OnFinishMinigame", 1.0f);
                    
                }
            }
        }
    }

    void OnFinishMinigame()
    {
        GameManager.GetInstance().GetComponent<FadeController>().OnFadeIn();
        GameManager.GetInstance().mPlayer.GetComponent<Player>().SetMove(true);
    }
}
