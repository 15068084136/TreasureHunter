using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public void PlayMoveClip(){
        AudioManager.Instance.PlayMoveClip();
    }
}
