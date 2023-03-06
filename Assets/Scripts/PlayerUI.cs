using UnityEngine;
using UnityEngine.UI; 

public class PlayerUI : MonoBehaviour
{
    [SerializeField]
    RectTransform thrusterFuelFill; 

    [SerializeField]
    RectTransform healthBarFill; 

    [SerializeField]
    Text ammoText; 

    [SerializeField]
    GameObject pauseMenu; 

    [SerializeField]
    GameObject pauseMenuButtons; 

    [SerializeField]
    GameObject settingsMenu; 

    [SerializeField]
    GameObject scoreboard; 

    public GameObject scopeOverlay; 

    [SerializeField]
    Text countdownText; 

    private Player player; 
    private PlayerController controller; 
    private WeaponManager weaponManager; 

    public void SetPlayer(Player _player)
    {
        player = _player; 
        controller = player.GetComponent<PlayerController>(); 
        weaponManager = player.GetComponent<WeaponManager>(); 
    }

    void Start ()
    {
        PauseMenu.isOn = false; 
        pauseMenuButtons.SetActive(true); 
        settingsMenu.SetActive(false); 
    }

    void Update ()
    {
        SetFuelAmount (controller.GetThrusterFuelAmount()); 
        SetHealthAmount (player.GetHealthPercentage()); 
        SetAmmoAmount(weaponManager.GetCurrentWeapon().bullets); 

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu(); 
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            scoreboard.SetActive (true); 
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            scoreboard.SetActive (false); 
        }

        countdownText.text = GameManager.minutes + ":" + GameManager.seconds; 
    }

    public void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.isOn = pauseMenu.activeSelf;

        pauseMenuButtons.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void ToggleSettingsMenu()
    {
        pauseMenuButtons.SetActive(!pauseMenuButtons.activeSelf);
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }

    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3 (1f, _amount, 1f); 
    }

    void SetHealthAmount (float _amount)
    {
        healthBarFill.localScale = new Vector3 (1f, _amount, 1f); 
    }

    void SetAmmoAmount (int _amount)
    {
        ammoText.text = _amount.ToString() + "/" + weaponManager.GetCurrentWeapon().maxBullets; 
    }
}
