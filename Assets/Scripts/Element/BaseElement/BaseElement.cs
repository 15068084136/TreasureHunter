using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseElement : MonoBehaviour
{
    #region 成员变量

    // 坐标
    protected int x;
    protected int y;

    // 元素类型
    public ElementState elementState;
    public ElementType elementType;
    public ElementContent elementContent;

    #endregion

    protected virtual void Awake(){
        // 并没有丢失精度，因为原本就是这么根据整形设定的
        x = (int)transform.position.x;
        y = (int)transform.position.y;
        // 修改物体名称为该物体的坐标
        gameObject.name = "(" + x + "," + y + ")";
    }

    #region 点击事件

    /// <summary>
    /// 在鼠标在有碰撞体的物体上时，调用该方法，该方法生命周期类似于Update
    /// </summary>
    protected virtual void OnMouseOver() {
        // 采用UP的目的是当玩家误按了鼠标左键时，只要将鼠标移出碰撞体就可以避免误按
        // 采用else if的目的是避免同时按了左右键，导致两种点击事件都产生
        if(Input.GetMouseButtonUp(2) && elementState == ElementState.uncovered){
            OnMidMouseUP();
        }else if(Input.GetMouseButtonUp(0)){
            OnLeftMouseUP();
        }else if(Input.GetMouseButtonUp(1)){
            OnRightMouseUP();
        } 
    }

    protected virtual void OnLeftMouseUP(){
        GameManager.Instance.FindPath(new Point(x, y));
    }
    public virtual void OnRightMouseUP(){}
    protected virtual void OnMidMouseUP(){}

    //  玩家站立在元素方块上时会做的反应
    public virtual void PlayerOnStand(){

    }

    #endregion

    #region 特效与图片

    // 清除阴影
    protected void ClearShadow(){
        GameObject shadow = transform.GetChild(0).gameObject;
        if(shadow != null)
            shadow.SetActive(false);
    }

    // 生成尘土
    protected void BrickPlay(){
        GameObject brick = GameManager.Instance.poolManager.GetGameObject(EffectType.brickPartical, transform);
        if(brick != null){
            brick.name = "Brick";
            brick.transform.localPosition = Vector3.zero;
            brick.GetComponent<ParticleSystem>().Play();
        }
    }

    protected void LoadSprit(Sprite sprite){
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    #endregion

    #region 转为数字类

    // 转化为单翻数字类
    public void ToNumberElement(bool isNeedBrick = false){
        GameManager.Instance.elements[x, y] = gameObject.AddComponent<NumberElement>();
        // 完全翻开数字元素
        ((NumberElement)GameManager.Instance.elements[x, y]).UncoveredElement();
        if(transform.Find("Brick") != null && !isNeedBrick){
            transform.Find("Brick").gameObject.SetActive(false);
        }
        Destroy(this);
    }

    public void ToNumberElementSingel(bool isNeedBrick = false){
        GameManager.Instance.elements[x, y] = gameObject.AddComponent<NumberElement>();
        // 完全翻开数字元素
        ((NumberElement)GameManager.Instance.elements[x, y]).UncoveredElementSingle();
        if(transform.Find("Brick").gameObject != null && !isNeedBrick){
            transform.Find("Brick").gameObject.SetActive(false);
        }
        Destroy(this);
    }

    #endregion
}
