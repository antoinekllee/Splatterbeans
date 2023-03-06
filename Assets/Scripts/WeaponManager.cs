using UnityEngine; 
using UnityEngine.Networking;
using System.Collections; 
using System.Collections.Generic;

public class WeaponManager : NetworkBehaviour
{
    [SerializeField]
    private string weaponLayerName = "Weapon"; 

    [SerializeField]
    private Transform weaponHolder; 

    [SerializeField]
    private PlayerWeapon defaultWeapon; 
    private PlayerWeapon currentWeapon; 
    private WeaponGraphics currentGraphics;
    private List<PlayerWeapon> inventory = new List<PlayerWeapon>(); 
    private List<WeaponGraphics> weaponGraphicsList = new List<WeaponGraphics>();
    private GameObject weaponInstance; 
    private int currentWeaponSlot = -1;
    private int nextWeaponSlot; 

    public bool isReloading = false; 
    public bool isScoped = false;

    public GameObject weaponCamera; 
    Camera mainCamera; 

    void Start ()
    {
        mainCamera = Camera.main;
        SetupWeapon(defaultWeapon); 
    }

    public PlayerWeapon GetCurrentWeapon()
    {
        return currentWeapon; 
    }

    public WeaponGraphics GetCurrentGraphics()
    {
        return currentGraphics; 
    }

    public void SetupWeapon(PlayerWeapon _weapon)
    {
        foreach (PlayerWeapon weapon in inventory)
        {
            if (weapon.name == _weapon.name)
            {
                weapon.bullets = weapon.maxBullets; 
                return;
            }
        }

        inventory.Add(_weapon);

        if (weaponInstance != null)
            weaponHolder.transform.GetChild(currentWeaponSlot).gameObject.SetActive(false);

        currentWeapon = _weapon;
        currentWeapon.bullets = currentWeapon.maxBullets; 

        weaponInstance = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        weaponInstance.transform.SetParent(weaponHolder);

        currentGraphics = weaponInstance.GetComponent<WeaponGraphics>();
        if (currentGraphics == null)
            Debug.Log("No Weapon Graphics Component On the Object: " + weaponInstance.name);
        
        currentWeaponSlot += 1;
        
        if (isLocalPlayer)
            Util.SetLayerRecursively(weaponInstance, LayerMask.NameToLayer(weaponLayerName));

        weaponGraphicsList.Add(weaponInstance.GetComponent<WeaponGraphics>());
    }

    [Command]
    public void CmdSwitchWeapon(int _requestedSlot)
    {
        RpcSwitchWeapon(_requestedSlot);
    }

    [ClientRpc]
    public void RpcSwitchWeapon(int _requestedSlot)
    {
        if ((inventory.Count - 1) < _requestedSlot)
        {
            Debug.Log("No Weapon Found In Slot!");
            return;
        }
        weaponHolder.transform.GetChild(currentWeaponSlot).gameObject.SetActive(false);
        weaponHolder.transform.GetChild(_requestedSlot).gameObject.SetActive(true);
        currentWeaponSlot = _requestedSlot;

        if (isLocalPlayer)
        {
            currentWeapon = inventory[_requestedSlot];
        }

        currentGraphics = weaponGraphicsList[currentWeaponSlot]; 
    }

    public void Reload()
    {
        if (isReloading)
            return; 
        
        StartCoroutine(ReloadCoroutine()); 
    }

    private IEnumerator ReloadCoroutine()
    {
        Debug.Log ("Reloading"); 

        isReloading = true; 

        CmdOnReload(); 

        yield return new WaitForSeconds(currentWeapon.reloadTime); 

        currentWeapon.bullets = currentWeapon.maxBullets; 

        isReloading = false; 
    }

    [Command]
    void CmdOnReload()
    {
        RpcOnReload(); 
    }

    [ClientRpc]
    void RpcOnReload()
    {
        Animator anim = currentGraphics.GetComponent<Animator>(); 
        if (anim != null)
        {
            anim.SetTrigger("Reload"); 
        }
    }

