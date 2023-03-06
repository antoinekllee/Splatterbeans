using UnityEngine;
using System.Collections.Generic;
using System.Collections;  
using System.Linq; 
// using UnityEngine.Networking; 

// public class GameManager : NetworkBehaviour
public class GameManager : MonoBehaviour
{
    public static GameManager instance; 

    public MatchSettings matchSettings; 

    public float timeLeft; 
    public static string minutes; 
    public static string seconds; 

    public bool inGame = true; 

    [SerializeField]
    private GameObject sceneCamera; 

    public delegate void OnPlayerKilledCallback(string player, string source); 
    public OnPlayerKilledCallback onPlayerKilledCallback; 

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one GameManager in scene"); 
        }
        else
        {
            instance = this; 
        }

        timeLeft = matchSettings.matchTime;
    }

    public void StartCountdownTimer()
    {
        minutes = Mathf.Floor(timeLeft / 60).ToString("00");
        seconds = (timeLeft % 60).ToString("00");

        StartCoroutine(CountdownTimer());
    }

    IEnumerator CountdownTimer()
    {
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(1f);

            minutes = Mathf.Floor(timeLeft / 60).ToString("00");
            seconds = (timeLeft % 60).ToString("00");

            timeLeft -= 1;
        }
    }

    public void SetSceneCameraActive (bool isActive)
    {
        if (sceneCamera == null)
            return; 
        
        sceneCamera.SetActive(isActive); 
    }

    #region Player Tracking

    private const string PLAYER_ID_PREFIX = "Player"; 

    private static Dictionary <string, Player> players = new Dictionary<string, Player>(); 

    public static void RegisterPlayer(string _netID, Player _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID; 
        players.Add(_playerID, _player); 
        _player.transform.name = _playerID; 
    }

    public static void UnRegisterPlayer (string _playerID)
    {
        players.Remove(_playerID); 
    }

    public static Player GetPlayer(string _playerID)
    {
        return players [_playerID]; 
    }

    public static Player[] GetAllPlayers()
    {
        return players.Values.ToArray(); 
    }

    // void OnGUI ()
    // {
    //    GUILayout.BeginArea(new Rect(200, 200, 200, 500));
    //    GUILayout.BeginVertical();

    //    foreach (string _playerID in players.Keys)
    //    {
    //        GUILayout.Label(_playerID + "  -  " + players[_playerID].transform.name);
    //    }

    //    GUILayout.EndVertical();
    //    GUILayout.EndArea();
    // }

    #endregion
}
