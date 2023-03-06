using UnityEngine;
using UnityEngine.UI; 
using System.Collections;  

public class PlayerStats : MonoBehaviour
{
    public Text experienceText; 
    public Text killCount; 
    public Text deathCount; 
    public Text KDRText; 

    void Start ()
    {
        if (UserAccountManager.IsLoggedIn)
        {
            UserAccountManager.instance.GetData(OnReceivedData); 
        }
    }

    void OnReceivedData(string data)
    {
        if (killCount == null || deathCount == null)
            return; 
        
        float kills = DataTranslator.DataToKills(data); 
        float deaths = DataTranslator.DataToDeaths(data); 
        float KDR = kills; 

        float experience = kills * 100; 

        if (deaths != 0f)
            KDR = kills/deaths; 
            
        experienceText.text = "Experience: " + experience.ToString(); 
        killCount.text = "Kills: " + kills.ToString();
        deathCount.text = "Deaths: " + deaths.ToString();
        KDRText.text = "KDR: " + KDR.ToString("F1"); 
    }
}
