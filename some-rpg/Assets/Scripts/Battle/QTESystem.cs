using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Script responsbile for creating QTEs and receiving QTE Inputs. <br></br>
/// QTE = Quick Time Event, When the player is prompted to press a button on the keyboard within a limited time.
/// </summary>
public class QTESystem : MonoBehaviour {
    public KeyCode QTEKey { get; private set; }
    [SerializeField] private float _maxTime = 1.0f;
    private float _currentTime;
    private bool _qteIsActive = false;

    public UnityEvent OnQTEFail;
    public UnityEvent<float> OnQTESuccess;

    /// <summary>
    /// Generates a QTE in which the player must input within a given _maxTime.
    /// A random key is chosen from the keyPool.
    /// </summary>
    /// <returns> Returns the key to be used for the QTE. </returns>
    public IEnumerator GenerateQTE(KeyCode[] keyPool) {
        int randomInt = Random.Range(0, keyPool.Length);
        KeyCode randomKey = keyPool[randomInt];

        QTEKey = randomKey;
        _currentTime = _maxTime;
        _qteIsActive = true;

        while (_qteIsActive) {
            Debug.Log("[QTE System] Time: " + _currentTime + " ; Key: " + QTEKey.ToString());

            if (_currentTime <= 0) {
                _qteIsActive = false;
                OnQTEFail.Invoke();
                break;
            }

            if (Input.GetKeyUp(QTEKey)) {
                OnQTESuccess.Invoke(_currentTime);
                Debug.Log("[QTE System] Key Success!");
                _qteIsActive = false;
            }

            yield return new WaitForSeconds(Time.deltaTime);
            _currentTime -= Time.deltaTime;
        }
    }
}
