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


    //Is called on server when a player shoots
    [ServerRpc]
    void OnShootServerRpc()
    {
        DoShootEffectsClientRpc();
    }


    //is called on all clients when we needs to do a shoot effect
    [ClientRpc]
    void DoShootEffectsClientRpc()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play();
    }

    //Is called on server when a bullet hits!
    //Takes in the hit point and normal of the surface
    [ServerRpc]
    void OnHitServerRpc(Vector3 _pos, Vector3 _normal)
    {
        DoHitEffectsClientRpc(_pos, _normal);
    }


    //is called on all clients when we bullet hits an object
    //We spawn cool effects here
    [ClientRpc]
    void DoHitEffectsClientRpc(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitEffectInstance = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal));
        Destroy(_hitEffectInstance, 2f);
    }

    void Shoot()
    {
        Debug.Log("Test Shooting");

        if (!IsClient && !IsLocalPlayer)
        {
            return;
        }

        //We are shooting, call the onShoot method on the server
        OnShootServerRpc();
        RaycastHit _hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, layerMask))
        {
          
            if (_hit.collider.tag == PLAYER_TAG)
            {
                PlayerShotServerRpc(_hit.collider.name, currentWeapon.damage);
            }

            Debug.Log("Object I hit:" + _hit.collider.name);
            //Wer hit something call the OnHit Method on server
            OnHitServerRpc(_hit.point, _hit.normal);
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
