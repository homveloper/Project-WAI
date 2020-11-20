using UnityEngine;

[CreateAssetMenu(fileName = "new ContinuableItem", menuName = "Item/ContinuableItem", order = 0)]
public class ContinuableItem : Item
{
    // 크리스탈 전용 스크립트

    public override void Use(Player playerStat)
    {

    }

    public override void Continue(Player playerStat)
    {
        if (!playerStat.IsAlienObject() && !playerStat.IsDead() && playerStat.GetO2() >= 10)
            playerStat.SetO2(playerStat.GetO2() + (playerStat.GetModO2() / 2 * Time.deltaTime));
    }
}