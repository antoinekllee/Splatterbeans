using UnityEngine; 
using UnityEngine.UI; 
using System.Collections.Generic; 
using UnityEngine.Networking; 
using UnityEngine.Networking.Match; 
using System.Collections; 

public class JoinGame : MonoBehaviour
{
    List<GameObject> roomList = new List<GameObject>(); 

    [SerializeField]
    private Text status; 

    [SerializeField]
    private GameObject roomListItemPrefab; 

    [SerializeField]
    private Transform roomListParent; 

    private NetworkManager networkManager; 

    void Start () 
    {
        networkManager = NetworkManager.singleton; 

        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker(); 
        }

        RefreshRoomList(); 
    }

    public void RefreshRoomList()
    {
        ClearRoomList();
        if (networkManager.matchMaker == null)
        {
            networkManager.StartMatchMaker(); 
        }
        
        networkManager.matchMaker.ListMatches(0, 20, "", true, 0, 0, OnMatchList); 
        status.text = "Loading..."; 
    }

    public void OnMatchList (bool sucess, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        status.text = "";
         
        if (!sucess || matchList == null)
        {
            status.text = "Couldn't get room list"; 
            return; 
        }
        
        foreach (MatchInfoSnapshot match in matchList)
        {
            GameObject _roomListItemGameObject = Instantiate(roomListItemPrefab); 
            _roomListItemGameObject.transform.SetParent(roomListParent); 

            RoomListItem _roomListItem = _roomListItemGameObject.GetComponent<RoomListItem>(); 
            if (_roomListItem != null)
            {
                _roomListItem.Setup(match, JoinRoom); 
            }
            
            roomList.Add(_roomListItemGameObject); 

            _roomListItemGameObject.transform.localPosition = Vector3.zero; // reset the position of the room list item because of the screen space - camera canvas
            _roomListItemGameObject.transform.localScale = Vector3.one; // reset the scale of the room list item  because of the screen space - camera canvas
        }

        if (roomList.Count == 0)
        {
            status.text = "No rooms at the moment"; 
        }
    }

    void ClearRoomList()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]); 
        }

        roomList.Clear(); 
    }

    public void JoinRoom (MatchInfoSnapshot _match)
    {
        networkManager.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkManager.OnMatchJoined); 
        StartCoroutine(WaitForJoin()); 
    }
    
    IEnumerator WaitForJoin()
    {
        ClearRoomList(); 

        int countdown = 10; 
        while (countdown > 0)
        {
            status.text = "Joining... (" + countdown + ")"; 

            yield return new WaitForSeconds(1); 

            countdown--; 
        }

        status.text = "Failed to connect"; 
        yield return new WaitForSeconds(1); 

        MatchInfo matchInfo = networkManager.matchInfo; 
        if (matchInfo != null)
        {
            networkManager.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkManager.OnDropConnection); 
            networkManager.StopHost(); 
        }

        RefreshRoomList(); 
    }
}
