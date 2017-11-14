using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LookAtPlayerIKControl : MonoBehaviour {

    protected Animator animator;
    private Vector3 charlieToPlayer;
    private const float NECK_ROTATION_RANGE = 85f;
    private const float ATTENTION_DISTANCE = 5F;
    private const float TURN_BACK_SPEED_RATE = 0.95f;
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
    public LookatPlayer lookatPlayer;




	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        currentLookAtWeight = lookAtWeight;
    }

    private void OnAnimatorIK() {
        if (animator != null) {
            if (isActive && target != null)
            {
                charlieToPlayer = target.transform.position - transform.position;
                // Debug.Log(Mathf.Abs(Vector3.Angle(transform.forward, charlieToPlayer)));
                // Debug.Log(charlieToPlayer.magnitude);

                // when target is in Charlie's attention distance (too far to notice)
                // and within her normal neck rotation range
                // turn her head to face player
                if (charlieToPlayer.magnitude < ATTENTION_DISTANCE &&
                        Mathf.Abs(Vector3.Angle(transform.forward, charlieToPlayer)) < NECK_ROTATION_RANGE)
                {
                    if (lookatPlayer.enabled) lookatPlayer.enabled = false;

                    if (lookAtWeight - currentLookAtWeight < 0.001f) { currentLookAtWeight = lookAtWeight; }
                    else { currentLookAtWeight = Mathf.Lerp(lookAtWeight, 0f, TURN_BACK_SPEED_RATE); }

                    animator.SetLookAtWeight(lookAtWeight, bodyWeight, headWeight, eyeWeight);       
                    animator.SetLookAtPosition(target.transform.position);
                }
                else if (charlieToPlayer.magnitude < ATTENTION_DISTANCE)// otherwise turn around her body to face player
                {
                    if (currentLookAtWeight < 0.001f) { currentLookAtWeight = 0f; }
                    else { currentLookAtWeight = Mathf.Lerp(0f, currentLookAtWeight, TURN_BACK_SPEED_RATE); }
                    animator.SetLookAtWeight(currentLookAtWeight);

                    if (currentLookAtWeight == 0f)
                    {
                        Debug.Log("Hello");
                        if (!lookatPlayer.enabled) lookatPlayer.enabled = true;
                        //transform.parent.LookAt(Vector3.ProjectOnPlane(Vector3.Lerp(target.transform.position, transform.parent.forward, TURN_BACK_SPEED_RATE), Vector3.up));
                    }
                }

            }
            else {
                // set IK back to original pose
                if (currentLookAtWeight < 0.001f) { currentLookAtWeight = 0f; }
                else {currentLookAtWeight = Mathf.Lerp(0f, currentLookAtWeight, TURN_BACK_SPEED_RATE); }
                animator.SetLookAtWeight(currentLookAtWeight);
            }

        }
    }
}
