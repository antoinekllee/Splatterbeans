using UnityEngine;
using UnityEngine.Networking; 
using System.Collections; 
using UnityEngine.UI; 

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour
{
    private bool _isDead = false; 
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private static int maxHealth = 100; 

    private int currentHealth = maxHealth; 

    public float GetHealthPercentage()
    {
        return (float)currentHealth/maxHealth; 
    }

    // [SyncVar]
    public string username = "Loading...";  

    public int kills;
    public int deaths; 

    [SerializeField]
    private Behaviour[] disableOnDeath; 
    private bool[] wasEnabled; 

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath; 

    [SerializeField]
    private GameObject deathEffect; 
    [SerializeField]
    private GameObject spawnEffect; 

    private bool firstSetup = true; 

    [SerializeField]
    private GameObject setupMenu; 
    [SerializeField]
    private GameObject StartGameButton; 

    [SerializeField]
    private GameObject respawnMenu; 
    [SerializeField]
    private Text respawnCountdownText;

    [SerializeField]
    private GameObject endGameMenu;

    // void Start()
    // {
    //     if (!isLocalPlayer)
    //         username = transform.name; 
    // }

    public void StartGame()
    {
        CmdStartGame(); 
    }

    [Command]
    private void CmdStartGame()
    {
        RpcStartGame(); 
    }
    [ClientRpc]
    private void RpcStartGame()
    {
        SetDefaults();

        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);

            GameManager.instance.timeLeft = GameManager.instance.matchSettings.matchTime;
            GameManager.instance.StartCountdownTimer();
            
            setupMenu.SetActive(false);
        }
    }

    public void SetupPlayer()
    {
        if (isLocalPlayer) 
        {
            GameManager.instance.SetSceneCameraActive(false);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
        }

        CmdBroadcastNewPlayerSetup(); 
    }

    [Command]
    private void CmdBroadcastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients(); 
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (firstSetup)
        {
            if (isLocalPlayer)
                currentHealth = maxHealth; 
            
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }

            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableOnDeath[i].enabled = false;
            }

            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableGameObjectsOnDeath[i].SetActive(false);
            }

            if (isLocalPlayer)
            {
                GameManager.instance.SetSceneCameraActive(true);
                GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
            }

            if (isLocalPlayer)
                setupMenu.SetActive(true); 
            
            // if (isServer)        
                StartGameButton.SetActive(true);

            username = transform.name;

            firstSetup = false;

            // Set Defaults

            // isDead = false;

            // currentHealth = maxHealth;

            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableOnDeath[i].enabled = wasEnabled[i];
            }

            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableGameObjectsOnDeath[i].SetActive(true);
            }

            // Collider _col = GetComponent<Collider>();
            // if (_col != null)
            //     _col.enabled = true;

            // GameObject _gfxInstance = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
            // Destroy(_gfxInstance, 3f);

            // Reset attributes

            // for (int i = 0; i < disableOnDeath.Length; i++)
            // {
            //     disableGameObjectsOnDeath[i].SetActive(false);
            // }
        }
        else
        {
            SetDefaults(); // NOT SETTING DEFAULTS SO PLAYER DOESN'T RESPAWN ON BOTH CLIENTS
        }   
    }

    void Update ()
    {   
        if (!isLocalPlayer)
            return;

        if (GameManager.instance.timeLeft <= 0)
            EndGame();

        if (UserAccountManager.IsLoggedIn)
        {
            if (username != UserAccountManager.LoggedIn_Username)
                username = UserAccountManager.LoggedIn_Username; 
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            RpcTakeDamage(40, transform.name); 
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(int _amount, string _sourceId)
    {
        if (isDead)
            return; 

        currentHealth -= _amount; 

        if (currentHealth <= 0)
        {
            Die(_sourceId); 
        }

        Debug.Log(transform.name + " now has " + currentHealth); 
    }

    private void Die(string _sourceId)
    {
        isDead = true; 

        Player sourcePlayer = GameManager.GetPlayer(_sourceId); 
        if (sourcePlayer != null)
        {
            sourcePlayer.kills++; 
            GameManager.instance.onPlayerKilledCallback.Invoke(username, sourcePlayer.username);
        }

        deaths++;

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false; 
        }

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false); 
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        GameObject _gfxInstance = (GameObject)Instantiate(deathEffect, transform.position, Quaternion.identity); 
        Destroy (_gfxInstance, 3f); 

        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false); 
        }

        Debug.Log(transform.name + " is dead"); 

        StartCoroutine(Respawn()); 
    }

    private IEnumerator Respawn()
    {
        if (isLocalPlayer)
            respawnMenu.SetActive(true);

        StartCoroutine(CountdownRespawnTimer());
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        if (isLocalPlayer)
            respawnMenu.SetActive(false); 

        yield return new WaitForSeconds(0.1f); 

        SetupPlayer();

        Debug.Log(transform.name + " respawned"); 
    }

    private IEnumerator CountdownRespawnTimer()
    {
        float respawnCountdown = GameManager.instance.matchSettings.respawnTime;

        while (respawnCountdown > 0)
        {
            respawnCountdownText.text = "Respawning in " + respawnCountdown;
            yield return new WaitForSeconds(1f);
            respawnCountdown--; 
        }

        if (respawnCountdown <= 0)
        {
            respawnCountdownText.text = "Respawning"; 
        }
    }

    public void SetDefaults()
    {
        isDead = false; 

        currentHealth = maxHealth; 

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i]; 
        }

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }

        Collider _col = GetComponent<Collider>(); 
        if (_col != null)
            _col.enabled = true;

        GameObject _gfxInstance = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_gfxInstance, 3f);
    }

    public void EndGame()
    {
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        respawnMenu.SetActive(false); 
        endGameMenu.SetActive(true); 
    }
}
