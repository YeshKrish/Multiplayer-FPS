using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private string weaponLayerName = "Weapon";

    [SerializeField]
    private Transform weaponHolder;

    [SerializeField]
    private PlayerWeapon primaryWeapon;

    private PlayerWeapon currentWeapon;

    private void Start()
    {
        EquipWeapon(primaryWeapon);
    }
    void EquipWeapon(PlayerWeapon _weapon)
    {
        currentWeapon = _weapon;

        GameObject _weaponInstance = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponInstance.transform.SetParent(weaponHolder);

        if (IsLocalPlayer)
        {
            _weaponInstance.layer = LayerMask.NameToLayer(weaponLayerName);
        }
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

}
