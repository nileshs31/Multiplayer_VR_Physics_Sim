using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRigReferences : MonoBehaviour
{
    public static VRRigReferences Singleton;

    public Transform root, leftHand, rightHand;


    private void Awake()
    {
        Singleton = this;
    }
}
