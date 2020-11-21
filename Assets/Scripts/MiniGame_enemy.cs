using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame_enemy : MonoBehaviour
{
    public enum Type
    {
        Demon,
        Jelly,
        Wolf,
        Chest
    };
    public Type type;
    // Start is called before the first frame update
    void Start()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetInteger("EnemyType", (int)type);
    }

// Update is called once per frame
void Update()
    {
        
    }
}
