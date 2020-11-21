using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MiniGame_itemEditer : MonoBehaviour
{
    public Sprite holySprite;
    public Sprite maceSprite;
    public Sprite swordSprite;
    public Sprite keySprite;

    private MiniGame_item.Type previousType;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        MiniGame_item item = GetComponent<MiniGame_item>();
        MiniGame_item.Type type = item.type;

        if (type != previousType)
        {
            previousType = type;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            switch (type)
            {
                case MiniGame_item.Type.Holy:
                    sr.sprite = holySprite;
                    break;

                case MiniGame_item.Type.Mace:
                    sr.sprite = maceSprite;
                    break;

                case MiniGame_item.Type.Sword:
                    sr.sprite = swordSprite;
                    break;

                case MiniGame_item.Type.Key:
                    sr.sprite = keySprite;
                    break;
            }
        }
    }
    
}
