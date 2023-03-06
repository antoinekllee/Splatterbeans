using UnityEngine;
using UnityEngine.UI; 

public class RespawnMenu : MonoBehaviour
{
    [SerializeField]
    Text punText; 

    [SerializeField]
    string[] puns; 

    int punIndex; 

    void OnEnable()
    {
        punIndex = Random.Range(0, puns.Length); 

        punText.text = puns[punIndex]; 
    }
}
