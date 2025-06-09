using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

public class NetworkHUD : MonoBehaviour
{
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 150, 120));
        if(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Host")) NetworkManager.Singleton.StartHost();
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server")) NetworkManager.Singleton.StartServer();
        }
        else
        {
            string status =
                NetworkManager.Singleton.IsHost ? "Hosting" :
                NetworkManager.Singleton.IsClient ? "Client" : "Server";
            GUILayout.Label($"Status: {status}");
        }
        GUILayout.EndArea();
    }
}
