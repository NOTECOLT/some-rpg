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
    public WeaponQTE Type { get; private set; }

    public QuickTimeEvent(GameObject qteButton, WeaponQTE type) {
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
        if (Type is PressQTE) {
            yield return PressQTE(keyPool, activeTime, leadTime);
        } else if (Type is MashQTE) {
            yield return MashQTE(keyPool, activeTime, leadTime);
        } else if (Type is ReleaseQTE) {
            yield return ReleaseQTE(keyPool, activeTime, leadTime);
        }
    }

    public IEnumerator WaitForQTEFinish() {
        while (Result == QTE_NULL_RESULT) yield return new WaitForSeconds(Time.deltaTime);
    }
    private IEnumerator PressQTE(KeyCode[] keyPool, float window, float leadTime) {
        TMP_Text qteKeyText = _qteButton.GetComponent<QTEPrompt>().QTEText;
        TMP_Text qteFlashText = _qteButton.GetComponent<QTEPrompt>().QTEFlashText;

        qteFlashText.text = "";

        Result = QTE_NULL_RESULT;

        QTEKey = keyPool[UnityEngine.Random.Range(0, keyPool.Length)];
        qteKeyText.text = QTEKey.ToString();
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
        qteFlashText.text = "Press!";

        while (_qteIsActive) {
            // Debug.Log($"[QTE System] Time left: {_currentTime}");
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
        
        qteFlashText.text = "";
        EndQTE();
    }
    
    private IEnumerator MashQTE(KeyCode[] keyPool, float window, float leadTime) {
        TMP_Text qteKeyText = _qteButton.GetComponent<QTEPrompt>().QTEText;
        TMP_Text qteFlashText = _qteButton.GetComponent<QTEPrompt>().QTEFlashText;

        Result = QTE_NULL_RESULT;
        _qteIsActive = true;
        qteFlashText.text = "";
        
        // LEAD TIME -----------------------------------------
        _currentTime = leadTime;
        while (_currentTime >= 0) {
            yield return new WaitForSeconds(Time.deltaTime);
            _currentTime -= Time.deltaTime;
        }

        // WINDOW -----------------------------------------
        QTEKey = keyPool[UnityEngine.Random.Range(0, keyPool.Length)];
        qteKeyText.text = QTEKey.ToString();
        qteFlashText.text = "Mash!";
        _qteButton.transform.localScale = Vector3.one * 0.7f;
        _qteButton.SetActive(true);
        _animator.SetBool("IsActive", true);
        _currentTime = window;
        int inputNumber = 0;

        while (_qteIsActive) {
            // Debug.Log($"[QTE System] Time left: {_currentTime}");

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

        qteFlashText.text = "";
        _animator.SetBool("IsActive", false);
        Debug.Log($"[QTE System] QTE Mashed: {inputNumber}!");
        _qteButton.transform.localScale = Vector3.one;
        Result = inputNumber;
        EndQTE();
    }

    private IEnumerator ReleaseQTE(KeyCode[] keyPool, float window, float leadTime) {
        TMP_Text qteKeyText = _qteButton.GetComponent<QTEPrompt>().QTEText;
        TMP_Text qteFlashText = _qteButton.GetComponent<QTEPrompt>().QTEFlashText;

        Result = QTE_NULL_RESULT;
        
        // LEAD TIME -----------------------------------------

        // Wait for the player to start holding the designated key
        QTEKey = keyPool[UnityEngine.Random.Range(0, keyPool.Length)];
        _qteIsActive = true;
        qteKeyText.text = QTEKey.ToString();
        _qteButton.SetActive(true);
        _animator.SetBool("IsActive", false);
        _qteButton.transform.localScale = Vector3.one * 0.7f;
        qteFlashText.text = "Hold!";
        while (!Input.GetKeyDown(QTEKey)) yield return new WaitForSeconds(Time.deltaTime);
        qteFlashText.text = "";
        
        _currentTime = leadTime;
        while (_currentTime >= 0) {
            yield return new WaitForSeconds(Time.deltaTime);
            _currentTime -= Time.deltaTime;
        }

        // WINDOW -----------------------------------------
        _animator.SetBool("IsActive", true);
        _currentTime = window;
        qteFlashText.text = "Release!";

        while (_qteIsActive) {
            // Debug.Log($"[QTE System] Time left: {_currentTime}");
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

        qteFlashText.text = "";
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

#region QTEType
public enum QTEType {
    PRESS,
    MASH,
    RELEASE
}

#endregion