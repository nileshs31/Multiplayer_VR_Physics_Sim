using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Content.Interaction;

public class WheelAudioPlayer : NetworkBehaviour
{
    public XRKnob wheelLeft;
    public AudioSource audiop;
    bool played = false;

    private NetworkVariable<float> wheelLeftValue = new NetworkVariable<float>(0f , NetworkVariableReadPermission.Everyone);

    // Start is called before the first frame update
    void Start()
    {
        
    }


    [ServerRpc(RequireOwnership = false)]
    public void ChangeWheelValsServerRpc()
    {
        ChangeWheelValsClientRpc();

    }
    [ClientRpc(RequireOwnership = false)]
    public void ChangeWheelValsClientRpc()
    {
        wheelLeftValue.Value = wheelLeft.value;

    }


    // Update is called once per frame
    void Update()
    {
        
        ChangeWheelValsServerRpc();

        if (wheelLeftValue.Value > 0.6f)
        {
            if (!played)
            {
                audiop.Play();
                played = true;
            }
        }
        else
        {
            played = false;

        }
    }
}
