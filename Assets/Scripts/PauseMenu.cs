using UnityEngine; 
using UnityEngine.Networking; 
using UnityEngine.Networking.Match; 
using UnityEngine.UI; 

public class PauseMenu : MonoBehaviour
{
    public static bool isOn = false; 

    private NetworkManager networkManager;

    [SerializeField]
    Slider qualitySlider;
    
    [SerializeField]
    Text screenSizeText;
    private bool isFullscreen = false; 

    void Start ()
    {
        networkManager = NetworkManager.singleton; 
    }

    void Update() 
    {
        if (Screen.fullScreen)
        {
            screenSizeText.text = "Fullscreen";
        }
        else
        {
            screenSizeText.text = "Windowed";
        }

        QualitySettings.SetQualityLevel((int)qualitySlider.value, true);
    }

    public void ToggleScreenSize()
    {
        if (isFullscreen)
        {
            Screen.SetResolution(1400, 900, false);
            isFullscreen = false; 
        }
        else
        {
            Screen.SetResolution(1400, 900, true);
            isFullscreen = true; 
        }
    }

    public void LeaveRoom()
    {
        MatchInfo matchInfo = networkManager.matchInfo; 
        networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection); 
        networkManager.StopHost(); 
    }
}
