using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigWallElement : CantCoveredElement
{
    protected override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.bigWall;
        LoadSprit(GameManager.Instance.bigWallSprit[UnityEngine.Random.Range(0, GameManager.Instance.bigWallSprit.Length)]);
    }
}
