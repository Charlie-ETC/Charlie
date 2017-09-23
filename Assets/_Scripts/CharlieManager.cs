using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharlieManager : MonoBehaviour {

    public AnimationClip bodyMovementClip;
    public Animator animator;
    public IEnumerator speakCoroutine;
    
    public static CharlieManager Instance { get; private set; }

    // singleton instance
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else {
            Destroy(this);
        }
    }

    public void SpeakAnimation(float audioLen) {
        
        if ( bodyMovementClip.length < audioLen) {
            // blend body movement animation
            animator.SetBool("BlendMovement", true);
        }
        else {
            // start speak animation 
            animator.SetBool("MouseTalkEyeBlink", true);
        }
        
        if (speakCoroutine != null) { StopCoroutine(speakCoroutine); }
        speakCoroutine = Speak(audioLen);
        StartCoroutine(speakCoroutine);
    }

    private IEnumerator Speak(float audioLen) {

        while (audioLen > 0) {
            yield return new WaitForSeconds(Time.deltaTime);
            audioLen -= Time.deltaTime;
        }

        // set back to idle animation
        animator.Play("girl idle");
    }

}
