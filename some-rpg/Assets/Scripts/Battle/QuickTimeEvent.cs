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
        switch (Type) {
            case QTEType.PRESS:
                yield return PressQTE(keyPool, activeTime, leadTime);
                break;
            case QTEType.MASH:
                yield return MashQTE(keyPool, activeTime, leadTime);
                break;
            case QTEType.RELEASE:
                yield return ReleaseQTE(keyPool, activeTime, leadTime);
                break;
        }
    }

    public IEnumerator WaitForQTEFinish() {
        while (Result == QTE_NULL_RESULT) yield return new WaitForSeconds(Time.deltaTime);
    }
    private IEnumerator PressQTE(KeyCode[] keyPool, float window, float leadTime) {
        Result = QTE_NULL_RESULT;

        QTEKey = keyPool[UnityEngine.Random.Range(0, keyPool.Length)];
        _qteButton.GetComponentInChildren<TMP_Text>().text = QTEKey.ToString();
        _qteButton.SetActive(true);
        _qteButton.transform.localScale = Vector3.one * 0.7f;
        _qteIsActive = true;

        // LEAD TIME -----------------------------------------
        _currentTime = leadTime;
        _animator.SetBool("IsActive", false);
        while (_currentTime >= 0) {
            _qteButton.transform.localScale += Vector3.one * ((1f - 0.7f)/leadTime) * Time.deltaTime;
            
            yield return new WaitForSeconds(Time.deltaTime);
            _currentTime -= Time.deltaTime;

            // Second condition allows for some leeway in failing the quick time event
            if (Input.anyKey) {
                SetQTEFail();
                EndQTE();
                yield break;
            }
        }

        _qteButton.transform.localScale = Vector3.one;
        
        // WINDOW  -----------------------------------------
        _currentTime = window;
        _animator.SetBool("IsActive", true);

        while (_qteIsActive) {
            // Check if the QTE's window reached the end
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
    
    private IEnumerator MashQTE(KeyCode[] keyPool, float window, float leadTime) {
        Result = QTE_NULL_RESULT;
        _qteIsActive = true;
        
        // LEAD TIME -----------------------------------------
        _currentTime = leadTime;
        while (_currentTime >= 0) {
            yield return new WaitForSeconds(Time.deltaTime);
            _currentTime -= Time.deltaTime;
        }

        // WINDOW -----------------------------------------
        QTEKey = keyPool[UnityEngine.Random.Range(0, keyPool.Length)];
        _qteButton.GetComponentInChildren<TMP_Text>().text = QTEKey.ToString();
        _qteButton.transform.localScale = Vector3.one * 0.7f;
        _qteButton.SetActive(true);
        _animator.SetBool("IsActive", true);
        _currentTime = window;
        int inputNumber = 0;

        while (_qteIsActive) {
            // Check if the QTE's window reached the end
            if (_currentTime <= 0) {
                _qteIsActive = false;
                break;
            }

            _qteButton.transform.localScale += Vector3.one * ((1f - 0.7f)/window) * Time.deltaTime;
            
            // Check for any input for the QTE
            if (Input.GetKeyUp(QTEKey)) {
                inputNumber += 1;
            }

            yield return new WaitForSeconds(Time.deltaTime);
            _currentTime -= Time.deltaTime;
        }

        _animator.SetBool("IsActive", false);
        Debug.Log($"[QTE System] QTE Mashed: {inputNumber}!");
        _qteButton.transform.localScale = Vector3.one;
        Result = inputNumber;
        EndQTE();
    }

    private IEnumerator ReleaseQTE(KeyCode[] keyPool, float window, float leadTime) {
        Result = QTE_NULL_RESULT;
        
        // LEAD TIME -----------------------------------------

        // Wait for the player to start holding the designated key
        QTEKey = keyPool[UnityEngine.Random.Range(0, keyPool.Length)];
        _qteIsActive = true;
        _qteButton.GetComponentInChildren<TMP_Text>().text = QTEKey.ToString();
        _qteButton.SetActive(true);
        _animator.SetBool("IsActive", false);
        _qteButton.transform.localScale = Vector3.one * 0.7f;
        while (!Input.GetKeyDown(QTEKey)) yield return new WaitForSeconds(Time.deltaTime);

        _currentTime = leadTime;
        while (_currentTime >= 0) {
            yield return new WaitForSeconds(Time.deltaTime);
            _currentTime -= Time.deltaTime;
        }

        // WINDOW -----------------------------------------
        _animator.SetBool("IsActive", true);
        _currentTime = window;
        
        while (_qteIsActive) {
            // Check if the QTE's window reached the end
            if (_currentTime <= 0) {
                SetQTEFail();
                _qteIsActive = false;
                break;
            }

            // Check for any input for the QTE
            if (Input.GetKeyUp(QTEKey)) {
                Result = QTE_SUCCESS_RESULT;
                Debug.Log("[QTE System] QTE Success!");
                _qteIsActive = false;     
                break; 
            }

            _qteButton.transform.localScale += Vector3.one * ((1f - 0.7f)/window) * Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
            _currentTime -= Time.deltaTime;
        }

        _animator.SetBool("IsActive", false);
        _qteButton.transform.localScale = Vector3.one;
        EndQTE();
    }

    private void SetQTEFail() {
        Result = QTE_FAIL_RESULT;
        Debug.Log($"[QTE System] QTE Fail!");
    }

    private void EndQTE() {
        _animator.SetBool("IsActive", false);
        _qteButton.SetActive(false);
    }
}

public enum QTEType {
    PRESS,
    MASH,
    RELEASE
}