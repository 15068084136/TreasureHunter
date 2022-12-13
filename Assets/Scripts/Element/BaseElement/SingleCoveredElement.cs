using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 单翻元素
/// </summary>
public class SingleCoveredElement : BaseElement
{
    protected override void Awake()
    {
        base.Awake();
        elementState = ElementState.covered;
        elementType = ElementType.singleCovered;
        LoadSprit(GameManager.Instance.coveredTileSprit[UnityEngine.Random.Range(0, GameManager.Instance.coveredTileSprit.Length)]);
    }

    #region 点击事件

    protected override void OnLeftMouseUP()
    {
        base.OnLeftMouseUP();

    }

    public override void OnRightMouseUP()
    {
        base.OnRightMouseUP();
        switch (elementState)
        {
            case ElementState.covered:
                AddFlag();
                break;
            case ElementState.uncovered:
                return;
            case ElementState.marked:
                RemoveFlag();
                break;
        }
    }

    #endregion

    public override void PlayerOnStand()
    {
        switch (elementState)
        {
            case ElementState.covered:
                UncoveredElement();
                break;
            case ElementState.uncovered:
                return;
            case ElementState.marked:
                RemoveFlag();
                break;
        }
    }

    // 翻开元素
    public void UncoveredElement(){
        // 翻开过的元素不能再次翻开
        if(!(elementState == ElementState.uncovered)){
            AudioManager.Instance.PlayDigClip();
            AudioManager.Instance.PlayDigClip();
            UncoveredElementSingle();
            OnUnCovered();
        }
    }

    // 翻开单个元素
    public virtual void UncoveredElementSingle(){
        
    }

    // 在翻开后要做什么事情
    protected virtual void OnUnCovered(){

    }

    // 插棋子
    protected void AddFlag(){
        AudioManager.Instance.PlayFlagClip();
        elementState = ElementState.marked;
        GameObject flag = Instantiate(GameManager.Instance.Flag, transform);
        flag.name = "flag";
        // 目标位置，0.1s的持续时间
        flag.transform.DOLocalMoveY(0, 0.1f);

        // 添加烟尘特效
        GameObject smoke = GameManager.Instance.poolManager.GetGameObject(EffectType.smokePartical, transform);
        smoke.GetComponent<ParticleSystem>().Play();
    }

    // 拔棋子
    protected void RemoveFlag(){
        if(transform.Find("flag") != null){
            elementState = ElementState.covered;
            // 在完成的时候删除图标
            transform.Find("flag").DOLocalMoveY(0.15f, 0.1f).OnComplete(()=>{
                Destroy(transform.Find("flag").gameObject);
            });
        }
    }
}
