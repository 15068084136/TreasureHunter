using System.Collections.Generic;
// 元素状态，分为有覆盖，无覆盖，有标记
public enum ElementState
{
    covered,
    uncovered,
    marked
}

// 元素类型，可以翻一次，可以翻两次，不能翻
public enum ElementType{
    singleCovered,
    doubleCovered,
    cantCovered
}

public enum ElementContent{
    number,// 数字
    trap,// 陷阱
    tool,// 工具
    gold,// 金币
    enemy,
    door,
    bigWall,
    smallWall,
    exit,// 出口
}

// 道具类型
public enum ToolType{
    hp,
    armor,
    sword,
    map,
    arrow,
    key,
    tnt,
    hoe,
    grass,
}

// 金币类型
public enum GoldType{
    one,
    two,
    three,
    four,
    five,
    six,
    seven
}

// 武器类型
public enum weaponType{
    none,
    arrow,
    sword
}

// 特效类型
public enum EffectType{
    smokePartical,// 棋子消失特效
    brickPartical,// 尘土特效
    starPartical,// 闪闪发光特效
}
