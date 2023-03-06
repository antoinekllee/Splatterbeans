using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw"; 
    [SerializeField]
    GameObject playerGraphics; 

    [SerializeField]
    GameObject playerUIPrefab; 
    [HideInInspector]
    public GameObject playerUIInstance; 

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {
            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName)); 
            playerUIInstance = Instantiate(playerUIPrefab); 
            playerUIInstance.name = playerUIPrefab.name; 

            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>(); 
            if (ui == null)
                Debug.LogError("No PlayerUI component on PlayerUI prefab"); 
            ui.SetPlayer (GetComponent<Player>());


            string _username = "Loading...";
            // if (UserAccountManager.IsLoggedIn && isLocalPlayer)
            //     _username = UserAccountManager.LoggedIn_Username;
            // else
            // {
            _username = transform.name;
            // }

            CmdSetUsername(transform.name, _username); // If function is not called, scoreboard usernames on the client are still set to blank
            
            
            GetComponent<Player>().SetupPlayer();
        }    

        // Debug.Log("Calling player setup"); 
        // GetComponent<Player>().SetupPlayer(); 
    }

    [Command]
    void CmdSetUsername(string playerID, string username)
    {
        Player player = GameManager.GetPlayer(playerID);
        if (player != null)
        {
            Debug.Log(username + " has joined!");
            player.username = username;
        }
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer; 

        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer); 
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient(); 

        string _netID = GetComponent<NetworkIdentity>().netId.ToString(); 
        Player _player = GetComponent<Player>(); 

        GameManager.RegisterPlayer(_netID, _player); 
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    void OnDisable()
    {
        Destroy (playerUIInstance); 

        if (isLocalPlayer)
            GameManager.instance.SetSceneCameraActive(true); 

        GameManager.UnRegisterPlayer(transform.name);  
    }
}














// using UnityEngine;
// using UnityEngine.Networking;

// [RequireComponent(typeof(Player))]
// [RequireComponent(typeof(PlayerController))]
// public class PlayerSetup : NetworkBehaviour
// {
//     [SerializeField]
//     Behaviour[] componentsToDisable;

//     [SerializeField]
//     string remoteLayerName = "RemotePlayer";

//     [SerializeField]
//     string dontDrawLayerName = "DontDraw";
//     [SerializeField]
//     GameObject playerGraphics;

//     [SerializeField]
//     GameObject playerUIPrefab;
//     [HideInInspector]
//     public GameObject playerUIInstance;

//     [SerializeField]
//     GameObject setupMenu;
//     [SerializeField]
//     GameObject settingsMenu;

//     void Start()
//     {
//         ShowSetupMenuOnHost();

//         if (!isLocalPlayer)
//         {
//             setupMenu.SetActive(false);
//         }
//     }

//     [Server]
//     void ShowSetupMenuOnHost()
//     {
//         settingsMenu.SetActive(true);
//     }

//     [Command]
//     public void CmdStartGame()
//     {
//         RpcSetupPlayers();
//     }

//     [ClientRpc]
//     void RpcSetupPlayers()
//     {
//         setupMenu.SetActive(false);

//         if (!isLocalPlayer)
//         {
//             DisableComponents();
//             AssignRemoteLayer();
//         }
//         else
//         {
//             SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));
//             playerUIInstance = Instantiate(playerUIPrefab);
//             playerUIInstance.name = playerUIPrefab.name;

//             PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
//             if (ui == null)
//                 Debug.LogError("No PlayerUI component on PlayerUI prefab");
//             ui.SetPlayer(GetComponent<Player>());

//             GetComponent<Player>().SetupPlayer();

//             string _username = "Loading...";
//             if (UserAccountManager.IsLoggedIn)
//                 _username = UserAccountManager.LoggedIn_Username;
//             else
//                 _username = transform.name;

//             CmdSetUsername(transform.name, _username); // If function is not called, scoreboard usernames on the client are still set to blank
//         }

//         // Debug.Log("Calling player setup"); 
//         // GetComponent<Player>().SetupPlayer(); 
//     }

//     [Command]
//     void CmdSetUsername(string playerID, string username)
//     {
//         Player player = GameManager.GetPlayer(playerID);
//         if (player != null)
//         {
//             Debug.Log(username + " has joined!");
//             player.username = username;
//         }
//     }

//     void SetLayerRecursively(GameObject obj, int newLayer)
//     {
//         obj.layer = newLayer;

//         foreach (Transform child in obj.transform)
//         {
//             SetLayerRecursively(child.gameObject, newLayer);
//         }
//     }

//     public override void OnStartClient()
//     {
//         base.OnStartClient();

//         string _netID = GetComponent<NetworkIdentity>().netId.ToString();
//         Player _player = GetComponent<Player>();

//         GameManager.RegisterPlayer(_netID, _player);
//     }

//     void AssignRemoteLayer()
//     {
//         gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
//     }

//     void DisableComponents()
//     {
//         for (int i = 0; i < componentsToDisable.Length; i++)
//         {
//             componentsToDisable[i].enabled = false;
//         }
//     }

//     void OnDisable()
//     {
//         Destroy(playerUIInstance);

//         if (isLocalPlayer)
//             GameManager.instance.SetSceneCameraActive(true);

//         GameManager.UnRegisterPlayer(transform.name);
//     }
// }


