using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CantCoveredElement : BaseElement
{
    protected override void Awake()
    {
        base.Awake();
        elementType = ElementType.cantCovered;
        // 默认是已经翻开的
        elementState = ElementState.uncovered;
    }
}
