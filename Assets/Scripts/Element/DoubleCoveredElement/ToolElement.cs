using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolElement : DoubleCoveredElement
{
    public ToolType toolType;

    protected override void Awake()
    {
        base.Awake();
        elementContent = ElementContent.tool;
    }

    protected override void OnUnCovered()
    {

        // TODO 获得道具
        GetTool();
        // 同样要转变为数字类
        base.OnUnCovered();
    }

    private void GetTool(){
        switch(toolType){
            case ToolType.hp:
            GameManager.Instance.hp++;
            MainPanel.Instance.UpdateUI(MainPanel.Instance.hpText.rectTransform);
                break;
            case ToolType.armor:
            GameManager.Instance.armor++;
            MainPanel.Instance.UpdateUI(MainPanel.Instance.armorIcon.rectTransform, MainPanel.Instance.armorText.rectTransform);
                break;
            case ToolType.sword:
            GameManager.Instance.weaponType = weaponType.sword ;
            GameManager.Instance.arrow = 0;
            MainPanel.Instance.UpdateUI(MainPanel.Instance.swordIcon.rectTransform);
                break;
            case ToolType.map:
            GameManager.Instance.map++;
            MainPanel.Instance.UpdateUI(MainPanel.Instance.mapIcon.rectTransform, MainPanel.Instance.mapText.rectTransform);
                break;
            case ToolType.arrow:
            GameManager.Instance.weaponType = weaponType.arrow;
            GameManager.Instance.arrow++;
            MainPanel.Instance.UpdateUI(MainPanel.Instance.arrowIcon.rectTransform, MainPanel.Instance.weaponText.rectTransform);
                break;
            case ToolType.key:
            GameManager.Instance.key++;
            MainPanel.Instance.UpdateUI(MainPanel.Instance.keyIcon.rectTransform, MainPanel.Instance.keyText.rectTransform);
                break;
            case ToolType.tnt:
            GameManager.Instance.tnt++;
            MainPanel.Instance.UpdateUI(MainPanel.Instance.tntIcon.rectTransform, MainPanel.Instance.tntText.rectTransform);
                break;
            case ToolType.hoe:
                GameManager.Instance.hoe++;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.hoeIcon.rectTransform, MainPanel.Instance.hoeText.rectTransform);
                break;
            case ToolType.grass:
                GameManager.Instance.isGrass = true;
                MainPanel.Instance.UpdateUI(MainPanel.Instance.grassIcon.rectTransform);
                break;
        }
    }
    public override void ConfirmElement()
    {
        LoadSprit(GameManager.Instance.toolSprit[(int)toolType]);
    }
}
