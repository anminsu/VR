using UnityEngine;
using System.Collections;

public class SoccerGameManager : MonoBehaviour {

    #region Public Variables

    public static SoccerGameManager instance;
    public GameObject[] spawnPoints;

    #endregion

    #region Monobehavior Callbacks

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn");
    }

    #endregion

    #region Public Methods

    public void Test(string test)
    {
        Debug.Log(test);
    }

    #endregion

}
