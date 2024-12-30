using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Script responsbile for creating QTEs and receiving QTE Inputs. <br></br>
/// QTE = Quick Time Event, When the player is prompted to press a button on the keyboard within a limited time.
/// </summary>
public class QTESystem : MonoBehaviour {
    public KeyCode QTEKey { get; private set; }
    private float _currentTime;
    private bool _qteIsActive = false;

    [SerializeField] private GameObject _qteButton;

    public UnityEvent OnQTEFail;
    public UnityEvent<float> OnQTESuccess;

    void Start() {
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
                OnQTEFail.Invoke();
                Debug.Log("[QTE System] QTE Fail, no input!");
                _qteIsActive = false;
                break;
            }

            if (Input.anyKey) {
                if (Input.GetKey(QTEKey)) {
                    OnQTESuccess.Invoke(_currentTime);
                    Debug.Log("[QTE System] QTE Success!");
                    _qteIsActive = false;          
                } else {
                    OnQTEFail.Invoke();
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
