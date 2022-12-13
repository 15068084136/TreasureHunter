using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance{
        get{
            return instance;
        }
    }

    private void Awake() {
        instance = this;
        bgmSource = gameObject.AddComponent<AudioSource>();
        fxSource = gameObject.AddComponent<AudioSource>();
        PlayBGM();
    }

    private void Start() {
        isMute = DataManager.Instance.isMute;
    }

    #region AudioClip + AudioSource

    public AudioClip button;
    public AudioClip dig;
    public AudioClip end;
    public AudioClip hoe;
    public AudioClip hurt;
    public AudioClip die;
    public AudioClip move;
    public AudioClip door;
    public AudioClip pass;
    public AudioClip enemy;
    public AudioClip tnt;
    public AudioClip map;
    public AudioClip pick;
    public AudioClip flag;
    public AudioClip why;
    public AudioClip winbg;
    public AudioClip quickCheck;
    public AudioClip bgm;

    private AudioSource bgmSource;
    private AudioSource fxSource;

    #endregion

    public bool isMute = false;

    public void MuteAudio(){
        isMute = !isMute;
        if(isMute){
            StopBGM();
        }else{
            PlayBGM();
        }
    }

    #region BGMSource

    public void PlayBGM(){
        if(!isMute){
            bgmSource.clip = bgm;
            bgmSource.loop = true;
            bgmSource.Play();
        }else{
            return;
        }
    }

    public void StopBGM(){
        bgmSource.Pause();
    }

    #endregion

    #region FXSource

    public void PlayBtnClip(){
        if(!isMute){
            fxSource.PlayOneShot(button);
        }
    }

    public void PlayHoeClip(){
        if(!isMute){
            fxSource.PlayOneShot(hoe);
        }
    }

    public void PlayDigClip(){
        if(!isMute){
            fxSource.PlayOneShot(dig);
        }
    }

    public void PlayEndClip(){
        if(!isMute){
            fxSource.PlayOneShot(end);
        }
    }

    public void PlayPassClip(){
        if(!isMute){
            fxSource.PlayOneShot(pass);
        }
    }

    public void PlayHurtClip(){
        if(!isMute){
            fxSource.PlayOneShot(hurt);
        }
    }

    public void PlayLossClip(){
        if(!isMute){
            bgmSource.Stop();
            bgmSource.clip = die;
            bgmSource.loop = false;
            bgmSource.Play();
        }
    }

    public void PlayWinBgClip(){
        if(!isMute){
            bgmSource.Stop();
            bgmSource.clip = winbg;
            bgmSource.loop = false;
            bgmSource.Play();
        }
    }

    public void PlayMoveClip(){
        if(!isMute){
            fxSource.PlayOneShot(move);
        }
    }

    public void PlayDoorClip(){
        if(!isMute){
            fxSource.PlayOneShot(door);
        }
    }

    public void PlayEnemyClip(){
        if(!isMute){
            fxSource.PlayOneShot(enemy);
        }
    }

    public void PlayTntClip(){
        if(!isMute){
            fxSource.PlayOneShot(tnt);
        }
    }

    public void PlayMapClip(){
        if(!isMute){
            fxSource.PlayOneShot(map);
        }
    }

    public void PlayPickClip(){
        if(!isMute){
            fxSource.PlayOneShot(pick);
        }
    }

    public void PlayFlagClip(){
        if(!isMute){
            fxSource.PlayOneShot(flag);
        }
    }

    public void PlayWhyClip(){
        if(!isMute){
            fxSource.PlayOneShot(why);
        }
    }

    public void PlayQuickCheckClip(){
        if(!isMute){
            fxSource.PlayOneShot(quickCheck);
        }
    }

    #endregion
}
