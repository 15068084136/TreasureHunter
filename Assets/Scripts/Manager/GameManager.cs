using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance{
        get{
            return instance;
        }
    }
    public PoolManager poolManager;

    #region 成员变量

    [Header("角色")]
    public GameObject player;
    [HideInInspector]
    public Animator animator;
    private Vector3Int moveDir;

    [Header("UI界面")]
    public GameObject endPanel;
    public GameObject lossPanel;

    // 生成地图
    [Header("元素预制体")]
    public GameObject bgElement;
    [Tooltip("边界预制体，顺序为：\n上下左右\n左上,右上,左下,右下")]
    public GameObject[] borderElements;
    public GameObject baseElement;// 基础方块
    public GameObject Flag;
    public GameObject error;
    [Header("地图设置")]
    public int mapwidth;
    public int mapHeight;
    private Transform mapBackgroundTrans;// 放元素的地方
    private Transform elementTrans;// 放基础方块的地方
    public BaseElement[,] elements;// 基础方块元素二维数组
    private float minTrapChance;// 最小雷区概率
    private float maxTrapChance;// 最大雷区概率
    public float uncoveredChance;// DoubleElement开局就翻开的概率
    private int standAreaWidth;// 站立区的宽度
    private int standAreaHeight;// 站立区的高度
    private int obstacleAreaWidth;// 障碍物区的宽度
    private int obstacleAreaNum;
    private bool isOver = false;
    [HideInInspector]
    public Vector3Int prePos;// 前一帧所在的位置
    [HideInInspector]
    public Vector3Int curPos;// 当前帧所在的位置
    private Tweener tweener;
    [HideInInspector]
    public bool isFindPath = false;// 是否正在寻路

    [Header("游戏数据")]
    public int level;
    public int hp;
    public int armor;
    public int key;
    public weaponType weaponType;
    public int arrow;
    public int hoe;
    public int tnt;
    public int map;
    public bool isGrass;
    public int gold;

    // 图片
    [Header("图片")]
    public Sprite[] coveredTileSprit;// 覆盖瓦片图片
    public Sprite[] trapSprite;// 陷阱图片
    public Sprite[] numberSprite;// 数字图片
    public Sprite[] toolSprit;// 道具图片
    public Sprite[] goldSprit;// 金币图片
    public Sprite[] bigWallSprit;// 大墙图片
    public Sprite[] smallWallSprit;// 小墙图片
    public Sprite[] enemySprit;// 敌人图片
    public Sprite doorSprit;// 门图片
    public Sprite exitSprit;// 出口图片

    // 特效
    [Header("特效")]
    public ParticleSystem smokePartical;// 棋子消失特效
    public ParticleSystem brickPartical;// 尘土特效
    public ParticleSystem starPartical;// 闪闪发光特效
    public GameObject doorCloseEffect;
    public GameObject hoeSelect;
    public GameObject tntSelect;
    public GameObject mapSelect;

    #endregion

    private void Awake() {
        instance = this;
        poolManager.Init();
        MapInit();
        CameraInit();
    }

    private void Update(){
        // 按空格键能够回到原始视角
        if(Input.GetKeyDown(KeyCode.Space)){
            ResetPoint();
        }
        curPos = player.transform.position.ToVector3Int();
        if(prePos != curPos){
            moveDir = new Vector3Int(Mathf.Clamp(curPos.x - prePos.x, -1, 1), Mathf.Clamp(curPos.y - prePos.y, -1, 1));
            animator.SetFloat("dirX", moveDir.x);
            animator.SetFloat("dirY", moveDir.y);
            elements[curPos.x, curPos.y].PlayerOnStand();
            // 做两次的目的是可以直接拿到双翻元素
            elements[curPos.x, curPos.y].PlayerOnStand();
            // 踩到地雷，停止移动
            if(elements[curPos.x, curPos.y].elementContent == ElementContent.trap){
                tweener.Kill();
                curPos = prePos;
                player.transform.position = curPos;
            }else{
                prePos = curPos;
            }
        }
    }

    #region 地图创建

    private void MapInit(){
        LoadData();
        player = GameObject.Find("Player").gameObject;
        animator = player.GetComponent<Animator>();
        mapBackgroundTrans = GameObject.Find("Map/Background").transform;
        elementTrans = GameObject.Find("Map/Element").transform;
        elements = new BaseElement[mapwidth, mapHeight];// 初始化二维数组
        minTrapChance = 0.11f;
        maxTrapChance = 0.13f;
        uncoveredChance = 0.1f;
        standAreaWidth = 3;
        standAreaHeight = UnityEngine.Random.Range(1, mapHeight - 1);
        obstacleAreaWidth = 8;
        // 站立区和出口区各占三列
        obstacleAreaNum = (mapwidth - (standAreaWidth + 3))/obstacleAreaWidth;
        CreatMap();

        List<int> indexList = new List<int>();
        for (int i = 0; i < mapwidth * mapHeight; i++)
        {
            indexList.Add(i);
        }
        // 初始化出口
        InitExitElement(indexList);
        // 生成障碍物
        InitObstacleElement(indexList);
        // 初始化地雷(在站立区不会生成地雷)
        InitTrapElement(standAreaHeight, indexList);
        
        // 初始化金币
        InitGoldElement(indexList);
        // 初始化道具
        InitToolElement(indexList);
        // 最后初始化数字
        InitNumberElement(indexList);
        // 生成站立区
        InitStandArea();
    }

    private void CreatMap(){
        // 背景元素铺垫，基础方块
        for (int i = 0; i < mapwidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                GameObject.Instantiate(bgElement, new Vector3(i, j, 0), Quaternion.identity, mapBackgroundTrans);
                // 创建可操作的元素并放置到地图元素数组中
                elements[i, j] = GameObject.Instantiate(baseElement, new Vector3(i, j, 0), Quaternion.identity, elementTrans).GetComponent<BaseElement>();
            }
        }
        // 上边界预制体铺垫
        for (int i = 0; i < mapwidth; i++)
        {
            GameObject.Instantiate(borderElements[0], new Vector3(i, mapHeight + 0.25f), Quaternion.identity, mapBackgroundTrans);
        }
        // 下边界预制体铺垫
        for (int i = 0; i < mapwidth; i++)
        {
            GameObject.Instantiate(borderElements[1], new Vector3(i, -1.25f), Quaternion.identity, mapBackgroundTrans);
        }
        // 左边界预制体铺垫
        for (int i = 0; i < mapHeight; i++)
        {
            GameObject.Instantiate(borderElements[2], new Vector3(-1.25f, i), Quaternion.identity, mapBackgroundTrans);
        }
        // 右边界预制体铺垫
        for (int i = 0; i < mapHeight; i++)
        {
            GameObject.Instantiate(borderElements[3], new Vector3(mapwidth + 0.25f, i), Quaternion.identity, mapBackgroundTrans);
        }
        // 生成左上边界预制体
        GameObject.Instantiate(borderElements[4], new Vector3(-1.25f, mapHeight + 0.25f), Quaternion.identity, mapBackgroundTrans);
        // 生成右上边界预制体
        GameObject.Instantiate(borderElements[5], new Vector3(mapwidth + 0.25f, mapHeight + 0.25f), Quaternion.identity, mapBackgroundTrans);
        // 生成左下边界预制体
        GameObject.Instantiate(borderElements[6], new Vector3(-1.25f, -1.25f), Quaternion.identity, mapBackgroundTrans);
        // 生成右下边界预制体
        GameObject.Instantiate(borderElements[7], new Vector3(mapwidth + 0.25f, -1.25f), Quaternion.identity, mapBackgroundTrans);
    }

    private void InitExitElement(List<int> indexList){
        float x = mapwidth - 1.5f;
        float y = UnityEngine.Random.Range(1, mapHeight) - 0.5f;
        BaseElement baseElement = InitChangeElement(GetSingleArr((int)(x + 0.5f), (int)(y - 0.5f)), ElementContent.exit);
        // 改变出口的位置，放在四个格子的中间
        baseElement.transform.position = new Vector3(x, y, 0);
        // 删除原先出口格子的碰撞体
        Destroy(baseElement.GetComponent<BoxCollider2D>());
        // 生成新的碰撞体
        baseElement.gameObject.AddComponent<BoxCollider2D>();
        // 删除四个格子在可用元素列表中的位置
        indexList.Remove(GetSingleArr((int)(x + 0.5f), (int)(y - 0.5f)));
        indexList.Remove(GetSingleArr((int)(x - 0.5f), (int)(y - 0.5f)));
        indexList.Remove(GetSingleArr((int)(x + 0.5f), (int)(y + 0.5f)));
        indexList.Remove(GetSingleArr((int)(x - 0.5f), (int)(y + 0.5f)));
        // 删除另外三个的元素物体
        Destroy(elements[(int)(x - 0.5f), (int)(y - 0.5f)].gameObject);
        Destroy(elements[(int)(x + 0.5f), (int)(y + 0.5f)].gameObject);
        Destroy(elements[(int)(x - 0.5f), (int)(y + 0.5f)].gameObject);
        // 将四个位置均指向出口元素
        elements[(int)(x - 0.5f), (int)(y - 0.5f)] = baseElement;
        elements[(int)(x + 0.5f), (int)(y + 0.5f)] = baseElement;
        elements[(int)(x - 0.5f), (int)(y + 0.5f)] = baseElement;
        elements[(int)(x + 0.5f), (int)(y - 0.5f)] = baseElement;
    }

    #region 障碍物生成

    private void InitObstacleElement(List<int> indexList){
        for (int i = 0; i < obstacleAreaNum; i++)
        {
            if(UnityEngine.Random.value < 0.5f){
                // 百分之五十概率生成闭环障碍物
                InitCloseObstacleElement(i, indexList);
            }else{
                // 百分之五十概率生成随机障碍物
                InitRandomObstacleElement(i, indexList);
            }
        }
    }

    // i：指的是第几片障碍区
    private void InitCloseObstacleElement(int i, List<int> indexList){
        // 0:与边界闭合，1：自闭合
        int shape = UnityEngine.Random.Range(0, 2);
        CloseAreaInfo info = new CloseAreaInfo();
        switch (shape)
        {
            case 0:
                GenerateCloseAreaInfo(0, i, ref info);
                int dir = UnityEngine.Random.Range(0, 4);// 四种障碍方向
                switch (dir)
                {
                    // U型闭环障碍物，与上面墙相连
                    case 0:
                        // 找到门的位置，两种情况，一种是生成在竖杠上，一种是生成在横杠上
                        info.doorPos = UnityEngine.Random.value < 0.5f?new Vector2(UnityEngine.Random.Range(info.startX + 1, info.endX), info.y)
                            // 生成在竖杠上，门X轴的位置要么在前要么在后
                            :new Vector2(UnityEngine.Random.value < 0.5f?info.startX:info.endX, UnityEngine.Random.Range(info.y + 1, mapHeight));
                        InitDoor(info, indexList);
                        // 生成第一条竖杠
                        for (int j = mapHeight - 1; j > info.y; j--)
                        {
                            // 第一条竖杠是否存在门
                            if(indexList.Contains(GetSingleArr(info.startX, j))){
                                indexList.Remove(GetSingleArr(info.startX, j));
                                InitChangeElement(GetSingleArr(info.startX, j), ElementContent.bigWall);
                            }
                        }
                        // 生成第二条横杠
                        for (int j = info.startX; j < info.endX; j++)
                        {
                            if(indexList.Contains(GetSingleArr(j, info.y))){
                                indexList.Remove(GetSingleArr(j, info.y));
                                InitChangeElement(GetSingleArr(j, info.y), ElementContent.bigWall);
                            }
                        }
                        // 生成第三条竖杠
                        for (int j = mapHeight - 1; j >= info.y; j--)
                        {
                            if(indexList.Contains(GetSingleArr(info.endX, j))){
                                indexList.Remove(GetSingleArr(info.endX, j));
                                InitChangeElement(GetSingleArr(info.endX, j), ElementContent.bigWall);
                            }
                        }
                        info.startY = info.y;
                        info.endY = mapHeight - 1;
                        info.y = mapHeight - info.y - 1;
                        InitReward(info, indexList);
                        break;
                    // n型闭环障碍物，与下面墙相连
                    case 1:
                        // 找到门的位置，两种情况，一种是生成在竖杠上，一种是生成在横杠上
                        info.doorPos = UnityEngine.Random.value < 0.5f?new Vector2(UnityEngine.Random.Range(info.startX + 1, info.endX), info.y)
                            // 生成在竖杠上，门X轴的位置要么在前要么在后
                            :new Vector2(UnityEngine.Random.value < 0.5f?info.startX:info.endX, UnityEngine.Random.Range(0, info.y - 1));
                        InitDoor(info, indexList);
                        // 生成第一条竖杠
                        for (int j = info.y - 1; j >= 0; j--)
                        {
                            // 第一条竖杠是否存在门
                            if(indexList.Contains(GetSingleArr(info.startX, j))){
                                indexList.Remove(GetSingleArr(info.startX, j));
                                InitChangeElement(GetSingleArr(info.startX, j), ElementContent.bigWall);
                            }
                        }
                        // 生成第二条横杠
                        for (int j = info.startX; j < info.endX; j++)
                        {
                            if(indexList.Contains(GetSingleArr(j, info.y))){
                                indexList.Remove(GetSingleArr(j, info.y));
                                InitChangeElement(GetSingleArr(j, info.y), ElementContent.bigWall);
                            }
                        }
                        // 生成第三条竖杠
                        for (int j = info.y; j >= 0; j--)
                        {
                            if(indexList.Contains(GetSingleArr(info.endX, j))){
                                indexList.Remove(GetSingleArr(info.endX, j));
                                InitChangeElement(GetSingleArr(info.endX, j), ElementContent.bigWall);
                            }
                        }
                        info.startY = -1;
                        info.endY = info.y;
                        InitReward(info, indexList);
                        break;
                    // 上下两面墙接触，向右扭
                    case 2:
                        // 找到门的位置，两种情况，一种是生成在竖杠上，一种是生成在横杠上
                        info.doorPos = UnityEngine.Random.value < 0.5f?new Vector2(UnityEngine.Random.Range(info.startX + 1, info.endX), info.y)
                            // 生成在竖杠上，且只可能生成在前面竖杠上
                            :new Vector2(info.startX, UnityEngine.Random.Range(info.y + 1, mapHeight));
                        InitDoor(info, indexList);
                        // 生成第一条竖杠
                        for (int j = mapHeight - 1; j > info.y; j--)
                        {
                            // 第一条竖杠是否存在门
                            if(indexList.Contains(GetSingleArr(info.startX, j))){
                                indexList.Remove(GetSingleArr(info.startX, j));
                                InitChangeElement(GetSingleArr(info.startX, j), ElementContent.bigWall);
                            }
                        }
                        // 生成第二条横杠
                        for (int j = info.startX; j < info.endX; j++)
                        {
                            if(indexList.Contains(GetSingleArr(j, info.y))){
                                indexList.Remove(GetSingleArr(j, info.y));
                                InitChangeElement(GetSingleArr(j, info.y), ElementContent.bigWall);
                            }
                        }
                        // 生成第三条竖杠
                        for (int j = info.y; j >= 0; j--)
                        {
                            if(indexList.Contains(GetSingleArr(info.endX, j))){
                                indexList.Remove(GetSingleArr(info.endX, j));
                                InitChangeElement(GetSingleArr(info.endX, j), ElementContent.bigWall);
                            }
                        }
                        break;
                    // 上下两面墙接触，向左扭
                    case 3:
                        // 找到门的位置，两种情况，一种是生成在竖杠上，一种是生成在横杠上
                        info.doorPos = UnityEngine.Random.value < 0.5f?new Vector2(UnityEngine.Random.Range(info.startX + 1, info.endX), info.y)
                            // 生成在竖杠上，且门只可能在前面
                            :new Vector2(info.startX, UnityEngine.Random.Range(0, info.y - 1));
                        InitDoor(info, indexList);
                        // 生成第一条竖杠
                        for (int j = info.y - 1; j >= 0; j--)
                        {
                            // 第一条竖杠是否存在门
                            if(indexList.Contains(GetSingleArr(info.startX, j))){
                                indexList.Remove(GetSingleArr(info.startX, j));
                                InitChangeElement(GetSingleArr(info.startX, j), ElementContent.bigWall);
                            }
                        }
                        // 生成第二条横杠
                        for (int j = info.startX; j < info.endX; j++)
                        {
                            if(indexList.Contains(GetSingleArr(j, info.y))){
                                indexList.Remove(GetSingleArr(j, info.y));
                                InitChangeElement(GetSingleArr(j, info.y), ElementContent.bigWall);
                            }
                        }
                        // 生成第三条竖杠
                        for (int j = info.y; j < mapHeight; j++)
                        {
                            if(indexList.Contains(GetSingleArr(info.endX, j))){
                                indexList.Remove(GetSingleArr(info.endX, j));
                                InitChangeElement(GetSingleArr(info.endX, j), ElementContent.bigWall);
                            }
                        }
                        break;
                }
                InitTool(info, indexList);
                break;
            case 1:
                GenerateCloseAreaInfo(1, i, ref info);
                // 生成第一条竖杠
                for (int j = info.startY + 1; j <= info.endY; j++)
                {
                    // 第一条竖杠是否存在门
                    if(indexList.Contains(GetSingleArr(info.startX, j))){
                        indexList.Remove(GetSingleArr(info.startX, j));
                        InitChangeElement(GetSingleArr(info.startX, j), ElementContent.bigWall);
                    }
                }
                // 生成第二条横杠
                for (int j = info.startX + 1; j <= info.endX; j++)
                {
                    if(indexList.Contains(GetSingleArr(j, info.endY))){
                        indexList.Remove(GetSingleArr(j, info.endY));
                        InitChangeElement(GetSingleArr(j, info.endY), ElementContent.bigWall);
                    }
                }
                // 生成第三条竖杠
                for (int j = info.endY - 1; j >= info.startY; j--)
                {
                    if(indexList.Contains(GetSingleArr(info.endX, j))){
                        indexList.Remove(GetSingleArr(info.endX, j));
                        InitChangeElement(GetSingleArr(info.endX, j), ElementContent.bigWall);
                    }
                }
                // 生成第四条横杠
                for (int j = info.startX; j < info.endX; j++)
                {
                    if(indexList.Contains(GetSingleArr(j, info.startY))){
                        indexList.Remove(GetSingleArr(j, info.startY));
                        InitChangeElement(GetSingleArr(j, info.startY), ElementContent.bigWall);
                    }
                }
                InitTool(info, indexList);
                info.x++;
                InitReward(info, indexList);
                break;
        }
    }

    private void InitRandomObstacleElement(int i, List<int> indexList){
        for (int j = 0; j < 5; j++)
        {
            // 生成范围
            int startX = standAreaWidth + i * obstacleAreaWidth + 1;
            int endX = startX + obstacleAreaWidth;
            int wallPosX = UnityEngine.Random.Range(startX, endX);
            int wallPosY = UnityEngine.Random.Range(0, mapHeight);
            // 确定墙体位置存在
            while(!indexList.Contains(GetSingleArr(wallPosX, wallPosY))){
                wallPosX = UnityEngine.Random.Range(startX, endX);
                wallPosY = UnityEngine.Random.Range(0, mapHeight);
            }
            indexList.Remove(GetSingleArr(wallPosX, wallPosY));
            InitChangeElement(GetSingleArr(wallPosX, wallPosY), UnityEngine.Random.value < 0.5f?ElementContent.smallWall:ElementContent.bigWall);
        }

    }

    // 生成开启门的道具，且在startX之前生成
    private void InitTool(CloseAreaInfo info, List<int> indexList){
        info.toolX = UnityEngine.Random.Range(0, info.startX);
        info.toolY = UnityEngine.Random.Range(0, mapHeight);
        // 若这一个格子已经生成了其他物品，则重新挑选位置
        while(!indexList.Contains(GetSingleArr(info.toolX, info.toolY))){
            info.toolX = UnityEngine.Random.Range(0, info.startX);
            info.toolY = UnityEngine.Random.Range(0, mapHeight);
        }
        indexList.Remove(GetSingleArr(info.toolX, info.toolY));
        info.toolElement = (ToolElement)InitChangeElement(GetSingleArr(info.toolX, info.toolY), ElementContent.tool);
        info.toolElement.toolType = (ToolType)info.doorType;
        if(info.toolElement.hide){
            info.toolElement.ConfirmElement();
        }
    }

    private void InitDoor(CloseAreaInfo info, List<int> indexList){
        int doorIndex = GetSingleArr((int)info.doorPos.x, (int)info.doorPos.y);
        indexList.Remove(doorIndex);
        InitChangeElement(doorIndex, (ElementContent)info.doorType);
    }

    private void InitReward(CloseAreaInfo info, List<int> indexList){
        info.innerCount = (info.x - 1) * info.y;
        info.goldNum = UnityEngine.Random.Range(1, UnityEngine.Random.value < 0.5f?info.innerCount + 1:info.innerCount / 2);
        for (int i = 0; i < info.goldNum; i++)
        {
            info.goldY = i / (info.x - 1);
            info.goldX = i - info.goldY * (info.x - 1);
            info.goldX += info.startX + 1;
            info.goldY += info.startY + 1;
            if(indexList.Contains(GetSingleArr(info.goldX, info.goldY))){
                indexList.Remove(GetSingleArr(info.goldX, info.goldY));
                info.goldElement = (GoldElement)InitChangeElement(GetSingleArr(info.goldX, info.goldY), ElementContent.gold);
                info.goldElement.goldType = (GoldType)UnityEngine.Random.Range(0, 7);
                if(info.goldElement.hide){
                    info.goldElement.ConfirmElement();
                }
            }
        }
    }

    // nowArea:闭合区域索引值
    // info:要生成的闭合区域信息结构体
    private void GenerateCloseAreaInfo(int type, int nowArea, ref CloseAreaInfo info){
        switch (type)
        {
            // 边闭合
            case 0:
                info.x = UnityEngine.Random.Range(3, obstacleAreaWidth - 2);
                // mapHeight - 3的目的是让上下空间足够，避免出现只存在一条路线的情况
                info.y = UnityEngine.Random.Range(3, mapHeight - 3);
                info.startX = standAreaWidth + nowArea * obstacleAreaWidth + 1;
                info.endX = info.startX + info.x;
                // 门的类型
                info.doorType = UnityEngine.Random.Range(4, 8);
                break;
            // 自闭合
            case 1:
                info.x = UnityEngine.Random.Range(3, obstacleAreaWidth - 2);
                // 高度不超过宽度
                info.y = UnityEngine.Random.Range(3, info.x + 1);
                info.startX = standAreaWidth + nowArea * obstacleAreaWidth + 1;
                info.endX = info.startX + info.x;
                info.startY = UnityEngine.Random.Range(3, mapHeight - info.y - 1);
                info.endY = info.startY + info.y;
                // 门类型定位大墙
                info.doorType = (int)ElementContent.bigWall;
                break;
        }
    }   

    // 闭合障碍物区域结构体
    private struct CloseAreaInfo{
        // x,y分别表示障碍物中间横杠的宽和高
        public int x, y, startX, endX, startY, endY;
        public int doorType;// 门的类型（0：与边界闭合 1：自闭合）
        public Vector2 doorPos;// 门的位置
        public int toolX, toolY;// 道具的位置
        public ToolElement toolElement;// 道具的类型
        public int goldX, goldY;// 金币的位置
        public GoldElement goldElement;// 金币的类型
        public int innerCount, goldNum;// 闭合区域内有空间大小，有几个金币财宝 
    }

    #endregion

    private void InitTrapElement(int standAreaHeight, List<int> indexList){
        // 获得地雷概率
        float trapChance = UnityEngine.Random.Range(minTrapChance, maxTrapChance);
        // 获得地雷数量
        int trapNum = (int)(indexList.Count * trapChance);
        for (int i = 0; i < trapNum; i++)
        {
            // 随机获得二维数组的一维变量
            int tempSingleArr = indexList[UnityEngine.Random.Range(0, indexList.Count)];
            int x, y;
            GetDoubleArr(tempSingleArr, out x, out y);
            // 在站立区不能够生成地雷
            if(x>=0 && x < standAreaWidth && y >= standAreaHeight - 1 && y <= standAreaHeight + 1) continue;
            // 在列表中移除该内容
            indexList.Remove(tempSingleArr);
            // 设置地雷元素
            InitChangeElement(tempSingleArr, ElementContent.trap);

        }
    }

    // 初始化金币
    private void InitGoldElement(List<int> indexList){
        for (int i = 0; i < obstacleAreaNum * 3; i++)
        {
            // 随机获得二维数组的一维变量
            int tempSingleArr = indexList[UnityEngine.Random.Range(0, indexList.Count)];
            int x, y;
            GetDoubleArr(tempSingleArr, out x, out y);
            // 在玩家初始生成的位置不要生成金币
            if(x == curPos.x && y == curPos.y) return;
            // 在列表中移除该内容
            indexList.Remove(tempSingleArr);
            // 设置金币元素
            GoldElement goldElement = (GoldElement)InitChangeElement(tempSingleArr, ElementContent.gold);
            goldElement.goldType = (GoldType)UnityEngine.Random.Range(0, 7);
            if(goldElement.hide){
                goldElement.ConfirmElement();
            }
        }
    }

    // 初始化工具
    private void InitToolElement(List<int> indexList){
        for (int i = 0; i < 3; i++)
        {
            // 随机获得二维数组的一维变量
            int tempSingleArr = indexList[UnityEngine.Random.Range(0, indexList.Count)];
            int x, y;
            GetDoubleArr(tempSingleArr, out x, out y);
            // 在玩家初始生成的位置不要生成道具
            if(x == curPos.x && y == curPos.y) return;
            // 在列表中移除该内容
            indexList.Remove(tempSingleArr);
            // 设置道具元素
            ToolElement toolElement = (ToolElement)InitChangeElement(tempSingleArr, ElementContent.tool);
            toolElement.toolType = (ToolType)UnityEngine.Random.Range(0, 9);
            if(toolElement.hide){
                toolElement.ConfirmElement();
            }
        }
    }

    private void InitNumberElement(List<int> indexList){
        // 剩下的元素，全部赋值为数字
        foreach (int item in indexList)
        {
            // 设置数字元素
            InitChangeElement(item, ElementContent.number);
        }
        // 清空列表
        indexList.Clear();
    }

    // 生成站立区
    private void InitStandArea(){
        for (int i = 0; i < standAreaWidth; i++)
        {
            for (int j = standAreaHeight - 1; j <= standAreaHeight + 1; j++)
            {
                // 直接将站立区的单翻、双翻元素翻开，注意，无法翻开的元素本身就不会生成在站立区
                ((SingleCoveredElement)elements[i, j]).UncoveredElementSingle();
            }
        }
        // 将角色放置在站立区中间
        player.transform.position = new Vector2(standAreaWidth / 2, standAreaHeight);
        // 位置赋值
        curPos = prePos = player.transform.position.ToVector3Int();
        // 翻开当前位置元素，万一角色生成位置就有道具或者金币
    }

    // 在地图初始化时，改变基础元素脚本，变为特定元素脚本
    private BaseElement InitChangeElement(int tempSingleArr, ElementContent elementContent){
        int x,y;
        GetDoubleArr(tempSingleArr, out x, out y);
        // 生成地雷，摧毁基本元素脚本
        GameObject tempElement = elements[x, y].gameObject;
        Destroy(tempElement.GetComponent<BaseElement>());
        // 添加特定元素脚本脚本
        switch (elementContent)
        {
            case ElementContent.trap:
                elements[x, y] = tempElement.AddComponent<TrapElement>();
                break;
            case ElementContent.number:
                elements[x, y] = tempElement.AddComponent<NumberElement>();
                break;
            case ElementContent.gold:
                elements[x, y] = tempElement.AddComponent<GoldElement>();
                break;
            case ElementContent.tool:
                elements[x, y] = tempElement.AddComponent<ToolElement>();
                break;
            case ElementContent.door:
                elements[x, y] = tempElement.AddComponent<DoorElement>();
                break;
            case ElementContent.enemy:
                elements[x, y] = tempElement.AddComponent<EnemyElement>();
                break;
            case ElementContent.bigWall:
                elements[x, y] = tempElement.AddComponent<BigWallElement>();
                break;
            case ElementContent.smallWall:
                elements[x, y] = tempElement.AddComponent<SmallElement>();
                break;
            case ElementContent.exit:
                elements[x, y] = tempElement.AddComponent<ExitElement>();
                break;
        }
        return elements[x, y];
    }

    #endregion

    #region 寻路

    public void FindPath(Point endPoint){
        // 如果正在寻路，再点击可以打断
        if(isFindPath) tweener.Kill();
        Point startPoint = new Point((int)player.transform.position.x, (int)player.transform.position.y);
        List<Point> pathList = new List<Point>();
        if(startPoint.Equals(endPoint)) return;
        // 如果没有找到这条路线，则直接退出
        if(!AStar.FindPath(startPoint, endPoint, pathList)){
            AudioManager.Instance.PlayWhyClip();
            animator.SetTrigger("why");
            return;
        }
        // 将摄像机归位
        ResetPoint();
        isFindPath = true;
        animator.SetBool("idle", false);
        // 前者指的是，要走的几个点，后者指的是，总共经历了几秒
        tweener = player.transform.DOPath(pathList.ToVector3Array(), pathList.Count * 0.1f);
        // 速度线性变化
        tweener.SetEase(Ease.Linear);
        tweener.onComplete += ()=>{
            // 寻路结束时设置false
            isFindPath = false;
            animator.SetBool("idle", true);
        };
        tweener.onKill += ()=>{
            isFindPath = false;
            animator.SetBool("idle", true);
        };
    }

    #endregion

    #region 摄像机

    private void CameraInit(){
        // // 设置摄像机的视口size = 屏幕高度的一半
        // Camera.main.orthographicSize = (mapHeight + 1.5f * 2) / 2f;
        // // 设置摄像机的位置，摄像机本来就在Z轴-10的位置
        // Camera.main.transform.position = new Vector3((mapwidth - 1) / 2, (mapHeight - 1) / 2, -10);
        CinemachineVirtualCamera cinemachine = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        cinemachine.m_Lens.OrthographicSize = (mapHeight + 1.5f * 2) / 2f;
        PolygonCollider2D polygonCollider2D = GetComponent<PolygonCollider2D>();
        polygonCollider2D.SetPath(0, new Vector2[]{
            new Vector2(-2f, -2f),
            new Vector2(-2f, mapHeight + 1f),
            new Vector2(mapwidth + 1f, mapHeight + 1f),
            new Vector2(mapwidth + 1f, -2f),
        });
        // 将这个polygon的碰撞器后移，避免覆盖格子的碰撞器
        transform.position = new Vector3(0, 0, 10);
    }

    private void OnMouseOver() {
        // 提前预热，让Cinemachine可以动起来
        if(Input.GetMouseButtonDown(0)){
            player.transform.GetChild(0).position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2) + new Vector3(0, 0, 10));
        }
        if(Input.GetMouseButton(0)){
            player.transform.GetChild(0).position -= new Vector3(Input.GetAxis("Mouse X") * 0.5f, 0, 0);
        }
    }

    private void ResetPoint(){
        player.transform.GetChild(0).localPosition = Vector3.zero;
    }

    #endregion

    #region Tool

    #region 一维二维数组转换

    // 由一维数组转换为二维数组
    // x是第几列，y是第几行
    private void GetDoubleArr(int index, out int x, out int y){
        y = index / mapwidth;
        x = index - y * mapwidth;
    }

    // 二维转一维
    private int GetSingleArr(int x, int y){
        return y * mapwidth + x;
    }

    #endregion

    #region 八领域

    // 获得八领域的地雷数量
    public int GetAdjcentTrap(int x, int y){
        int count = 0;
        if(IsElement(x, y + 1, ElementContent.trap)) count++;
        if(IsElement(x + 1, y, ElementContent.trap)) count++;
        if(IsElement(x + 1, y + 1, ElementContent.trap)) count++;
        if(IsElement(x - 1, y, ElementContent.trap)) count++;
        if(IsElement(x - 1, y + 1, ElementContent.trap)) count++;
        if(IsElement(x + 1, y - 1, ElementContent.trap)) count++;
        if(IsElement(x, y - 1, ElementContent.trap)) count++;
        if(IsElement(x - 1, y - 1, ElementContent.trap)) count++;
        return count;
    }

    // 判断八领域是否为某一元素
    public bool IsElement(int x, int y, ElementContent elementContent){
        bool isElement = false;
        // 没有超过二维数组阈
        if(x >= 0 && x < mapwidth && y >= 0 && y < mapHeight){
            if(elements[x, y].elementContent == elementContent){
                isElement = true;
            }
        }
        return isElement;
    }

    // 泛洪算法，第三个参数指的是，当前元素是否已经被访问过了
    public void FloodFillElement(int x, int y, bool[,] visited){
        // 检测是否超出二维数组阈
        if(x >= 0 && x < mapwidth && y >= 0 && y < mapHeight){
            // 是否访问过
            if(!visited[x, y]){
                // 标记为访问过的元素
                visited[x, y] = true;
                // 如果当前这个元素就是地雷，那么不继续下去
                if(elements[x, y].elementContent == ElementContent.trap) return;
                // 除了不可翻开的元素之外，都可以翻开
                if(elements[x, y].elementType != ElementType.cantCovered){
                    // 翻开单翻元素
                    ((SingleCoveredElement)elements[x, y]).UncoveredElementSingle();
                    // 检查该元素附近是否有地雷
                    if(GetAdjcentTrap(x, y) > 0) return;
                    // 此时，该元素是已被标记，且是可以翻开的，且已经翻开了，通知八领域翻开
                    FloodFillElement(x + 1, y, visited);
                    FloodFillElement(x - 1, y, visited);
                    FloodFillElement(x, y + 1, visited);
                    FloodFillElement(x, y - 1, visited);
                    FloodFillElement(x + 1, y + 1, visited);
                    FloodFillElement(x + 1, y - 1, visited);
                    FloodFillElement(x - 1, y + 1, visited);
                    FloodFillElement(x - 1, y - 1, visited);
                }
            }
        }
    }

    #endregion

    #region 快速探查

    // 判断八领域是否为棋子，且该棋子必须插在地雷上才算正确
    // 如果已经有一颗雷显现出来了，那么也算一面棋子
    public bool IsFlag(int x, int y){
        bool isFlag = false;
        // 没有超过二维数组阈
        if(x >= 0 && x < mapwidth && y >= 0 && y < mapHeight && elements[x, y].elementContent == ElementContent.trap){
            if(elements[x, y].elementContent == ElementContent.trap && elements[x, y].elementState == ElementState.uncovered){
                isFlag = true;
                return isFlag;
            }
            if(elements[x, y].elementState == ElementState.marked){
                isFlag = true;
            }
        }
        return isFlag;
    }

    public bool SeekWrongFlag(int x, int y){
        bool isFlagWrong = false;
        // 没有超过二维数组阈
        if(x >= 0 && x < mapwidth && y >= 0 && y < mapHeight){
            if(elements[x, y].elementContent != ElementContent.trap && elements[x, y].elementState == ElementState.marked){
                isFlagWrong = true;
                return isFlagWrong;
            }
        }
        return isFlagWrong;
    }

    // 八领域有多少面棋子
    public int GetAdjcentFlag(int x, int y){
        int count = 0;
        if(IsFlag(x, y + 1)) count++;
        if(IsFlag(x + 1, y)) count++;
        if(IsFlag(x + 1, y + 1)) count++;
        if(IsFlag(x - 1, y)) count++;
        if(IsFlag(x - 1, y + 1)) count++;
        if(IsFlag(x + 1, y - 1)) count++;
        if(IsFlag(x, y - 1)) count++;
        if(IsFlag(x - 1, y - 1)) count++;
        return count;
    }

    public bool IsFlagWrong(int x, int y){
        bool Wrong = false;
        if(SeekWrongFlag(x, y + 1)) Wrong = true;
        if(SeekWrongFlag(x + 1, y)) Wrong = true;
        if(SeekWrongFlag(x + 1, y + 1)) Wrong = true;
        if(SeekWrongFlag(x - 1, y)) Wrong = true;
        if(SeekWrongFlag(x - 1, y + 1)) Wrong = true;
        if(SeekWrongFlag(x + 1, y - 1)) Wrong = true;
        if(SeekWrongFlag(x, y - 1)) Wrong = true;
        if(SeekWrongFlag(x - 1, y - 1)) Wrong = true;
        return Wrong;
    }

    // 八领域快速探查
    public void QuickSeek(int x, int y){
        IsNeedUnCovered(x, y + 1);
        IsNeedUnCovered(x + 1, y);
        IsNeedUnCovered(x + 1, y + 1);
        IsNeedUnCovered(x - 1, y);
        IsNeedUnCovered(x - 1, y + 1);
        IsNeedUnCovered(x + 1, y - 1);
        IsNeedUnCovered(x, y - 1);
        IsNeedUnCovered(x - 1, y - 1);
    }

    // 判断该位置是否需要翻开
    public bool IsNeedUnCovered(int x, int y){
        bool isNeedUnCovered = false;
        // 没有超过二维数组阈
        if(x >= 0 && x < mapwidth && y >= 0 && y < mapHeight){
            if(elements[x, y].elementContent == ElementContent.trap) return false;
            if(elements[x, y].elementType == ElementType.cantCovered) return false;
            if(elements[x, y].elementState != ElementState.uncovered){
                isNeedUnCovered = true;
                ((SingleCoveredElement)elements[x, y]).UncoveredElement();
                ((SingleCoveredElement)elements[x, y]).UncoveredElement();
                bool[,] visited = new bool[mapwidth, mapHeight];
                FloodFillElement(x, y, visited);
            }
        }
        return isNeedUnCovered;
    }

    #endregion

    #endregion

    #region 游戏失败

    public void GameOver(){
        // 显示所有地雷
        foreach (BaseElement item in elements)
        {
            if(item.elementContent == ElementContent.trap){
                ((TrapElement)item).UncoveredElementSingle();
            }
            // 标记错误，则实例化错误标志
            if(item.elementState == ElementState.marked && item.elementContent != ElementContent.trap){
                GameObject errorPre = Instantiate(error, item.transform);
                errorPre.name = "Error";
                errorPre.transform.DOLocalMoveY(0, 0.1f);
            }
        }
    }

    #endregion

    #region 血量

    public void TakeDamage(){
        if(armor > 0){
            armor--;
            MainPanel.Instance.UpdateUI(MainPanel.Instance.armorIcon.rectTransform, MainPanel.Instance.armorText.rectTransform);
        }else{
            hp--;
            MainPanel.Instance.UpdateUI(MainPanel.Instance.hpText.rectTransform);
        }
        
        if(hp <= 0){
            GameOver();
            animator.SetBool("die", true);
            lossPanel.SetActive(true);
            // 清空数据
            DataManager.Instance.ClearData();
            // 停止移动
            OnPanelShow();
            AudioManager.Instance.PlayEndClip();
            AudioManager.Instance.PlayLossClip();
        }else{
            animator.SetTrigger("takeDamage");
            AudioManager.Instance.PlayHurtClip();
        }
    }

    #endregion

    #region 数据管理

    private void LoadData(){
        DataManager.Instance.LoadData();
        mapHeight = DataManager.Instance.mapHeight;
        mapwidth = DataManager.Instance.mapwidth;
        level = DataManager.Instance.level;
        hp = DataManager.Instance.hp;
        armor = DataManager.Instance.armor;
        key = DataManager.Instance.key;
        hoe = DataManager.Instance.hoe;
        tnt = DataManager.Instance.tnt;
        map = DataManager.Instance.map;
        gold = DataManager.Instance.gold;
    }

    public void SaveData(){
        // 避免重复过关
        if(isOver) return;
        isOver = true;
        level++;
        if(hp < 3){
            hp = 3;
        }
        DataManager.Instance.SaveData(level, hp, armor, key, hoe, tnt, map, gold, AudioManager.Instance.isMute);
        // 播放胜利动画
        animator.SetBool("pass", true);
        endPanel.SetActive(true);
        // 停止移动
        OnPanelShow();
        // 播放金币特效
        GameObject.Find("Coin").GetComponent<ParticleSystem>().Play();
        AudioManager.Instance.PlayPassClip();
        AudioManager.Instance.PlayWinBgClip();
    }

    #endregion

    #region UI界面

    public void NextLevel(){
        SceneManager.LoadScene(1);
        AudioManager.Instance.PlayBtnClip();
    }

    public void ReturnStartPanel(){
        SceneManager.LoadScene(0);
        AudioManager.Instance.PlayBtnClip();
    }

    private void OnPanelShow(){
        // 使得碰撞体覆盖地图碰撞体，从而达到无法移动的效果
        this.transform.position = new Vector3(0, 0, -1);
    }

    #endregion
}
