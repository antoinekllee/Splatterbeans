using UnityEngine;
using UnityEngine.UI; 

public class PlayerScoreboardItem : MonoBehaviour
{
    [SerializeField]
    Text usernameText;

    [SerializeField]
    Text killsText;

    [SerializeField]
    Text deathsText;

    [SerializeField]
    Text KDRText; 

    // public void Setup (string username, int kills, int deaths, int KDR)
    public void SetupWithScores (string username, int kills, int deaths, float KDR)
    {
        usernameText.text = username; 
        killsText.text = kills.ToString(); 
        deathsText.text = deaths.ToString(); 
        KDRText.text = KDR.ToString("F1"); 
    }
    public void SetupUsernames(string username)
    {
        usernameText.text = username; 
    }

}
