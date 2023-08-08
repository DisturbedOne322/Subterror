using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField, Header("Must implement IShootableWeapon")]
    private GameObject go;

    [SerializeField, Min(0)]
    private float recoilStrength;

    private IShootableWeapon weapon;


    // Start is called before the first frame update
    void Start()
    {
       TryGetComponent<IShootableWeapon>(out IShootableWeapon weapon);
       if(weapon != null)
           weapon.OnShoot += ShootableWeapon_OnShoot;
       
    }

    private void OnDestroy()
    {
        if (weapon != null)
            weapon.OnShoot -= ShootableWeapon_OnShoot;
    }

    private void ShootableWeapon_OnShoot()
    {
        transform.Rotate(new Vector3(0, 0, recoilStrength));
    }
}
