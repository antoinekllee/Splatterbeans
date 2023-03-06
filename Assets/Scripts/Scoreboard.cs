using UnityEngine;
using System.Collections;

public class Scoreboard : MonoBehaviour
{
    [SerializeField]
    GameObject playerScoreboardItem = null; 
    [SerializeField]
    GameObject playerListItem = null; 

    [SerializeField]
    Transform playerScoreboardList; 

    [SerializeField]
    bool showScores = true; 

    [SerializeField]
    bool automaticallyUpdateScores = true; 

    void Start()
    {
        RefreshScoreboard(); 
    }

    void OnEnable() 
    {
        Player[] players = GameManager.GetAllPlayers(); 

        foreach (Player player in players)
        {
            if (showScores)
            {
                GameObject itemGameObject = (GameObject)Instantiate (playerScoreboardItem, playerScoreboardList); 
                // itemGameObject.transform.SetSiblingIndex(0); 

                PlayerScoreboardItem item = itemGameObject.GetComponent<PlayerScoreboardItem>(); 
                if (item != null)
                {
                    // Debug.Log(player.transform.name + " - " + player.username); 
                    float kills = player.kills; 
                    float deaths = player.deaths; 
                    float KDR = kills; 
                    
                    if (player.deaths != 0f)
                        KDR = kills/deaths; 
                    
                    item.SetupWithScores (player.username, player.kills, player.deaths, KDR); 
                }   
            }
            else
            {
                GameObject itemGameObject = (GameObject)Instantiate(playerListItem, playerScoreboardList);

                PlayerScoreboardItem item = itemGameObject.GetComponent<PlayerScoreboardItem>();
                if (item != null)
                {
                    // Debug.Log(player.transform.name + " - " + player.username);
                    item.SetupUsernames(player.username);
                }
            }
        }
    }

    void OrderScoreboardItems()
    {
        
    }

    void OnDisable() 
    {
        foreach (Transform child in playerScoreboardList)
        {
            Destroy (child.gameObject); 
        }
    }

    void Update()
    {
        // if (automaticallyUpdateScores)
        StartCoroutine(CheckForUpdate(4f));
    }
    IEnumerator CheckForUpdate(float refreshRate)
    {
        Player[] oldPlayers = GameManager.GetAllPlayers();
        yield return new WaitForSeconds(refreshRate);
        Player[] newPlayers = GameManager.GetAllPlayers();
        if (oldPlayers != newPlayers)
        {
            RefreshScoreboard();
        }
    }

    private void RefreshScoreboard()
    {
        OnDisable();
        OnEnable();
    }
}
