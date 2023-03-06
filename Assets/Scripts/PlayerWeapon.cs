using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
    public string name = "Pistol"; 

    public int damage = 10; 
    public float range = 100f; 

    public float fireRate = 0f; 

    public int maxBullets = 64; 
    [HideInInspector]
    public int bullets; 

    public float reloadTime = 1f; 

    public float scopedFOV = 30f;
    public float normalFOV = 60f; 
    public float scopeZoomSpeed = 10f;

    public bool canReload = false;
    public bool toggleScopeOverlay = false; 

    public GameObject graphics; 

    public PlayerWeapon ()
    {
        bullets = maxBullets; 
    }
}
