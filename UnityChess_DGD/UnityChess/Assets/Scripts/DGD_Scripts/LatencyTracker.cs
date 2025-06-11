using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;
using System;

public class LatencyTracker : MonoBehaviour
{
    [Tooltip("Seconds Between Pings")]
    public float pingInterval = 5f;
    private float nextPingTime;

    // Update is called once per frame
    void Update()
    {
        if (!NetworkManager.Singleton.IsClient || !NetworkManager.Singleton.IsConnectedClient)
            return;

        if(Time.time >= nextPingTime)
        {
            nextPingTime = Time.time + pingInterval;
            long ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            Debug.Log($"[LatencyTracker] Sending Ping at {ts}");
            FindObjectOfType<TurnManager>().PingServerRpc(ts);
        }
    }
}
