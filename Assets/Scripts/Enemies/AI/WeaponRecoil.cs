using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [SerializeField, Header("Must implement IShootableWeapon")]
    private GameObject go;

    [SerializeField, Min(0)]
    private float recoilStrength;


    // Start is called before the first frame update
    void Start()
    {
       if(go.TryGetComponent<IShootableWeapon>(out IShootableWeapon shootableWeapon))
       {
            shootableWeapon.OnShoot += ShootableWeapon_OnShoot;
       }
    }

    private void OnDestroy()
    {
        if (go.TryGetComponent<IShootableWeapon>(out IShootableWeapon shootableWeapon))
        {
            shootableWeapon.OnShoot -= ShootableWeapon_OnShoot;
        }
    }

    private void ShootableWeapon_OnShoot()
    {
        transform.Rotate(new Vector3(0, 0, recoilStrength));
    }
}
