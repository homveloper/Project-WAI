using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame_player : MonoBehaviour
{
    public float moveSpeed = 4;
    private MiniGame_item holdItem;
    private Animator animator;
    void Start()
    {
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
                Destroy(holdItem.gameObject);
                holdItem = null;

                Destroy(enemy.gameObject);
            }
        }
    }
}
