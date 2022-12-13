using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallElement : CantCoveredElement
{
    protected override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.smallWall;
        // 小石头是没有阴影的
        ClearShadow();
        LoadSprit(GameManager.Instance.smallWallSprit[UnityEngine.Random.Range(0, GameManager.Instance.smallWallSprit.Length)]);
    }
}
