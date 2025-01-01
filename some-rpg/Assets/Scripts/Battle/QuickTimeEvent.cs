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
    private Animator _animator;

    /// <summary>
    /// The Result of the QTE.
    /// Result = -1 if the QTE was a fail,
    /// Result >=0 if the QTE was a success
    /// </summary>
    public float Result;

    public QuickTimeEvent(GameObject qteButton) {
        _qteButton = qteButton;
        _animator = qteButton.GetComponent<Animator>();
        _qteButton.SetActive(false);
    }

    /// <summary>
    /// Generates a QTE in which the player must input within a given _maxTime.
    /// A random key is chosen from the keyPool.
    /// </summary>
    /// <param name="keyPool">The pool of possible valid keys that a QTE may be.</param>
    /// <param name="activeTime">The amount of time that a QTE is active and can be executed with success</param>
    /// <param name="leadTime">The amount of time before a QTE is active. This can be 0 for no lead time at all.</param>
    /// <returns> Returns the key to be used for the QTE. </returns>
    public IEnumerator GenerateQTE(KeyCode[] keyPool, float activeTime, float leadTime = 0) {
        QTEKey = keyPool[Random.Range(0, keyPool.Length)];
        _qteButton.GetComponentInChildren<TMP_Text>().text = QTEKey.ToString();
        _qteButton.SetActive(true);
        _qteButton.transform.localScale = Vector3.one * 0.7f;

        // Lead Time
        _currentTime = leadTime;
        _animator.SetBool("IsActive", false);
        while (_currentTime >= 0) {
            _qteButton.transform.localScale += Vector3.one * ((1f - 0.7f)/leadTime) * Time.deltaTime;
            
            yield return new WaitForSeconds(Time.deltaTime);
            _currentTime -= Time.deltaTime;

            // Second condition allows for some leeway in failing the quick time event
            if (Input.anyKey && _currentTime >= 0.2f) {
                SetQTEFail();
                EndQTE();
                yield break;
            }
        }

        _qteButton.transform.localScale = Vector3.one;
        
        // Active Time
        _currentTime = activeTime;
        _qteIsActive = true;
        
        _animator.SetBool("IsActive", true);
        while (_qteIsActive) {
            // Check if the QTE's active time reached the end
            if (_currentTime <= 0) {
                SetQTEFail();
                _qteIsActive = false;
                break;
            }

            // Check for any input for the QTE
            if (Input.anyKey) {
                if (Input.GetKey(QTEKey)) {
                    Result = _currentTime;
                    Debug.Log("[QTE System] QTE Success!");
                    _qteIsActive = false;          
                } else {
                    SetQTEFail();
                    _qteIsActive = false;
                    break;
                }
            }

            yield return new WaitForSeconds(Time.deltaTime);
            _currentTime -= Time.deltaTime;
        }
        
        EndQTE();
    }

    private void SetQTEFail() {
        Result = -1;
        Debug.Log("[QTE System] QTE Fail, wrong input!");
    }

    private void EndQTE() {
        _animator.SetBool("IsActive", false);
        _qteButton.SetActive(false);
    }
}
