#if UNITY_5 && (!UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2 && ! UNITY_5_3) || UNITY_6
#define UNITY_MIN_5_4
#endif

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using ExitGames.Demos.DemoAnimator;
using System;

public class PlayerManager : Photon.PunBehaviour, IPunObservable {

    #region Public Variables

    public GameObject beams;
    public float health = 1f;
    [Tooltip("The local player instance.  Use this to know if the local player is represented in the scene")]
    public static GameObject localPlayerInstance;
    public GameObject playerUIPrefab;

    #endregion

    #region

    private bool isFiring;

    #endregion

    #region Monobehavior Callbacks

    public void Awake()
    {
        if (beams == null)
        {
            Debug.LogError("Missing beam reference", this);
        } else {
            beams.SetActive(false);
        }

        // #Important: Used in GameManager.cs to keep track of localPlayerInstance to prevent instantiation when levels are synchronized.
        if (photonView.isMine)
        {
            PlayerManager.localPlayerInstance = this.gameObject;
        }
        // #Critical: We flag as don't destroy on load so that the instance survives level synchronization thus giving a seamless experience when the levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        CameraWork _cameraWork = GetComponent<CameraWork>();

        if (playerUIPrefab != null)
        {
            GameObject _uiGo = Instantiate(playerUIPrefab) as GameObject;
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        } else {
            Debug.LogError("playerUIPrefab is null", this);
        }

        if (_cameraWork != null)
        {
            if (photonView.isMine)
                _cameraWork.OnStartFollowing();
        } else {
            Debug.LogError("Missing CameraWork component on prefab", this);
        }

#if UNITY_MIN_5_4
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += (scene, loadingMode) =>
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        };
#endif
    }

    public void Update()
    {
        if (photonView.isMine)
            ProcessInputs();

        if (health <= 0f)
            GameManager.instance.LeaveRoom();

        if (beams != null && isFiring != beams.GetActive())
            beams.SetActive(isFiring);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!photonView.isMine) return;
        if (!other.name.Contains("Beam")) return;

        health -= 0.1f;
    }

    public void OnTriggerStay(Collider other)
    {
        if (!photonView.isMine) return;
        if (!other.name.Contains("Beam")) return;

        health -= 0.1f * Time.deltaTime;
    }

#if !UNITY_MIN_5_4
    /// <summary> See CalledOnLevelWasLoaded. Outdated in Unity 5.4 </summary>
    void OnLevelWasLoaded(int level)
    {
        this.CalledOnLevelWasLoaded(level);
    }
#endif

    private void CalledOnLevelWasLoaded(int level)
    {
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }

        GameObject _uiGo = Instantiate(playerUIPrefab) as GameObject;
        _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    #endregion

    #region Custom Methods

    private void ProcessInputs()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isFiring)
                isFiring = true;
        }
        if (Input.GetButtonUp("Fire1"))
        {
            if (isFiring)
                isFiring = false;
        }
    }

#endregion

#region IPunObservable
    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player.  Send others our data
            stream.SendNext(isFiring);
            stream.SendNext(health);
        } else {
            // Network player.  Recieve data
            this.isFiring = (bool)stream.ReceiveNext();
            this.health = (float)stream.ReceiveNext();
        }
    }
    
#endregion
}
