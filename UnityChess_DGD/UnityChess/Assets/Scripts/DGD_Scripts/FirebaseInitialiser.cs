using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Extensions;

public class FirebaseInitialiser : MonoBehaviour
{
    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase is Ready!");
            }
            else
            {
                Debug.LogError($"Could not Resolve Firebase Dependencies...: {task.Result}");
            }
        });
    }
}
