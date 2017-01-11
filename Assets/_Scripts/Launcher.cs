using UnityEngine;
using System.Collections;

public class Launcher : Photon.PunBehaviour {

    #region Public Variables

    /// <summary>
    /// The PUN Loglevel
    /// </summary>
    public PhotonLogLevel logLevel = PhotonLogLevel.Informational;

    /// <summary>
    /// The maximum players per room.  When room is full, cannot be joined by new players.  A new room will be created.
    /// </summary>
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    public byte maxPlayersPerRoom = 4;

    [Tooltip("The UI Panel")]
    public GameObject controlPanel;

    [Tooltip("The label to let the user know the game is loading")]
    public GameObject progressLabel;

    #endregion

    #region Private Variables

    /// <summary>
    /// The clients version number.  Users are separated by their version number
    /// </summary>
    string _gameVersion = "1";

    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon, 
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;

    #endregion

    #region Monobehavior Callbacks

    void Awake()
    {
        // #Critical
        // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;

        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // #NotImportant
        // Force LogLevel
        PhotonNetwork.logLevel = logLevel; 
    }

    void Start()
    {
        // Connecting will be handled by the UI Button
        //Connect();

        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Start the connection process. 
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        isConnecting = true;

        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        // We check if we are connected or not. We join if we are, else we initiate the connection with the server.
        if (PhotonNetwork.connected)
        {
            // #Critical At this point we'll try connecting to a random room.  If it fails, we'll get notified on the OnPhotonRandomJoinFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        } else {
            // #Critical We must first and foremost connect to Photon Online Server
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    #endregion

    #region Photon.Punbehavior Callbacks

    public override void OnConnectedToMaster()
    {
        //base.OnConnectedToMaster();
        Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster was called by PUN");

        // If we aren't attempting to join the room, don't do anything
        if (!isConnecting) return;

        // #Critical: First thing we try to do is join a potential existing room.  If we do, good, if not we'll called back with OnPhotonRandomJoinFailed()
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnectedFromPhoton()
    {
        //base.OnDisconnectedFromPhoton();
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);

        Debug.Log("DemoAnimator/Launcher: OnDisconnectedFromPhoton was called by PUN");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        //base.OnPhotonRandomJoinFailed(codeAndMsg);

        Debug.Log("DemoAnimator/Launcher: OnPhotonRandomJoinFailed was called by PUN. No random room available so we create one.\nCalling: Photon.CreateRoom(null, new RoomOptions() {maxPlayers = maxPlayersPerRoom}, null);");

        // #Critical: we failed to join a random room, maybe none exist or they are all full. No worries we created a new one
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = maxPlayersPerRoom }, null);
    }

    public override void OnJoinedRoom()
    {
        //base.OnJoinedRoom();

        Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room");

        // #Critical: We only load if we are the first player, else we rely on PhotonNetwork.AutomaticallySyncScene to sync our instance scene
        if (PhotonNetwork.room.playerCount == 1)
        {
            Debug.Log("We load the 'Room for 1'");

            // #Critical: Load the room level
            PhotonNetwork.LoadLevel("Room for 1");
        }
    }

    #endregion
}
