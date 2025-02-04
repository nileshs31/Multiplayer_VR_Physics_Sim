using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkMeteorPistol : NetworkBehaviour
{
    public ParticleSystem rayFromGun;
    public LayerMask layerMask;
    public Transform shootSource;
    public float distance = 10;

    private bool rayActivate = false;

    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.activated.AddListener(x => StartShoot());
        grabInteractable.deactivated.AddListener(x => StopShoot());
    }

    public void StartShoot()
    {
        if (IsOwner) // Only the owner can trigger shooting
        {
            rayActivate = true;
            rayFromGun.Play();
            StartShootServerRpc(); // Notify the server
        }
    }

    public void StopShoot()
    {
        if (IsOwner) // Only the owner can stop shooting
        {
            rayActivate = false;
            rayFromGun.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            StopShootServerRpc(); // Notify the server
        }
    }

    void Update()
    {
        if (IsOwner && rayActivate)
        {
            RaycastCheck();
        }
    }

    void RaycastCheck()
    {
        RaycastHit hit;
        bool hasHit = Physics.Raycast(shootSource.position, shootSource.forward, out hit, distance, layerMask);

        if (hasHit)
        {
            hit.transform.gameObject.SendMessage("Break", SendMessageOptions.DontRequireReceiver);
            // Send an RPC to notify all clients to perform the break
            NotifyBreakClientRpc();
        }
    }

    // ClientRpc to notify all clients to break the object
    [ClientRpc]
    private void NotifyBreakClientRpc()
    {

        Breakable breakable = FindObjectOfType<Breakable>();
        if (breakable != null)
        {
            breakable.Break();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartShootServerRpc()
    {
        StartShootClientRpc(); // Notify all clients
    }

    [ServerRpc(RequireOwnership = false)]
    private void StopShootServerRpc()
    {
        StopShootClientRpc(); // Notify all clients
    }

    [ClientRpc]
    private void StartShootClientRpc()
    {
        if (!IsOwner) // Prevent duplicate play for the owner
        {
            rayActivate = true;
            rayFromGun.Play();
        }
    }

    [ClientRpc]
    private void StopShootClientRpc()
    {
        if (!IsOwner) // Prevent duplicate stop for the owner
        {
            rayActivate = false;
            rayFromGun.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}
