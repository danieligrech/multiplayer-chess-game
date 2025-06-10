using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ResignUI : MonoBehaviour
{
    [SerializeField] private Button resignButton;
    private TurnManager _turnManager;

    // Start is called before the first frame update
    void Start()
    {
        _turnManager = FindObjectOfType<TurnManager>();

        resignButton.onClick.AddListener(OnResignClicked);
    }

    void OnResignClicked()
    {
        if(_turnManager != null)
        {
            _turnManager.ResignServerRpc();
        }
    }
}