    private void Update() 
    {
        if (!isLocalPlayer || PauseMenu.isOn)
            return; 

        if (isReloading)
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, currentWeapon.normalFOV, Time.deltaTime * currentWeapon.scopeZoomSpeed);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (!isScoped)
            {
                isScoped = true;

                Animator anim = currentGraphics.GetComponent<Animator>();
                if (anim != null)
                    anim.SetBool("Scoped", isScoped);
                
                if (!isReloading)
                {
                    if (currentWeapon.toggleScopeOverlay)
                        StartCoroutine(ToggleScopeOverlayOn());
                }
            }
            
            if (!isReloading)
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, currentWeapon.scopedFOV, Time.deltaTime * currentWeapon.scopeZoomSpeed);
            
            if (currentWeapon.toggleScopeOverlay && isReloading)
                ToggleScopeOverlayOff();
        }
        else
        {
            if (isScoped)
            {
                isScoped = false;

                Animator anim = currentGraphics.GetComponent<Animator>();
                if (anim != null)
                    anim.SetBool("Scoped", isScoped);

                if (currentWeapon.toggleScopeOverlay)
                    ToggleScopeOverlayOff();
            }

            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, currentWeapon.normalFOV, Time.deltaTime * currentWeapon.scopeZoomSpeed);
        }
        
        if (isReloading || isScoped)
            return; 

        if (Input.GetKeyDown(KeyCode.E))
        {
            nextWeaponSlot = currentWeaponSlot + 1; 

            if (nextWeaponSlot > inventory.Count - 1)
            {
                nextWeaponSlot = 0; 
            }

            CmdSwitchWeapon(nextWeaponSlot); 

            GetComponent<PlayerShoot>().CancelInvoke("Shoot"); 
        }

        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CmdSwitchWeapon(0);
            GetComponent<PlayerShoot>().CancelInvoke("Shoot");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CmdSwitchWeapon(1);
            GetComponent<PlayerShoot>().CancelInvoke("Shoot"); 
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CmdSwitchWeapon(2);
            GetComponent<PlayerShoot>().CancelInvoke("Shoot");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            CmdSwitchWeapon(3);
            GetComponent<PlayerShoot>().CancelInvoke("Shoot");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            CmdSwitchWeapon(4);
            GetComponent<PlayerShoot>().CancelInvoke("Shoot");
        }
    }

    IEnumerator ToggleScopeOverlayOn()
    {
        yield return new WaitForSeconds(0.2f); 

        GameObject playerUI = GetComponent<PlayerSetup>().playerUIInstance;
        playerUI.GetComponent<PlayerUI>().scopeOverlay.SetActive(true);

        weaponCamera.SetActive(false);
    }

    private void ToggleScopeOverlayOff()
    {
        GameObject playerUI = GetComponent<PlayerSetup>().playerUIInstance;
        playerUI.GetComponent<PlayerUI>().scopeOverlay.SetActive(false);

        weaponCamera.SetActive(true);
    }
}






































// using UnityEngine; 
// using UnityEngine.Networking;
// using System.Collections; 

// public class WeaponManager : NetworkBehaviour
// {
//     [SerializeField]
//     private string weaponLayerName = "Weapon";

//     [SerializeField]
//     private Transform weaponHolder;

//     [SerializeField]
//     private PlayerWeapon primaryWeapon;
//     private GameObject primaryWeaponObject;

//     [SerializeField]
//     private PlayerWeapon secondaryWeapon;
//     private GameObject secondaryWeaponObject;

//     bool firstWeaponSwap = true;

//     bool primaryWeaponEquipped = true;

//     private PlayerWeapon currentWeapon;

//     private WeaponGraphics currentGraphics;

//     public bool isReloading = false;
//     public bool isScoped = false;

//     public GameObject weaponCamera;
//     Camera mainCamera;

//     void Start()
//     {
//         primaryWeaponObject = SetupWeapon(primaryWeapon);
//         primaryWeapon.bullets = primaryWeapon.maxBullets;

//         EquipWeapon(primaryWeapon);
//         mainCamera = Camera.main;
//     }

