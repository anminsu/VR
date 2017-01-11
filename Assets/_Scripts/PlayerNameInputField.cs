using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Player name input field.  Will be used to display player name above head during game
/// </summary>
[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour {

    #region Private Variables

    // Store the PlayerPref key to avoid typos
    const string PLAYER_NAME_PREF_KEY = "PlayerName";

    #endregion

    #region MonoBehavior Callbacks

    void Start()
    {
        string defaultName = "";
        InputField _inputField = this.GetComponent<InputField>();
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(PLAYER_NAME_PREF_KEY))
            {
                defaultName = PlayerPrefs.GetString(PLAYER_NAME_PREF_KEY);
                _inputField.text = defaultName;
            }
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Sets the name of the player and saves it in PlayerPrefs for future reference
    /// </summary>
    /// <param name="value">The name of the Player</param>
    public void SetPlayerName(string value)
    {
        // #Important
        PhotonNetwork.playerName = value + " "; // Force empty space in case value is empty string.  Else playername would not be updated.

        PlayerPrefs.SetString(PLAYER_NAME_PREF_KEY, value);
    }

    #endregion
}
