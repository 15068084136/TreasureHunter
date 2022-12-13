using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyElement : CantCoveredElement
{
    protected override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.enemy;
        ClearShadow();
        LoadSprit(GameManager.Instance.enemySprit[UnityEngine.Random.Range(0, GameManager.Instance.enemySprit.Length)]);
    }

    protected override void OnLeftMouseUP()
    {
        // 距离在1.5以内，可以开门
        if(Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < 1.5f){
            switch(GameManager.Instance.weaponType){
                case weaponType.none:
                    base.OnLeftMouseUP();
                    break;
                case weaponType.arrow:
                    AudioManager.Instance.PlayEnemyClip();
                    GameManager.Instance.arrow--;
                    if(GameManager.Instance.arrow == 0){
                        GameManager.Instance.weaponType = weaponType.none;
                    }
                    MainPanel.Instance.UpdateUI(MainPanel.Instance.arrowIcon.rectTransform, MainPanel.Instance.weaponText.rectTransform);
                    ToNumberElement();
                    break;
                case weaponType.sword:
                    AudioManager.Instance.PlayEnemyClip();
                    MainPanel.Instance.UpdateUI(MainPanel.Instance.swordIcon.rectTransform);
                    ToNumberElement();
                    break;
            }
        }else{
            base.OnLeftMouseUP();
        }
    }
}
