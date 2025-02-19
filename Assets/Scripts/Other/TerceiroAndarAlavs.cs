using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TerceiroAndarAlavs : MonoBehaviour
{
    private int numberOfInter = 0;
    private bool canInteract = false;
    public int maxNumsTillUnlock = 3;
    public BoxCollider _collidToUnlock;
    public GameObject DoorOpen;
    private bool isNotified;
    public AudioClip deactiveSound;
    public AudioClip switchSound;
    public AudioSource vfxAudioSource;

    public UnityEvent OnUnlock;
    
    public void Start(){

    }

    public void IncreaseInter(){
        numberOfInter++;
        vfxAudioSource.PlayOneShot(switchSound);
    }

    public void Update(){
        if(numberOfInter == maxNumsTillUnlock && !canInteract){
            
            canInteract = true;
            _collidToUnlock.enabled = true;
            
        }
        if(numberOfInter == maxNumsTillUnlock + 1 && canInteract && !isNotified){
            
            isNotified = true;
            DoorOpen.SetActive(false);
            OnUnlock.Invoke();
            vfxAudioSource.PlayOneShot(deactiveSound);
        }
    }

    
}
