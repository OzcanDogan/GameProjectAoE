using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private byte maxPlayers = 2;

    private void Start()
    {
        Debug.Log("Connecting to Photon...");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master. Joining random room...");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("No random room found. Creating a new room...");

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayers;

        PhotonNetwork.CreateRoom(null, options);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room! Player count: " + PhotonNetwork.CurrentRoom.PlayerCount);

        // Eğer sahnede değilsek (örneğin main menu'deysek) oyunu yükleyebilirsin:
        // PhotonNetwork.LoadLevel("GameScene"); 
    }
}
