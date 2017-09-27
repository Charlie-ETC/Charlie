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

    // always face the direction that the player is looking at
    private void Update()
    {
        transform.LookAt(Vector3.ProjectOnPlane(Camera.main.transform.position, Vector3.up));
    }

    public void SpeakAnimation(float audioLen) {
        
        if ( bodyMovementClip.length < audioLen * 2.0f) {
            // blend body movement animation
            animator.SetBool("BlendMovement", true);
            animator.SetBool("MouseTalkEyeBlink", true);
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
        //animator.Play("girl idle");

        if (animator.GetBool("BlendMovement")) {
            animator.SetBool("BlendMovement", false);
        }

        if (animator.GetBool("MouseTalkEyeBlink")) {
            animator.SetBool("MouseTalkEyeBlink", false);
        }

        
    }

}
