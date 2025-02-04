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


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (OwnerClientId == 0)
        {
            GameObject[] go = GameObject.FindGameObjectsWithTag("Player");
            foreach(var item in go)
            {
                if (IsOwner)
                    item.transform.position = new Vector3(-1, 0, 0);
            }
            //SpectatorJoined();
            //TwoPlayersJoined();
        }

        else if (OwnerClientId == 1)
        {
            GameObject[] go = GameObject.FindGameObjectsWithTag("Player");
            foreach (var item in go)
            {
                if (IsOwner)
                    item.transform.position = new Vector3(2, 0, 0);
            }
            TwoPlayersJoined();
        }

        else if (OwnerClientId == 2)
        {
            GameObject[] go = GameObject.FindGameObjectsWithTag("Player");
            foreach (var item in go)
            {
                if (IsOwner)
                    item.transform.position = new Vector3(1.5f, 0, -5);
            }
            SpectatorJoined();
        }

        if (IsOwner)
        {
            foreach (var item in meshToDisable)
            {
                item.enabled = false;
            }
        }

    }

    private void SpectatorJoined()
    {
        // Notify the GameManager about the spectator joining
        if (IsOwner)
        {
            GameManager.Instance.OnSpectatorJoined();
            QuestionManager.Instance.TurnQuestionUI();

        }
    }

    private void TwoPlayersJoined()
    {
        // Notify the GameManager about 2 players joining and quiz can start
        if (IsOwner)
            QuestionManager.Instance.OnTwoJoinedServerRpc();
    }

    void Update()
    {
        if (IsOwner)
        {
            root.SetPositionAndRotation(VRRigReferences.Singleton.root.position, VRRigReferences.Singleton.root.rotation);

            leftHand.SetPositionAndRotation(VRRigReferences.Singleton.leftHand.position, VRRigReferences.Singleton.leftHand.rotation);

            rightHand.SetPositionAndRotation(VRRigReferences.Singleton.rightHand.position, VRRigReferences.Singleton.rightHand.rotation);
        }
    }
}
