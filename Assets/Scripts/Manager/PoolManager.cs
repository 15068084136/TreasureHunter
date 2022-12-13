using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    private static PoolManager instance;
    public static PoolManager Instance{
        get{
            return instance;
        }
    }

    private Dictionary<EffectType, List<GameObject>> poolDic = new Dictionary<EffectType, List<GameObject>>();
    private List<GameObject> smokeParticalList = new List<GameObject>();
    private List<GameObject> brickParticalList = new List<GameObject>();
    private List<GameObject> starParticalList = new List<GameObject>();
    private Dictionary<EffectType, int> poolCapacityDic = new Dictionary<EffectType, int>();
    int smokeParticalListCapa;
    int brickParticalListCapa = 100;
    int starParticalListCapa = 20;
    private Dictionary<EffectType, GameObject> effectPreDic = new Dictionary<EffectType, GameObject>();

    public void Init(){
        instance = this;
        poolDic.Add(EffectType.smokePartical, smokeParticalList);
        poolDic.Add(EffectType.brickPartical, brickParticalList);
        poolDic.Add(EffectType.starPartical, starParticalList);
        smokeParticalListCapa = (int)(GameManager.Instance.mapHeight * GameManager.Instance.mapwidth * 0.2f);
        poolCapacityDic.Add(EffectType.smokePartical, smokeParticalListCapa);
        poolCapacityDic.Add(EffectType.brickPartical, brickParticalListCapa);
        poolCapacityDic.Add(EffectType.starPartical, starParticalListCapa);
        effectPreDic.Add(EffectType.smokePartical, GameManager.Instance.smokePartical.gameObject);
        effectPreDic.Add(EffectType.brickPartical, GameManager.Instance.brickPartical.gameObject);
        effectPreDic.Add(EffectType.starPartical, GameManager.Instance.starPartical.gameObject); 
    }

    public GameObject GetGameObject(EffectType effectType, Transform transform){
        List<GameObject> list;
        if(poolDic.TryGetValue(effectType, out list)){
            if(list.Count > 0){
                GameObject gameObject = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                if(gameObject != null){
                    gameObject.SetActive(true);
                    
                    if(transform != null){
                        gameObject.transform.SetParent(transform);
                    }
                    gameObject.transform.localPosition = Vector3.zero;
                    // 重置物体状态
                    ResetGameObject(gameObject);
                    return gameObject;
                }else{
                    return null;
                }
            }else{
                GameObject go;
                if(effectPreDic.TryGetValue(effectType, out go)){
                    GameObject gameObject = Instantiate(go, transform);
                    gameObject.transform.localPosition = Vector3.zero;
                    return gameObject;
                }else{
                    return null;
                }
            }
        }else{
            return null;
        }
    }

    private void ResetGameObject(GameObject gameObject){
        if(gameObject != null){
            ParticleSystem particle = gameObject.GetComponent<ParticleSystem>();
            particle.Stop();
            particle.Play();
        }
    }

    public void StoreGameObject(EffectType effectType, GameObject go){
        List<GameObject> list;
        poolDic.TryGetValue(effectType, out list);
        int capa;
        poolCapacityDic.TryGetValue(effectType, out capa);
        go.SetActive(false);
        if(list.Count < capa){
            list.Add(go);
        }else{
            Destroy(go);
        }
    }
}
