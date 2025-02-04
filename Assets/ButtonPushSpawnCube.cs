using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonPushSpawnCube : NetworkBehaviour
{
    public GameObject cubeToSpawn;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<XRSimpleInteractable>().selectEntered.AddListener(x => CubeSpawner());
    }

    public void CubeSpawner()
    {
        Debug.Log("ButtonPushed");
        CubeSpawnerServerRpc();
    }


    [ServerRpc(RequireOwnership = false)]
    public void CubeSpawnerServerRpc()
    {
        
        CubeSpawnerClientRpc();
    }


    [ClientRpc(RequireOwnership = false)]
    public void CubeSpawnerClientRpc()
    {
        if (cubeToSpawn.activeInHierarchy)
            cubeToSpawn.SetActive(false);
        else
        {
            cubeToSpawn.transform.position = new Vector3(0.0004f, 0.6540639f, 1.8407f);
            cubeToSpawn.SetActive(true);

        }
        cubeToSpawn.GetComponent<Rigidbody>().isKinematic = false;
    }
}
