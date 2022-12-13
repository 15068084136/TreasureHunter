using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleCoveredElement : SingleCoveredElement
{
    private int trapAdjcentCount;// 附近有多少颗地雷
    public bool hide = true;// 默认是隐藏状态

    protected override void Awake()
    {
        base.Awake();
        elementType = ElementType.doubleCovered;
        // 得到八领域地雷的数量
        trapAdjcentCount = GameManager.Instance.GetAdjcentTrap(x, y);
        // 开局直接暴露
        if(UnityEngine.Random.value < GameManager.Instance.uncoveredChance){
            // 翻开自己
            UncoveredElementSingle();
        }
    }

    #region 点击事件

    public override void OnRightMouseUP()
    {
        switch (elementState)
        {
            case ElementState.covered:
                if(hide)
                    AddFlag();
                break;
            case ElementState.uncovered:
                return;
            case ElementState.marked:
                if(hide)
                    RemoveFlag();
                break;
        }
    }

    protected override void OnMidMouseUP()
    {
        base.OnMidMouseUP();
        // 快速探查
        if(elementState == ElementState.uncovered){
            int flagAdjcentNum = GameManager.Instance.GetAdjcentFlag(x, y);
            // 如果八领域的棋子数量等于八领域的地雷数量
            if(flagAdjcentNum == trapAdjcentCount){
                GameManager.Instance.QuickSeek(x, y);
            }
        }
    }

    public override void PlayerOnStand()
    {
        switch (elementState)
        {
            case ElementState.covered:
                if(!hide){
                    OnUnCovered();
                }else{
                    // 翻开泥土，且只翻开这个泥土
                    UncoveredElementSingle();
                }
                break;
            case ElementState.uncovered:
                return;
            case ElementState.marked:
                if(hide)
                    RemoveFlag();
                break;
        }
    }

    public override void UncoveredElementSingle()
    {
        if(elementState == ElementState.uncovered) return;
        RemoveFlag();
        hide = false;
        ClearShadow();
        // 显示金币或者道具的图片
        ConfirmElement();
    }

    protected override void OnUnCovered()
    {
        AudioManager.Instance.PlayPickClip();
        elementState = ElementState.uncovered;
        // 变成数字元素，并完全翻开
        ToNumberElement();
    }

    #endregion

    public virtual void ConfirmElement(){
        
    }

}
