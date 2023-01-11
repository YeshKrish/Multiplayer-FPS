using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Player))]
public class Player_setup : NetworkBehaviour
{

    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    Camera sceneCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsLocalPlayer)
        {
            DisableComponets();
            AssignRemoteLayer();
        }
        else
        {
            sceneCamera = Camera.main;
            if(sceneCamera != null) {
                sceneCamera.gameObject.SetActive(false);
            }
            
        }

        GetComponent<Player>().Setup();
        ListenChanges();
    }


    void ListenChanges()
    {
        string _netID = GetComponent<NetworkObject>().NetworkObjectId.ToString();

        Player _player = GetComponent<Player>();

        Debug.Log(_netID + " " + _player);

        GameManager.RegisterPlayer(_netID, _player);
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponets()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    private void OnDisable()
    {
        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }
}
