using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUI : MonoBehaviour {

    #region Public Properties

    [Tooltip("UI text to display player's name")]
    public Text playerNameText;

    [Tooltip("UI Slider to display player's health")]
    public Slider playerHealthSlider;

    [Tooltip("Pixel offset from the player target")]
    public Vector3 screenOffset = new Vector3(0f, 30f, 0f);

    #endregion

    #region Private Properties

    private PlayerManager _target;
    private float _CharacterControllerHeight = 0f;
    private Transform _targetTransform;
    private Vector3 _targetPosition;

    #endregion

    #region Monobehavior Messages

    public void Awake()
    {
        this.GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
    }

    public void Update()
    {
        if (_target == null)
        {
            Destroy(this.gameObject);
            return;
        }

        if (playerHealthSlider != null)
            playerHealthSlider.value = _target.health;
    }

    public void LateUpdate()
    {
        if (_targetTransform != null)
        {
            _targetPosition = _targetTransform.position;
            _targetPosition.y += _CharacterControllerHeight;
            this.transform.position = Camera.main.WorldToScreenPoint(_targetPosition) + screenOffset;
        }
    }

    #endregion

    #region Public Methods

    public void SetTarget(PlayerManager target)
    {
        if (target == null)
        {
            Debug.LogError("PlayerManager target is null", this);
            return;
        }
        _target = target;

        if (playerNameText != null)
            playerNameText.text = _target.photonView.owner.name;

        CharacterController characterController = target.GetComponent<CharacterController>();
        if (characterController != null)
            _CharacterControllerHeight = characterController.height;
    }

    #endregion
}
