
using UnityEngine;
using System.Collections;

public class Killfeed : MonoBehaviour 
{
	[SerializeField]
	GameObject killfeedItemPrefab;

	void Start () 
    {
		GameManager.instance.onPlayerKilledCallback += OnKill;
	}

	public void OnKill (string player, string source)
	{
		GameObject gameObject = (GameObject)Instantiate(killfeedItemPrefab, transform);
        gameObject.transform.SetAsFirstSibling(); 
		gameObject.GetComponent<KillfeedItem>().Setup(player, source);

        // Destroy(gameObject, 5f); 
		Destroy(gameObject, 7.5f); 
	}

}