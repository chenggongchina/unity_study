using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTest : MonoBehaviour
{
    public Transform lookAtPos;
    public Transform rightHandObj;
    private Animator anim;
    
    void OnAnimatorIK()
    {
        //Debug.Log("OnAnimatorIK called");
        if (lookAtPos != null)
        {
            anim.SetLookAtWeight(1.0f);
            anim.SetLookAtPosition(lookAtPos.position);    
        }
        
        // Set the right hand target position and rotation, if one has been assigned
        if(rightHandObj != null) {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand,1);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand,1);  
            anim.SetIKPosition(AvatarIKGoal.RightHand,rightHandObj.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand,rightHandObj.rotation);
        }     
    }
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }
}
