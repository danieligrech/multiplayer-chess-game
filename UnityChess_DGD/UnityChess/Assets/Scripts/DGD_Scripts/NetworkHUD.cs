using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;

public class NetworkHUD : MonoBehaviour
{
    public SessionManager sessionManager;
    private string sessionCode = "";
    private bool isCreatingSession = false;

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 200 - 10, 10, 200, 160));
        if(!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Host"))
            {
                NetworkManager.Singleton.StartHost();
                isCreatingSession = true;
                sessionCode = "";
                sessionManager.CreateSession(code =>
                {
                    isCreatingSession = false;
                    if (!string.IsNullOrEmpty(code))
                    {
                        sessionCode = code;
                    }
                    else
                    {
                        sessionCode = "Error";
                    }
                });
            }
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

        if (isCreatingSession)
        {
            GUILayout.Label("Creating Session...");
        }
        else if (!string.IsNullOrEmpty(sessionCode))
        {
            GUILayout.Label($"Session Code: {sessionCode}");
        }
            GUILayout.EndArea();
    }
}
