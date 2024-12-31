using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting.FullSerializer;

/// <summary>
/// Object responsible for creating QTEs and receiving QTE Inputs. <br></br>
/// QTE = Quick Time Event, When the player is prompted to press a button on the keyboard within a limited time.
/// </summary>
public class QuickTimeEvent {
    public KeyCode QTEKey { get; private set; }
    private float _currentTime;
    private bool _qteIsActive = false;

    private GameObject _qteButton;

    /// <summary>
    /// The Result of the QTE.
    /// Result = -1 if the QTE was a fail,
    /// Result >=0 if the QTE was a success
    /// </summary>
    public float Result;

    public QuickTimeEvent(GameObject qteButton) {
        _qteButton = qteButton;

        _qteButton.SetActive(false);
    }

    /// <summary>
    /// Generates a QTE in which the player must input within a given _maxTime.
    /// A random key is chosen from the keyPool.
    /// </summary>
    /// <returns> Returns the key to be used for the QTE. </returns>
    public IEnumerator GenerateQTE(KeyCode[] keyPool, float time) {
        int randomInt = Random.Range(0, keyPool.Length);
        KeyCode randomKey = keyPool[randomInt];

        QTEKey = randomKey;
        _currentTime = time;
        _qteIsActive = true;

        _qteButton.SetActive(true);
        _qteButton.GetComponentInChildren<TMP_Text>().text = QTEKey.ToString();
        while (_qteIsActive) {
            // Debug.Log("[QTE System] Time: " + _currentTime + " ; Key: " + QTEKey.ToString());

            if (_currentTime <= 0) {
                Result = -1;
                Debug.Log("[QTE System] QTE Fail, no input!");
                _qteIsActive = false;
                break;
            }

            if (Input.anyKey) {
                if (Input.GetKey(QTEKey)) {
                    Result = _currentTime;
                    Debug.Log("[QTE System] QTE Success!");
                    _qteIsActive = false;          
                } else {
                    Result = -1;
                    Debug.Log("[QTE System] QTE Fail, wrong input!");
                    _qteIsActive = false;
                    break;
                }
            }

            yield return new WaitForSeconds(Time.deltaTime);
            _currentTime -= Time.deltaTime;
        }

        _qteButton.SetActive(false);
    }
}
