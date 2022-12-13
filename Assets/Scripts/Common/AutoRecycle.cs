using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRecycle : MonoBehaviour
{
    public EffectType effectType;

    public float delayTime;

    private void Start() {
        Invoke("Recycle", delayTime);
    }

    private void OnEnable() {
        Invoke("Recycle", delayTime);
    }   

    private void Recycle(){
        GameManager.Instance.poolManager.StoreGameObject(effectType, gameObject);
    }
}
