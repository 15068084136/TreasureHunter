using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MainPanel : MonoBehaviour
{
    #region UIDefine

    public Image armorIcon;
    public Image keyIcon;
    public Image arrowBg;
    public Image arrowIcon;
    public Image swordIcon;
    public Image hoeIcon;
    public Image hoeBag;
    public Image tntIcon;
    public Image tntBag;
    public Image mapIcon;
    public Image mapBag;
    public Image grassIcon;
    public Text levelText;
    public Text hpText;
    public Text armorText;
    public Text keyText;
    public Text weaponText;
    public Text hoeText;
    public Text tntText;
    public Text mapText;
    public Text goldText;
    private GameObject mainPanel;
    public Toggle hoeToggle;
    public Toggle mapToggle;
    public Toggle tntToggle;

    #endregion

    public GameObject readPanel;
    public GameObject setPanel;

    private bool isHide = false;

    private static MainPanel instance;
    public static MainPanel Instance{
        get{
            return instance;
        }
    }

    private void Awake() {
        instance = this;
        mainPanel = GameObject.Find("MainPanel");
    }

    private void Start() {
        UpdateUI();
    }

    #region 点击事件

    public void SetMainPanelState(){
        isHide = !isHide;
        if(isHide){
            mainPanel.GetComponent<RectTransform>().DOAnchorPosY(-7, 0.5f);
        }else{
            mainPanel.GetComponent<RectTransform>().DOAnchorPosY(34, 0.5f);
        }
    }

    public void SetHoeSelect(bool isOn){
        GameManager.Instance.hoeSelect.SetActive(isOn);
    }

    public void SetTntSelect(bool isOn){
        GameManager.Instance.tntSelect.SetActive(isOn);
    }

    public void SetMapSelect(bool isOn){
        GameManager.Instance.mapSelect.SetActive(isOn);
    }

    public void OpenReadPanel(){
        readPanel.SetActive(true);
        transform.position = new Vector3(0, 0, -1);
        AudioManager.Instance.PlayBtnClip();
    }

    public void CloseReadPanel(){
        readPanel.SetActive(false);
        transform.position = new Vector3(0, 0, 1);
        AudioManager.Instance.PlayBtnClip();
    }

    public void OpenSetPanel(){
        setPanel.SetActive(true);
        transform.position = new Vector3(0, 0, -1);
        AudioManager.Instance.PlayBtnClip();
    }

    public void CloseSetPanel(){
        setPanel.SetActive(false);
        transform.position = new Vector3(0, 0, 1);
        AudioManager.Instance.PlayBtnClip();
    }

    public void ClearData(){
        DataManager.Instance.ClearData();
        SceneManager.LoadScene(0);
    }

    #endregion

    public void UpdateUI(params RectTransform[] rts){
        levelText.text = "Level " + GameManager.Instance.level;
        hpText.text = GameManager.Instance.hp.ToString();
        if(GameManager.Instance.armor == 0){
            armorIcon.gameObject.SetActive(false);
            armorText.gameObject.SetActive(false);
        }else{
            armorIcon.gameObject.SetActive(true);
            armorText.gameObject.SetActive(true);
            armorText.text = GameManager.Instance.armor.ToString();
        }
        if(GameManager.Instance.key == 0){
            keyIcon.gameObject.SetActive(false);
            keyText.gameObject.SetActive(false);
        }else{
            keyIcon.gameObject.SetActive(true);
            keyText.gameObject.SetActive(true);
            keyText.text = GameManager.Instance.key.ToString();
        }
        switch(GameManager.Instance.weaponType){
            case weaponType.none:
                arrowBg.gameObject.SetActive(true);
                arrowIcon.gameObject.SetActive(false);
                swordIcon.gameObject.SetActive(false);
                weaponText.gameObject.SetActive(false);
                break;
            case weaponType.arrow:
                arrowBg.gameObject.SetActive(false);
                arrowIcon.gameObject.SetActive(true);
                swordIcon.gameObject.SetActive(false);
                weaponText.gameObject.SetActive(true);
                weaponText.text = GameManager.Instance.arrow.ToString();
                break;
            case weaponType.sword:
                arrowBg.gameObject.SetActive(false);
                arrowIcon.gameObject.SetActive(false);
                swordIcon.gameObject.SetActive(true);
                weaponText.gameObject.SetActive(false);
                break;
        }
        if(GameManager.Instance.hoe == 0){
            hoeBag.gameObject.SetActive(false);
            hoeIcon.gameObject.SetActive(false);
            hoeText.gameObject.SetActive(false);
        }else{
            hoeBag.gameObject.SetActive(true);
            hoeIcon.gameObject.SetActive(true);
            hoeText.gameObject.SetActive(true);
            hoeText.text = GameManager.Instance.hoe.ToString();
        }
        if(GameManager.Instance.map == 0){
            mapBag.gameObject.SetActive(false);
            mapIcon.gameObject.SetActive(false);
            mapText.gameObject.SetActive(false);
        }else{
            mapBag.gameObject.SetActive(true);
            mapIcon.gameObject.SetActive(true);
            mapText.gameObject.SetActive(true);
            mapText.text = GameManager.Instance.map.ToString();
        }
        if(GameManager.Instance.tnt == 0){
            tntBag.gameObject.SetActive(false);
            tntIcon.gameObject.SetActive(false);
            tntText.gameObject.SetActive(false);
        }else{
            tntBag.gameObject.SetActive(true);
            tntIcon.gameObject.SetActive(true);
            tntText.gameObject.SetActive(true);
            tntText.text = GameManager.Instance.tnt.ToString();
        }
        grassIcon.gameObject.SetActive(GameManager.Instance.isGrass);
        goldText.text = GameManager.Instance.gold.ToString();
        // 摇晃对应UI
        foreach (RectTransform item in rts)
        {   
            item.DOShakeScale(0.5f).onComplete += ()=>{
                item.localScale = Vector3.one;
            };
        }
    }
}
