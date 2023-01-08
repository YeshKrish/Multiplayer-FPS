using UnityEngine;
using Unity.Netcode;

public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    public PlayerWeapon weapon;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private LayerMask layerMask;

    void Start()
    {
        if(cam == null)
        {
            Debug.LogError("Player Shoot: No camera Referenced");
            this.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    void Shoot()
    {
        if (!IsClient)
        {
            return;
        }
        RaycastHit _hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.range, layerMask))
        {
            if (_hit.collider.tag == PLAYER_TAG)
            {
                PlayerShotServerRpc(_hit.collider.name);
            }
        }
    }

    [ServerRpc]
    void PlayerShotServerRpc(string _ID)
    {
        Debug.Log(_ID + "has been shot");  
    }
}
