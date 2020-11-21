using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class MiniGame_enemyEditer : MonoBehaviour
{
    public Sprite demonSprite;
    public Sprite jellySprite;
    public Sprite wolfSprite;
    public Sprite chestSprite;

    private MiniGame_enemy.Type previousType;

    private void Update()
    {
        MiniGame_enemy enemy = GetComponent<MiniGame_enemy>();
        MiniGame_enemy.Type type = enemy.type;

        if (type != previousType)
        {
            previousType = type;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            switch (type)
            {
                case MiniGame_enemy.Type.Demon:
                    sr.sprite = demonSprite;
                    break;

                case MiniGame_enemy.Type.Jelly:
                    sr.sprite = jellySprite;
                    break;

                case MiniGame_enemy.Type.Wolf:
                    sr.sprite = wolfSprite;
                    break;

                case MiniGame_enemy.Type.Chest:
                    sr.sprite = chestSprite;
                    break;
            }
        }
    }

}
