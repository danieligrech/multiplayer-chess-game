using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Extensions;
using Firebase.Auth;
using System;
using Firebase.Storage;

public class FirebaseInitialiser : MonoBehaviour
{
    private FirebaseAuth _auth;

    void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(depTask =>
        {
            if (depTask.Result == DependencyStatus.Available)
            {
                Debug.Log("Firebase is Ready!");
                _auth = FirebaseAuth.DefaultInstance;
                SignInAnon();

                var app = FirebaseApp.DefaultInstance;
                Debug.Log($"[Firebase Init] apiKey: {app.Options.ApiKey}, projectId: {app.Options.ProjectId}");

                FirebaseStorage storage = FirebaseStorage.DefaultInstance;
                Debug.Log($"Firebase Storage has been Successfully Implemented!!: {storage.RootReference.Path}");
            }
            else
            {
                Debug.LogError($"Could not Resolve the Firebase Dependencies..: {depTask.Result}");
            }
        });
    }

    private void SignInAnon()
    {
        _auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(signInTask =>
        {
            if (signInTask.Exception != null)
            {
                Debug.LogError($"Authentication Failed..: {signInTask.Exception}");
                return;
            }

            FirebaseUser newUser = _auth.CurrentUser;
            Debug.Log($"Signed in Anonymously as {newUser.UserId}");
        });
    }
}
