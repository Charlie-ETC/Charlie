using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Asyncoroutine;

[RequireComponent(typeof(Animator))]
public class LookAtPlayerIKControl : MonoBehaviour {

    protected Animator animator;
    private Vector3 charlieToPlayer;
    private float angle;
    private const float NECK_ROTATION_RANGE = 80f;
    private const float ATTENTION_DISTANCE = 2F;
    private const float TURN_BACK_SPEED_RATE = 0.96f;
    private const float TURN_FORTH_SPEED_RATE = 0.1f;
    private float currentLookAtWeight;

    private bool isTurningBody = false;
    private bool isLookingAway = false;
    private IEnumerator lookAwayCoroutine;


    public bool isActive;
    public bool lookAwayOn;
    [Range(0,1)]
    public float lookAtWeight;
    [Range(0, 1)]
    public float headWeight;
    [Range(0, 1)]
    public float eyeWeight;
    [Range(0, 1)]
    public float bodyWeight;
    public GameObject target;
    //public LookatPlayer lookatPlayer;


	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        currentLookAtWeight = 0;
        lookAwayCoroutine = LookAway();
        StartCoroutine(lookAwayCoroutine);
    }

    private IEnumerator LookAway() {
        while (true) {
            if (isActive && lookAwayOn && !transform.parent.gameObject.GetComponent<AudioSource>().isPlaying) {
                if (Random.Range(0f, 1f) < 0.25f) {
                    isLookingAway = true;
                    yield return new WaitForSeconds(Random.Range(1.5f, 2f));
                    isLookingAway = false;
                }
            }

            yield return new WaitForSeconds(1.5f);
        }
        
    }

    private void OnDestroy()
    {
        if (lookAwayCoroutine != null) StopCoroutine(lookAwayCoroutine);
    }

    private void OnAnimatorIK() {

        if (animator != null) {
            if (isActive && target != null && !isLookingAway)
            {
                charlieToPlayer = target.transform.position - transform.position;
                angle = Vector3.Angle(transform.forward, charlieToPlayer);

                // Debug.Log(Mathf.Abs(Vector3.Angle(transform.forward, charlieToPlayer)));
                // Debug.Log(charlieToPlayer.magnitude);

                // when target is in Charlie's attention distance (too far to notice)
                
                if (charlieToPlayer.magnitude < ATTENTION_DISTANCE) {
                    // within her normal neck rotation range
                    // turn her head to face player
                    if (Mathf.Abs(angle) <= NECK_ROTATION_RANGE && !isTurningBody)
                    {
                        //if (lookatPlayer.enabled) lookatPlayer.enabled = false;

                        if (lookAtWeight - currentLookAtWeight < 0.001f) { currentLookAtWeight = lookAtWeight; }
                        else
                        {
                            // currentLookAtWeight = Mathf.Lerp(currentLookAtWeight, lookAtWeight, TURN_FORTH_SPEED_RATE); 
                            currentLookAtWeight += (1 - TURN_BACK_SPEED_RATE) * 0.8f;
                        }
                        //currentLookAtWeight = lookAtWeight;
                        //Debug.Log(currentLookAtWeight);
                        animator.SetLookAtWeight(currentLookAtWeight, bodyWeight, headWeight, eyeWeight);
                        animator.SetLookAtPosition(target.transform.position);
                    }
                    else // otherwise turn around her body to face player
                    {
                        isTurningBody = true;

                        if (currentLookAtWeight < 0.001f) { currentLookAtWeight = 0f; }
                        else { currentLookAtWeight = Mathf.Lerp(0f, currentLookAtWeight, TURN_BACK_SPEED_RATE); }
                        animator.SetLookAtWeight(currentLookAtWeight);

                        Vector3 v1 = Vector3.ProjectOnPlane(transform.parent.forward, Vector3.up);
                        Vector3 v2 = Vector3.ProjectOnPlane(charlieToPlayer, Vector3.up);
                        float vAngle = Vector3.Angle(v1, v2);  // absolute value

                        // use cross product to tell the direction
                        if (Vector3.Cross(v1, v2).y < 0) vAngle = -vAngle;

                        float xZAngle = vAngle * (1f - TURN_BACK_SPEED_RATE) * 0.4f;
                        transform.parent.eulerAngles = new Vector3(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y + xZAngle, transform.parent.eulerAngles.z);
                        //if (!lookatPlayer.enabled) lookatPlayer.enabled = true;

                        if (Mathf.Abs(vAngle) < 1f) isTurningBody = false;
                    }
                }

            }
            else {
                // set IK back to original pose
                if (currentLookAtWeight < 0.00001f) { currentLookAtWeight = 0f; }
                else {currentLookAtWeight = Mathf.Lerp(0f, currentLookAtWeight, TURN_BACK_SPEED_RATE); }
                animator.SetLookAtWeight(currentLookAtWeight);
            }


        }
    }
}
