using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDes : MonoBehaviour
{
    public float DestroyTime;
    void Start()
    {
        Destroy(gameObject, DestroyTime);
    }
}
