using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(WeaponManager))]
public class PlayerShoot : NetworkBehaviour
{
    private const string PLAYER_TAG = "Player"; 

    [SerializeField]
    private Camera cam; 

    [SerializeField]
    private LayerMask mask;

    private PlayerWeapon currentWeapon;
    private WeaponManager weaponManager; 

    void Start()
    {
        if (cam == null)
        {
            Debug.LogError("PlayerShoot: No camera referenced!"); 
            this.enabled = false; 
        }

        weaponManager = GetComponent<WeaponManager>(); 
    }

    void Update ()
    {
        currentWeapon = weaponManager.GetCurrentWeapon(); 

        if (PauseMenu.isOn)
            return; 

        if (currentWeapon.bullets < currentWeapon.maxBullets && currentWeapon.canReload)
        {
            if (Input.GetButtonDown("Reload"))
            {
                weaponManager.Reload(); 
                return; 
            }
        }
        
        if (currentWeapon.bullets > 0)
        {
            if (currentWeapon.fireRate <= 0f)
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    Shoot(); 
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    InvokeRepeating ("Shoot", 0f, 1f/currentWeapon.fireRate); 
                }
                if (Input.GetButtonUp("Fire1"))
                {
                    CancelInvoke ("Shoot"); 
                }
            }
        }
        else
        {
            CancelInvoke("Shoot");
        }
    }

    [Command]
    void CmdOnShoot ()
    {
        RpcDoShootEffect(); 
    }

    [ClientRpc]
    void RpcDoShootEffect ()
    {
        weaponManager.GetCurrentGraphics().muzzleFlash.Play(); 
    }
    
    [Command]
    void CmdOnHit (Vector3 _pos, Vector3 _normal)
    {
        RpcDoHitEffect(_pos, _normal); 
    }

    [ClientRpc]
    void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)
    {
        GameObject _hitEffect = (GameObject)Instantiate(weaponManager.GetCurrentGraphics().hitEffectPrefab, _pos, Quaternion.LookRotation(_normal)); 
        Destroy(_hitEffect, 2f); 
    }

    [Client]
    private void Shoot()
    {
        if (!isLocalPlayer || weaponManager.isReloading)
            return; 

        if (currentWeapon.bullets <= 0 && currentWeapon.canReload)
        {
            weaponManager.Reload(); 
            return; 
        }

        currentWeapon.bullets--; 

        Debug.Log("Remaining bullets " + currentWeapon.bullets); 

        CmdOnShoot(); 

        RaycastHit _hit; 
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, currentWeapon.range, mask))
        {
            if (_hit.collider.tag == PLAYER_TAG)
            {
                CmdPlayerShot(_hit.collider.name, currentWeapon.damage, transform.name); 
            }

            CmdOnHit(_hit.point, _hit.normal); 
        }

        if (currentWeapon.bullets <= 0 && currentWeapon.canReload)
        {
            weaponManager.Reload(); 
        }
    }

    [Command]
    private void CmdPlayerShot (string _playerID, int _damage, string _sourceId)
    {
        Debug.Log(_playerID + " has been shot");

        Player _player = GameManager.GetPlayer(_playerID); 
        _player.RpcTakeDamage(_damage, _sourceId);  
    }
}
