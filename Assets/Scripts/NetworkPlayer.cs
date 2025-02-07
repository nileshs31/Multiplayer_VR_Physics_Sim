using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR;
using UnityEngine.XR.Interaction;
public class NetworkPlayer : NetworkBehaviour
{

    public Transform root, body, leftHand, rightHand;
    public Renderer[] meshToDisable;

    void Update()
    {
        if (IsOwner)
        {
            root.SetPositionAndRotation(VRRigReferences.Singleton.root.position, VRRigReferences.Singleton.root.rotation);

            leftHand.SetPositionAndRotation(VRRigReferences.Singleton.leftHand.position, VRRigReferences.Singleton.leftHand.rotation);

            rightHand.SetPositionAndRotation(VRRigReferences.Singleton.rightHand.position, VRRigReferences.Singleton.rightHand.rotation);
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn(); 
        if (IsOwner)
        {
            foreach (var item in meshToDisable)
            {
                item.enabled = false;
            }
        }

        if (OwnerClientId > 0)
            TwoPlayersJoined();
    }


    private void TwoPlayersJoined()
    {
        if (IsOwner)
            GameManager.Instance.OnTwoJoinedServerRpc();
    }

}
