using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LookAtPlayerIKControl : MonoBehaviour {

    protected Animator animator;
    private Vector3 charlieToPlayer;
    private const float NECK_ROTATION_RANGE = 70f;
    private const float ATTENTION_DISTANCE = 2.5F;

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


	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
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
                if (charlieToPlayer.magnitude < ATTENTION_DISTANCE &&
                        Mathf.Abs(Vector3.Angle(transform.forward, charlieToPlayer)) < NECK_ROTATION_RANGE) {
                    animator.SetLookAtWeight(lookAtWeight, bodyWeight, headWeight, eyeWeight);
                    animator.SetLookAtPosition(target.transform.position);
                }
                
            }
            else {
                // set IK back to original pose
                animator.SetLookAtWeight(0);
            }

        }
    }
}
