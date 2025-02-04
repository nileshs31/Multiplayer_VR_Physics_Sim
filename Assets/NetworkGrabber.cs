using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkGrabber : NetworkBehaviour
{
    // Called when the grabbable object is selected
    public void OnSelectGrabbable(SelectEnterEventArgs args)
    {
        NetworkObject networkObj = args.interactableObject.transform.GetComponent<NetworkObject>();
        if (networkObj != null)
        {
            if (IsHost) // For the host
            {
                // Host takes ownership directly without needing an RPC
                networkObj.ChangeOwnership(NetworkManager.ServerClientId);
            }
            else if (IsClient) // For clients
            {
                // Request ownership from the server
                RequestGrabOwnershipServerRpc(networkObj.NetworkObjectId);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestGrabOwnershipServerRpc(ulong networkObjectId, ServerRpcParams rpcParams = default)
    {
        // Find the object using the provided NetworkObjectId
        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject netObj))
        {
            // Transfer ownership to the requesting client
            ulong requestingClientId = rpcParams.Receive.SenderClientId;
            netObj.ChangeOwnership(requestingClientId);
        }
        else
        {
            Debug.LogError("NetworkObject not found for ownership transfer.");
        }
    }

    // Optional: Handle deselection if needed
    public void OnDeselectGrabbable(SelectExitEventArgs args)
    {
        NetworkObject networkObj = args.interactableObject.transform.GetComponent<NetworkObject>();
        if (networkObj != null && networkObj.IsOwner)
        {
            if (IsHost)
            {
                // Host releases ownership directly
                networkObj.ChangeOwnership(NetworkManager.ServerClientId);
            }
            else if (IsClient)
            {
                // Client requests the server to release ownership
                ReleaseOwnershipServerRpc(networkObj.NetworkObjectId);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReleaseOwnershipServerRpc(ulong networkObjectId)
    {
        if (NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject netObj))
        {
            // Release ownership back to the server
            netObj.ChangeOwnership(NetworkManager.ServerClientId);
        }
        else
        {
            Debug.LogError("NetworkObject not found for ownership release.");
        }
    }
}
