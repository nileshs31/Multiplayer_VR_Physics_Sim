using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Netcode;
using TMPro;
#if UNITY_EDITOR
using ParrelSync;
#endif
public class PlayerCanvasHandler : NetworkBehaviour
{
    public TextMeshProUGUI numPlayers, chatText;
    public TMP_InputField chatInput;
    private NetworkVariable<int> noOfPlayers = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone);
 //   private NetworkVariable<FixedString128Bytes> chatTextServer = new NetworkVariable<FixedString128Bytes>("Chat -", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    // Start is called before the first frame update
    private void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
        numPlayers.text = "No. of Players - " + noOfPlayers.Value;
        if (IsServer)
        {
            noOfPlayers.Value = NetworkManager.Singleton.ConnectedClients.Count;
        }
    }

    public void sendChatButton()
    {
        if(chatInput.text != "")
        {
            var parrelarg = "";
#if UNITY_EDITOR
            parrelarg = ClonesManager.GetArgument();
#endif
            SendChatServerRpc(PlayerPrefs.GetString(parrelarg + "username"), DateTime.Now.ToString("MM-dd-yy HH:mm"), chatInput.text);
        }
        chatInput.text = "";
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendChatServerRpc(string playername, string timeString, string msg)
    {
        //chatTextServer.Value = playername + " - " + timeString + " : " + msg + "\n";
        SendChatClientRpc(playername, timeString, msg);

    }
    [ClientRpc(RequireOwnership = false)]
    public void SendChatClientRpc(string playername, string timeString, string msg)
    {
        chatText.text += playername + " - " + timeString + " : " + msg + "\n";
    }



    public void Exitgame()
    {
        if (NetworkManager.Singleton && NetworkManager.Singleton.IsConnectedClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
        SceneTransitionManager.singleton.GoToSceneAsync(0);
    }
}
