using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorElement : CantCoveredElement
{
    protected override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.door;
        LoadSprit(GameManager.Instance.doorSprit);
    }

    protected override void OnLeftMouseUP()
    {
        // 距离在1.5以内，可以开门
        if(Vector3.Distance(transform.position, GameManager.Instance.player.transform.position) < 1.5f){
            if(GameManager.Instance.key > 0){
                AudioManager.Instance.PlayDoorClip();
                GameManager.Instance.key--;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.keyIcon.rectTransform, MainPanel.Instance.keyText.rectTransform);
                GameObject doorCloseEffect = Instantiate(GameManager.Instance.doorCloseEffect, transform);
                doorCloseEffect.transform.localPosition = Vector3.zero;
                ToNumberElement();
            }else{
                // 没有钥匙的情况下就移动到门的位置
                base.OnLeftMouseUP();
            }
        }else{
            base.OnLeftMouseUP();
        }
    }
}
