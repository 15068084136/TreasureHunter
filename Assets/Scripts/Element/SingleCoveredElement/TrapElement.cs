using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapElement : SingleCoveredElement
{
    protected override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.trap;
    }

    public override void UncoveredElementSingle()
    {
        base.UncoveredElementSingle();
        if(elementState == ElementState.uncovered) return;
        RemoveFlag();
        elementState = ElementState.uncovered;
        ClearShadow();
        // 生成尘土
        BrickPlay();
        // 更换陷阱图片
        LoadSprit(GameManager.Instance.trapSprite[UnityEngine.Random.Range(0, GameManager.Instance.trapSprite.Length)]);
    }

    protected override void OnUnCovered()
    {
        base.OnUnCovered();
        // TODO 扣血或者扣护盾
        GameManager.Instance.TakeDamage();
    }
}
