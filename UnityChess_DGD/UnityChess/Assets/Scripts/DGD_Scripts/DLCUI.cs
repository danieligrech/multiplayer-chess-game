using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DLCUI : MonoBehaviour
{
    [Tooltip("Drag and Drop the DLCPanel Here!")]
    public GameObject dlcPanel;

    // Start is called before the first frame update
    void Start()
    {
        dlcPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            dlcPanel.SetActive(!dlcPanel.activeSelf);
        }
    }
}
