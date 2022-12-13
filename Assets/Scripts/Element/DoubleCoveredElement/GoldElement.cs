using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldElement : DoubleCoveredElement
{
    public GoldType goldType;
    private int buff = 1;// 金币倍数

    protected override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.gold;
    }

    protected override void OnUnCovered()
    {
        // 销毁金币特效
        Transform star = transform.Find("Star");
        if(star != null){
            // 回收特效
            GameManager.Instance.poolManager.StoreGameObject(EffectType.starPartical, star.gameObject);
        }
        // 获得金币
        GetGold();
        base.OnUnCovered();
    }

    private void GetGold(){
        if(GameManager.Instance.isGrass) buff = 2;
        else buff = 1;
        switch(goldType){
            case GoldType.one:
                GameManager.Instance.gold += 30 * buff;
                break;
            case GoldType.two:
                GameManager.Instance.gold += 60 * buff;
                break;
            case GoldType.three:
                GameManager.Instance.gold += 90 * buff;
                break;
            case GoldType.four:
                GameManager.Instance.gold += 120 * buff;
                break;
            case GoldType.five:
                GameManager.Instance.gold += 150 * buff;
                break;
            case GoldType.six:
                GameManager.Instance.gold += 180 * buff;
                break;
            case GoldType.seven:
                GameManager.Instance.gold += 210 * buff;
                break;
        }
        // 更新UI
        MainPanel.Instance.UpdateUI(MainPanel.Instance.goldText.rectTransform);
    }

    public override void ConfirmElement()
    {
        // 避免多次生成闪光特效
        if(transform.Find("Star") != null) return;
        // 生成金币闪闪发光特效
        GameObject star = GameManager.Instance.poolManager.GetGameObject(EffectType.starPartical, transform);
        star.name = "Star";
        star.transform.localPosition = Vector3.zero;
        star.GetComponent<ParticleSystem>().Play();
        LoadSprit(GameManager.Instance.goldSprit[(int)goldType]);
    }
}
