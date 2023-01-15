using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class Player_setup : NetworkBehaviour
{

    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    Camera sceneCamera;

    [SerializeField]
    string dontDrawLayerName = "DontDraw";

    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject crossHair;
    private GameObject playerUIInstance;

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

            //Disable Player Graphics for local player
            Util.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //Instatntiate PlayerUI
            playerUIInstance =  Instantiate(crossHair);
            playerUIInstance.name = crossHair.name;

            //Configure PlayerUI
            PlayerUI uI = playerUIInstance.GetComponent<PlayerUI>();
            if (uI == null)
                Debug.LogError("No PlayerUI component on PlayerUI prefab");
            uI.SetController(GetComponent<PlayerController>());
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
            Debug.Log("Components To Disable:" + componentsToDisable[i].name);
            componentsToDisable[i].enabled = false;
        }
    }

    private void OnDisable()
    {
        Destroy(playerUIInstance);

        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }


        GameManager.UnRegisterPlayer(transform.name);
    }
}
