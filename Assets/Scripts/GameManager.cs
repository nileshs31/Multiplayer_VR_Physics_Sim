using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance; // Singleton instance
    public List<CustomPhysicsSim> physicsObjects;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void OnTwoJoinedServerRpc()
    {
        OnTwoJoinedClientRpc();
    }

    [ClientRpc(RequireOwnership = false)]
    public void OnTwoJoinedClientRpc()
    {
        foreach (var item in physicsObjects)
        {
            item.ResetRequest();
        }
    }
}
