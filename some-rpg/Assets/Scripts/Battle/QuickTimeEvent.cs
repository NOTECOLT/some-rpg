using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Unity.VisualScripting.FullSerializer;
using System;

/// <summary>
/// Object responsible for creating QTEs and receiving QTE Inputs. <br></br>
/// QTE = Quick Time Event, When the player is prompted to press a button on the keyboard within a limited time.
/// </summary>
public class QuickTimeEvent {
    public static int QTE_NULL_RESULT = -1;
    public static int QTE_FAIL_RESULT = 0;
    public static int QTE_SUCCESS_RESULT = 1;
    private float _currentTime;
    private bool _qteIsActive = false;
    private GameObject _qteButton;
    private Animator _animator;

    public KeyCode QTEKey { get; private set; }
    public int Result;
    public QTEType Type { get; private set; }

    public QuickTimeEvent(GameObject qteButton, QTEType type) {
        _qteButton = qteButton;
        _animator = qteButton.GetComponent<Animator>();
        _qteButton.SetActive(false);

        Type = type;

        Result = QTE_NULL_RESULT;
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
        QTEKey = keyPool[UnityEngine.Random.Range(0, keyPool.Length)];
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
                    Result = QTE_SUCCESS_RESULT;
                    Debug.Log("[QTE System] QTE Success!");
                    _qteIsActive = false;     
                    break;     
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
        Result = QTE_FAIL_RESULT;
        Debug.Log($"[QTE System] QTE Result: {Result}!");
    }

    private void EndQTE() {
        _animator.SetBool("IsActive", false);
        _qteButton.SetActive(false);
    }
}

public enum QTEType {
    SINGLE,
    MASH
}