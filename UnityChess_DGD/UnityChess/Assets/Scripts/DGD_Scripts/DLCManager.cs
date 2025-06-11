using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Storage;
using Unity.Netcode;

public class DLCManager : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The UI Image Where the DLC Will be Shown")]
    public Image previewImage;

    private FirebaseStorage storage;
    private StorageReference dlcRef;
    private string localPath;

    void Awake()
    {
        storage = FirebaseStorage.DefaultInstance;
        dlcRef = storage.GetReference("dlc/dlc_success.png");
        localPath = Path.Combine(Application.persistentDataPath, "dlc_success.png");

        previewImage.gameObject.SetActive(false);
    }

    public void OnPurchasePressed()
    {
        FindObjectOfType<TurnManager>().PurchaseDLCServerRpc();
    }

    public void ShowPurchasedDLC()
    {
        _ = DownloadAndDisplayAsync();
    }

    private async Task DownloadAndDisplayAsync()
    {
        Debug.Log("[DLCManager] Downloading the DLC...");

        try
        {
            await dlcRef.GetFileAsync(localPath);
            Debug.Log($"[DLCManager] Saved DLC to {localPath}");

            byte[] bytes = File.ReadAllBytes(localPath);
            var tex = new Texture2D(2, 2);
            if (!tex.LoadImage(bytes))
            {
                throw new Exception("Failed to Load the Image From Bytes...");
            }

            var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            previewImage.sprite = sprite;
            previewImage.gameObject.SetActive(true);
        }
        catch (Exception e)
        {
            Debug.LogError($"[DLCManager] Download Failed..: {e}");
        }
    }
}
