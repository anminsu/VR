using UnityEngine;
using System.Collections;

public class PlayerAnimatorManager : Photon.MonoBehaviour {

    #region Public Variables

    public float directionDampTime = .25f;

    #endregion

    #region Private Variables

    private Animator _animator;

    #endregion

    #region Monobehavior Messages

    void Start()
    {
        _animator = GetComponent<Animator>();
        if (!_animator)
            Debug.LogError("Animator Component not found", this);
    }

    void Update()
    {
        if (photonView.isMine == false && PhotonNetwork.connected == true) return;

        if (!_animator) return;

        // Deal with jumping
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        // Only allowing jumping if we are running
        if (stateInfo.IsName("Base Layer.Run"))
        {
            if (Input.GetButtonDown("Fire2")) _animator.SetTrigger("Jump");
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (v < 0) v = 0;

        _animator.SetFloat("Speed", h * h + v * v);
        _animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
    }

    #endregion

}
