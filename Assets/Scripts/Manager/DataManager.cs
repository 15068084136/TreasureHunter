using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public static DataManager Instance{
        get{
            return instance;
        }
    }

    #region 数据

    public int mapwidth;
    public int mapHeight;
    public int level;
    public int hp;
    public int armor;
    public int key;
    public int hoe;
    public int tnt;
    public int map;
    public int gold;
    public bool isMute;

    #endregion

    private void Awake() {
        if(instance != null){
            Destroy(this.gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        LoadData();
    }

    public void LoadData(){
        mapwidth = 20 + level * 3;
        mapHeight = UnityEngine.Random.Range(9, 12);
        level = PlayerPrefs.GetInt("level", 1);
        hp = PlayerPrefs.GetInt("hp", 3);
        armor = PlayerPrefs.GetInt("armor", 0);
        key = PlayerPrefs.GetInt("key", 0);
        hoe = PlayerPrefs.GetInt("hoe", 0);
        tnt = PlayerPrefs.GetInt("tnt", 0);
        map = PlayerPrefs.GetInt("map", 0);
        gold = PlayerPrefs.GetInt("gold", 0);
        isMute = PlayerPrefs.GetInt("isMute", 1) == 0?true:false;
    }

    public void SaveData(int level, int hp, int armor, int key, int hoe, int tnt, int map, int gold, bool isMute){
        this.level = level;
        this.hp = hp;
        this.armor = armor;
        this.key = key;
        this.hoe = hoe;
        this.tnt = tnt;
        this.map = map;
        this.gold = gold;
        this.isMute = isMute;
        PlayerPrefs.SetInt("level", this.level);
        PlayerPrefs.SetInt("hp", this.hp);
        PlayerPrefs.SetInt("armor", this.armor);
        PlayerPrefs.SetInt("key", this.key);
        PlayerPrefs.SetInt("hoe", this.hoe);
        PlayerPrefs.SetInt("tnt", this.tnt);
        PlayerPrefs.SetInt("map", this.map);
        PlayerPrefs.SetInt("gold", this.gold);
        PlayerPrefs.SetInt("isMute", isMute?0:1);
    }

    public void ClearData(){
        SaveData(1, 3, 0, 0, 0, 0, 0, 0, AudioManager.Instance.isMute);
    }
}
