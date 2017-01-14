using UnityEngine;
using System.Collections;

public class Player : Photon.PunBehaviour {

    #region Monobehaviors

    public void Start()
    {
        SpawnMe();
    }

    #endregion

    #region Private Functions

    private void SpawnMe()
    {
        GameObject[] spawnPoints = SoccerGameManager.instance.spawnPoints;
        Vector3 startingPoint = spawnPoints[PhotonNetwork.countOfPlayers % spawnPoints.Length].transform.position;
        transform.position = startingPoint;
        transform.LookAt(Vector3.zero);
    }

    #endregion
}