//     public PlayerWeapon GetCurrentWeapon()
//     {
//         return currentWeapon;
//     }

//     public WeaponGraphics GetCurrentGraphics()
//     {
//         return currentGraphics;
//     }

//     void EquipWeapon(PlayerWeapon _weapon)
//     {
//         currentWeapon = _weapon;

//         if (primaryWeaponEquipped && !firstWeaponSwap)
//             currentGraphics = secondaryWeaponObject.GetComponent<WeaponGraphics>();
//         else
//             currentGraphics = primaryWeaponObject.GetComponent<WeaponGraphics>();
//     }

//     GameObject SetupWeapon(PlayerWeapon _weapon)
//     {
//         GameObject _weaponInstance = (GameObject)Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
//         _weaponInstance.transform.SetParent(weaponHolder.transform);

//         if (isLocalPlayer)
//             Util.SetLayerRecursively(_weaponInstance, LayerMask.NameToLayer(weaponLayerName));

//         return _weaponInstance;
//     }

//     void SwitchWeapon()
//     {
//         primaryWeaponObject.SetActive(!primaryWeaponObject.activeSelf);

//         if (firstWeaponSwap)
//         {
//             secondaryWeaponObject = SetupWeapon(secondaryWeapon);
//             secondaryWeapon.bullets = secondaryWeapon.maxBullets;
//             firstWeaponSwap = false;
//         }
//         else
//         {
//             secondaryWeaponObject.SetActive(!secondaryWeaponObject.activeSelf);
//         }

//         if (primaryWeaponEquipped)
//         {
//             EquipWeapon(secondaryWeapon);
//             primaryWeaponEquipped = false;
//         }
//         else
//         {
//             EquipWeapon(primaryWeapon);
//             primaryWeaponEquipped = true;
//         }

//         gameObject.GetComponent<PlayerShoot>().CancelInvoke("Shoot");
//     }

//     public void Reload()
//     {
//         if (isReloading)
//             return;

//         StartCoroutine(ReloadCoroutine());
//     }
//     private IEnumerator ReloadCoroutine()
//     {
//         isReloading = true;

//         CmdOnReload();

//         yield return new WaitForSeconds(currentWeapon.reloadTime);

//         currentWeapon.bullets = currentWeapon.maxBullets;

//         isReloading = false;
//     }
//     [Command]
//     void CmdOnReload()
//     {
//         RpcOnReload();
//     }
//     [ClientRpc]
//     void RpcOnReload()
//     {
//         Animator anim = currentGraphics.GetComponent<Animator>();
//         if (anim != null)
//         {
//             anim.SetTrigger("Reload");
//         }
//     }
//     private void Update()
//     {
//         if (!isLocalPlayer || PauseMenu.isOn)
//             return;

//         if (isReloading)
//             mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, currentWeapon.normalFOV, Time.deltaTime * currentWeapon.scopeZoomSpeed);

//         if (Input.GetKey(KeyCode.LeftShift))
//         {
//             if (!isScoped)
//             {
//                 isScoped = true;

//                 Animator anim = currentGraphics.GetComponent<Animator>();
//                 if (anim != null)
//                     anim.SetBool("Scoped", isScoped);

//             }

//             if (!isReloading)
//                 mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, currentWeapon.scopedFOV, Time.deltaTime * currentWeapon.scopeZoomSpeed);
//         }
//         else
//         {
//             if (isScoped)
//             {
//                 isScoped = false;

//                 Animator anim = currentGraphics.GetComponent<Animator>();
//                 if (anim != null)
//                     anim.SetBool("Scoped", isScoped);
//             }

//             mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, currentWeapon.normalFOV, Time.deltaTime * currentWeapon.scopeZoomSpeed);
//         }

//         if (Input.GetKeyDown(KeyCode.E))
//         {
//             if (!isReloading && !isScoped && !PauseMenu.isOn)
//                 SwitchWeapon();
//         }

//         if (Input.GetKeyDown(KeyCode.Q))
//         {
//             // Launch grenade
//         }
//     }
// }
