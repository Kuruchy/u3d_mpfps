using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;

public class RoomListItem : MonoBehaviour {

    private MatchInfoSnapshot match;

    // Create a delegate callback for the functions to subscribe to
    public delegate void JoinRoomDelegate(MatchInfoSnapshot _match);

    public JoinRoomDelegate joinRoomCallback;

    [SerializeField]
    private Text roomNameText;


    // Setup accepts a callback function
    public void Setup(MatchInfoSnapshot _match, JoinRoomDelegate _joinRoomCallback)
    {
        match = _match;
        joinRoomCallback = _joinRoomCallback;
        roomNameText.text = match.name + " (" + match.currentSize + "/" + match.maxSize + ")";
    }

    public void JoinRoom()
    {
        // invoke the callback function with parameter match
        joinRoomCallback.Invoke(match);
    }
}
