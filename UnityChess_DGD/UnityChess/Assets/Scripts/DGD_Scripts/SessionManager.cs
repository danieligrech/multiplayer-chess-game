using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class SessionManager : MonoBehaviour
{
    private DatabaseReference _dbRoot;
    private FirebaseAuth _auth;

    void Awake()
    {
        _auth = FirebaseAuth.DefaultInstance;
        string dbUrl = "https://dg-cg-homeassignment-default-rtdb.europe-west1.firebasedatabase.app/";
        _dbRoot = FirebaseDatabase.GetInstance(FirebaseApp.DefaultInstance, dbUrl).RootReference;
    }

    public void CreateSession(Action<string> onCreated)
    {
        string code = UnityEngine.Random.Range(0, 999999).ToString("D6");
        string hostUid = _auth.CurrentUser.UserId;
        var sessionRef = _dbRoot.Child("sessions").Child(code);

        var updates = new Dictionary<string, object>
        {
            ["hostId"] = hostUid,
            ["state"] = "waiting"
        };

        sessionRef.UpdateChildrenAsync(updates).ContinueWith(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError($"Failed to Create a Session..: {task.Exception}");
                onCreated?.Invoke(null);
            }
            else
            {
                Debug.Log($"Session Number {code} has Been Successfully Created!");
                onCreated?.Invoke(code);
            }
        });
    }
}
