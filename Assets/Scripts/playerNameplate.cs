using UnityEngine;
using UnityEngine.UI; 

public class playerNameplate : MonoBehaviour
{
    [SerializeField]
    private Text usernameText; 

    [SerializeField]
    private RectTransform healthBarFill; 

    [SerializeField]
    private Player player; 

    void Update ()
    {
        usernameText.text = player.username; 
        healthBarFill.localScale = new Vector3 (player.GetHealthPercentage(), 1f, 1f); 
    }
}
