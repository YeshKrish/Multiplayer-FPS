using UnityEngine;
using Unity.Netcode;

public class PlayerShoot : NetworkBehaviour
{
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
        RaycastHit _hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, weapon.range, layerMask))
        {
            //We hit something
            Debug.Log("Player Shoot: We hit:" + _hit.collider.name);
        }
    }
}
