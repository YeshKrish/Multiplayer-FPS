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

    [SerializeField]
    string dontDrawLayerName = "DontDraw";

    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
<<<<<<< HEAD
    GameObject playerUI;
    [HideInInspector]
    public GameObject playerUIInstance;
=======
    GameObject crossHair;
    private GameObject playerUIInstance;
>>>>>>> parent of 43e3892 (Explosion effects)

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
            GetComponent<Player>().SetupPlayer();
        }

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

    //When we destroyed
    private void OnDisable()
    {
        Destroy(playerUIInstance);

        if (IsLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }
}
