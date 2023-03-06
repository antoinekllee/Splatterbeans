using UnityEngine;
using System.Collections;

public class WeaponPickup : MonoBehaviour
{
    public PlayerWeapon newWeapon;

    [SerializeField]
    float respawnTime = 10f;
    [SerializeField]
    public float rotateSpeed = 2f;

    private bool collected = false;

    [SerializeField]
    private GameObject graphics;

    void Update()
    {
        graphics.transform.Rotate(0, rotateSpeed, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !collected)
        {
            collected = true;
            other.GetComponent<WeaponManager>().SetupWeapon(newWeapon);
            graphics.SetActive(false);
            GetComponent<Collider>().enabled = false;
            StartCoroutine(ResetPickUp()); 
        }
    }
    IEnumerator ResetPickUp()
    {
        collected = false;
        yield return new WaitForSeconds(respawnTime); 
        graphics.SetActive(true);
        GetComponent<Collider>().enabled = true;
    }
}