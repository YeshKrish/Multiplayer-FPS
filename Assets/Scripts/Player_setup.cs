using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

[RequireComponent(typeof(Player))]
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
    private GameObject crossHairInstance;

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
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //Instatntiate PlayerUI
            crossHairInstance =  Instantiate(crossHair);
            crossHairInstance.name = crossHair.name;
        }

        GetComponent<Player>().Setup();
        ListenChanges();
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach(Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
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
        Destroy(crossHairInstance);

        if(sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }


        GameManager.UnRegisterPlayer(transform.name);
    }
}
