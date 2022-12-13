using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectGizoms : MonoBehaviour
{
    public ToolType toolType;

    private void OnMouseOver() {
        int x = (int)transform.position.x;
        int y = (int)transform.position.y;
        if(Input.GetMouseButtonDown(0)){
            switch(toolType){
                case ToolType.hoe:
                    MainPanel.Instance.hoeToggle.isOn = false;
                    GameManager.Instance.hoe--;
                    AudioManager.Instance.PlayHoeClip();
                    MainPanel.Instance.UpdateUI(MainPanel.Instance.hoeIcon.rectTransform, MainPanel.Instance.hoeText.rectTransform);
                    for (int i = x - 1; i <= x + 1; i++)
                    {
                        for (int j = y - 1; j <= y + 1; j++)
                        {
                            if(i >= 0 && i < GameManager.Instance.mapwidth && j >= 0 && j < GameManager.Instance.mapHeight && 
                                GameManager.Instance.elements[i, j].elementContent != ElementContent.exit){
                                if(GameManager.Instance.elements[i, j].elementType != ElementType.cantCovered)
                                ((SingleCoveredElement)(GameManager.Instance.elements[i, j])).UncoveredElementSingle();
                                else{
                                    // 只能锄开小石头
                                    if(GameManager.Instance.elements[i, j].elementContent == ElementContent.smallWall){
                                        GameManager.Instance.elements[i, j].ToNumberElementSingel(true);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case ToolType.tnt:
                    MainPanel.Instance.tntToggle.isOn = false;
                    GameManager.Instance.tnt--;
                    AudioManager.Instance.PlayTntClip();
                    MainPanel.Instance.UpdateUI(MainPanel.Instance.tntIcon.rectTransform, MainPanel.Instance.tntText.rectTransform);
                    for (int i = x - 1; i <= x + 1; i++)
                    {
                        for (int j = y - 1; j <= y + 1; j++)
                        {
                            if(i >= 0 && i < GameManager.Instance.mapwidth && j >= 0 && j < GameManager.Instance.mapHeight && GameManager.Instance.elements[i, j]
                                .elementContent != ElementContent.exit){
                                GameManager.Instance.elements[i, j].ToNumberElementSingel(true);
                            }  
                            
                        }
                    }
                    break;
                case ToolType.map:
                    MainPanel.Instance.mapToggle.isOn = false;
                    GameManager.Instance.map--;
                    AudioManager.Instance.PlayMapClip();
                    MainPanel.Instance.UpdateUI(MainPanel.Instance.mapIcon.rectTransform, MainPanel.Instance.mapText.rectTransform);
                    for (int i = x - 3; i <= x + 3; i++)
                    {
                        for (int j = y - 3; j <= y + 3; j++)
                        {
                            if(i >= 0 && i < GameManager.Instance.mapwidth && j >= 0 && j < GameManager.Instance.mapHeight &&
                                GameManager.Instance.elements[i, j].elementContent != ElementContent.exit){
                                if(GameManager.Instance.elements[i, j].elementContent == ElementContent.trap && GameManager.Instance.elements[i, j]
                                    .elementState != ElementState.marked){
                                        GameManager.Instance.elements[i, j].OnRightMouseUP();
                                    }
                                if(GameManager.Instance.elements[i, j].elementContent != ElementContent.trap && GameManager.Instance.elements[i, j]
                                    .elementState == ElementState.marked){
                                        GameManager.Instance.elements[i, j].OnRightMouseUP();
                                    }
                            }   
                        }
                    }
                    break;
            }
        }
    }
}
