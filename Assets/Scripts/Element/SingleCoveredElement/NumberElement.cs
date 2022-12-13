using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数字元素块
/// </summary>
public class NumberElement : SingleCoveredElement
{
    private int trapAdjcentCount;// 附近有多少颗地雷
    private Vector3Int playerPos;

    protected override void Awake()
    {
        base.Awake();
        // 底下元素内容是数字
        elementContent = ElementContent.number;
    }

    protected override void OnMidMouseUP()
    {
        base.OnMidMouseUP();
        // 快速探查
        if(elementState == ElementState.uncovered){
            bool wrongFlag = false;
            wrongFlag = GameManager.Instance.IsFlagWrong(x, y);
            int flagAdjcentNum = GameManager.Instance.GetAdjcentFlag(x, y);
            playerPos = GameManager.Instance.player.transform.position.ToVector3Int();
            // 如果八领域的棋子数量等于八领域的地雷数量，并且，当前中键点击的位置必须是角色站立的位置
            if(flagAdjcentNum == trapAdjcentCount && x == playerPos.x && y == playerPos.y && !wrongFlag){
                if(!GameManager.Instance.isFindPath){
                    AudioManager.Instance.PlayQuickCheckClip();
                    GameManager.Instance.animator.SetTrigger("quickCheck");
                    GameManager.Instance.QuickSeek(x, y);
                }
            }else{
                AudioManager.Instance.PlayWhyClip();
                GameManager.Instance.animator.SetTrigger("why");
            }
        }
    }

    public override void UncoveredElementSingle()
    {
        base.UncoveredElementSingle();
        // 有可能会单独调用这个函数，比如使用炸药拆除元素
        // 所以需要做安全校验
        if(elementState == ElementState.uncovered) return;
        // 同样，不一定只有鼠标左键点击调用该函数，移除棋子，还有可能是炸弹调用，炸掉这个棋子
        RemoveFlag();
        ClearShadow();
        // 生成尘土
        BrickPlay();

        // 得到该元素附近的地雷数量
        trapAdjcentCount = GameManager.Instance.GetAdjcentTrap(x, y);
        LoadSprit(GameManager.Instance.numberSprite[trapAdjcentCount]);
        elementState = ElementState.uncovered;
    }

    protected override void OnUnCovered()
    {
        base.OnUnCovered();
        // 采用泛红算法，如果当前格子八领域，有地雷，则只有当前格子能翻开
        // 如果当前格子是不能翻开的，则不能翻开
        // 如果当前格子是已经翻开的，则也不能翻开
        // 有部分逻辑，比如用炸弹之类的，就只能炸开一个方块元素，不能引起泛红算法
        bool[,] visited = new bool[GameManager.Instance.mapwidth, GameManager.Instance.mapHeight];
        GameManager.Instance.FloodFillElement(x, y, visited);
    }
}
