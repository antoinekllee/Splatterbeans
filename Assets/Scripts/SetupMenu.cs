using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class SetupMenu : MonoBehaviour
{
    [SerializeField]
    Text mapText; 

    [SerializeField]
    Text matchTimeText; 

    [SerializeField]
    Text respawnTimeText;

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        mapText.text = currentScene.name; 
        
        matchTimeText.text = (Mathf.Floor(GameManager.instance.matchSettings.matchTime/60)).ToString() + " minuites"; 

        respawnTimeText.text = GameManager.instance.matchSettings.respawnTime.ToString() + " seconds"; 
    }
}
