using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField]
    private Button hostBtn;

    [SerializeField]
    private Button serverBtn;

    [SerializeField]
    private Button clientBtn;

    [SerializeField]
    GameObject networkUI;

    private void Awake()
    {
        hostBtn.onClick.AddListener(() =>
        {
            DisableNetworkGameObject();
            NetworkManager.Singleton.StartHost();
        });
        serverBtn.onClick.AddListener(() =>
        {
            DisableNetworkGameObject();
            NetworkManager.Singleton.StartServer();
        });
        clientBtn.onClick.AddListener(() =>
        {
            DisableNetworkGameObject();
            NetworkManager.Singleton.StartClient();
        });
    }

    private void DisableNetworkGameObject()
    {
        networkUI.SetActive(false);
    }
}
