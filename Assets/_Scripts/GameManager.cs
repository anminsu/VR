using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameManager : Photon.PunBehaviour {

    #region Public Variables

    static public GameManager instance;
    [Tooltip("The player prefab used for representing the player")]
    public GameObject playerPrefab;

    #endregion

    #region Monobehavior Callbacks

    public void Start()
    {
        instance = this;
        if (playerPrefab == null)
        {
            Debug.LogError("Missing playerPrefab", this);
        } else {
            if (PlayerManager.localPlayerInstance==null)
            {
                Debug.Log("We are instantiating player from " + Application.loadedLevelName);
                // We are in a room.  Spawn a character for the local player. It gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            } else {
                Debug.Log("Ignoring Scene to load for " + Application.loadedLevelName);
            }
            
        }
    }

    #endregion

    #region Photon Messages

    public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerConnected() " + other.NickName); // Not seen if you're the player connecting

        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerConnected() isMasterClient = " + PhotonNetwork.isMasterClient);

            LoadArena();
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("OnPhotonPlayerDisconnected() " + otherPlayer.NickName); // See when another player disconnects

        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerDisconnected() isMasterClient = " + PhotonNetwork.isMasterClient);

            LoadArena();
        }
    }

    /// <summary>
    /// Called when the local player leaves the room
    /// </summary>
    public void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Private Methods

    private void LoadArena()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("PhotonNetwork: Trying to load level but not master client");
        } else {
            Debug.Log("PhotonNetwork: Loading Level: " + PhotonNetwork.room.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.room.PlayerCount);
        }
    }

    #endregion

}
