using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player";

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager;

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
        
        weaponManager = GetComponent<WeaponManager>();
    }

    // Update is called once per frame
    void Update()
    {
        currentWeapon = weaponManager.GetCurrentWeapon();

        if(currentWeapon.fireRate <= 0)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            //Automatic weapon setup
            if (Input.GetButtonDown("Fire1"))
            {
                InvokeRepeating("Shoot", 0f, 1f / currentWeapon.fireRate);
            }
            else if (Input.GetButtonUp("Fire1"))
            {
                CancelInvoke("Shoot");
            }
        }
    }

    void Shoot()
    {
        Debug.Log("Test Shooting");

        if (!IsClient && !IsLocalPlayer)
        {
            return;
        }
        RaycastHit _hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, layerMask))
        {
          
            if (_hit.collider.tag == PLAYER_TAG)
            {
                PlayerShotServerRpc(_hit.collider.name, currentWeapon.damage);
            }
        }
    }

    [ServerRpc]
    void PlayerShotServerRpc(string _playerID, int _damage)
    {
        Debug.Log(_playerID + "has been shot");

        Player _player =  GameManager.GetPlayer(_playerID);

        _player.TakeDamageClientRpc(_damage);
    }
}
