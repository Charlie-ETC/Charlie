using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LookAtPlayerIKControl : MonoBehaviour {

    protected Animator animator;
    private Vector3 charlieToPlayer;
    private float angle;
    private const float NECK_ROTATION_RANGE = 80f;
    private const float ATTENTION_DISTANCE = 2F;
    private const float TURN_BACK_SPEED_RATE = 0.99f;
    private const float TURN_FORTH_SPEED_RATE = 0.1f;
    private float currentLookAtWeight;

    public bool isActive;
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
    }

    private void OnAnimatorIK() {

        if (animator != null) {
            if (isActive && target != null)
            {
                charlieToPlayer = target.transform.position - transform.position;
                angle = Vector3.Angle(transform.forward, charlieToPlayer);

                // Debug.Log(Mathf.Abs(Vector3.Angle(transform.forward, charlieToPlayer)));
                // Debug.Log(charlieToPlayer.magnitude);

                // when target is in Charlie's attention distance (too far to notice)
                // and within her normal neck rotation range
                // turn her head to face player
                if (charlieToPlayer.magnitude < ATTENTION_DISTANCE && Mathf.Abs(angle) <= NECK_ROTATION_RANGE)
                {
                    //if (lookatPlayer.enabled) lookatPlayer.enabled = false;

                    if (lookAtWeight - currentLookAtWeight < 0.001f) { currentLookAtWeight = lookAtWeight; }
                    else { 
                        // currentLookAtWeight = Mathf.Lerp(currentLookAtWeight, lookAtWeight, TURN_FORTH_SPEED_RATE); 
                        currentLookAtWeight += (1 - TURN_BACK_SPEED_RATE) * 0.8f;
                    }
                    //currentLookAtWeight = lookAtWeight;
                    //Debug.Log(currentLookAtWeight);
                    animator.SetLookAtWeight(currentLookAtWeight, bodyWeight, headWeight, eyeWeight);       
                    animator.SetLookAtPosition(target.transform.position);
                }
                else if (charlieToPlayer.magnitude < ATTENTION_DISTANCE)// otherwise turn around her body to face player
                {
                    if (currentLookAtWeight < 0.001f) { currentLookAtWeight = 0f; }
                    else { currentLookAtWeight = Mathf.Lerp(0f, currentLookAtWeight, TURN_BACK_SPEED_RATE); }
                    animator.SetLookAtWeight(currentLookAtWeight);

                    float xZAngle = Vector3.Angle(Vector3.ProjectOnPlane(transform.parent.forward, Vector3.up), Vector3.ProjectOnPlane(charlieToPlayer, Vector3.up)) * (1f - TURN_BACK_SPEED_RATE) * 0.4f;
                    transform.parent.eulerAngles = new Vector3(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y + xZAngle, transform.parent.eulerAngles.z);
                    //if (!lookatPlayer.enabled) lookatPlayer.enabled = true;
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
